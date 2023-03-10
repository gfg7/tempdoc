using gRPCContract.Models.ErrorModel;
using gRPCContract.Models.Request;
using gRPCContract.Utils;
using gRPCServer.Services.Management;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System.Text;
using TempDocTest.Mock.TestData;

namespace TempDocTest
{
    public class BucketTest : IClassFixture<TestFileInfo>
    {
        private readonly BucketManagementService _service;
        public BucketTest(BucketManagementService service)
        {
            _service = service;
        }

        [Fact]
        public async void GetNonExistingBucketAsync()
        {
            var bucketName = "nonExistentBucket";

            await Assert.ThrowsAsync<BucketNotFoundException>(async () => await _service.GetBucket(bucketName));
        }

        [Fact]
        public async Task UploadFilesAsync()
        {
            var bucketName = "NewBucket";
            var testFile = Encoding.UTF8.GetBytes("Test file content");
            var testFileStream = new MemoryStream(Encoding.UTF8.GetBytes("Test file content"));
            IFormFileCollection files = new FormFileCollection()
            {
                new FormFile(testFileStream, 0, testFileStream.Length, TestFileInfo.Valid.Filename, TestFileInfo.Valid.Filename)
            };

            await _service.UploadFiles(bucketName, files);

            var result = await _service.GetBucket(bucketName);
            var infoFile = result.First(x => x.Filename == TestFileInfo.Valid.Filename);
            var (filename, file) = await _service.GetFile(bucketName, infoFile.Id);

            Assert.NotNull(file);
            Assert.Equal(filename, infoFile.Filename);
            Assert.Equal(TestFileInfo.Valid.Filename, infoFile.Filename);
            Assert.Equal(testFile.Length, infoFile.Size);
            Assert.Equal(bucketName, infoFile.BucketName);
            Assert.Equal(infoFile.UploadDate.AddMinutes(int.Parse(Env.Get("DEFAULT_TEMP_TIME"))), infoFile.KeepTillDate);
        }

        [Fact]
        public async void DeleteExistingFileAsync()
        {
            var bucketName = TestFileInfo.Valid.BucketName;
            var fileCode = TestFileInfo.Valid.Id;
            var initial = await _service.GetBucket(bucketName);

            await _service.DeleteFile(bucketName, fileCode);

            var result = await _service.GetBucket(bucketName);
            var deletedFile = initial.Except(result).FirstOrDefault();

            Assert.NotEmpty(result);
            Assert.False(initial.SequenceEqual(result));
            Assert.NotEqual(initial, result);
            Assert.NotNull(deletedFile);
            Assert.Equal(fileCode, deletedFile.Id);
            Assert.Equal(bucketName, deletedFile.BucketName);
        }

        [Fact]
        public async Task DeleteNonExistingFileAsync()
        {
            var bucketName = TestFileInfo.Valid.BucketName;
            var fileCode = ObjectId.Empty.ToString();
            var initial = await _service.GetBucket(bucketName);

            await _service.DeleteFile(bucketName, fileCode);

            var result = await _service.GetBucket(bucketName);

            Assert.NotEmpty(result);
            Assert.True(initial.SequenceEqual(result));
            Assert.Equal(initial, result);

            await Assert.ThrowsAsync<GridFSFileNotFoundException>(async () => await _service.GetFile(bucketName, fileCode));
        }

        [Fact]
        public async Task DeleteNonExistingBucketAsync()
        {
            var bucketName = "nonExistentBucket";

            await Assert.ThrowsAsync<BucketNotFoundException>(async () => await _service.DropBucket(bucketName));
        }

        [Fact]
        public async Task DeleteExistingBucketAsync()
        {
            var bucketName = TestFileInfo.Valid.BucketName;

            await _service.DropBucket(bucketName);

            await Assert.ThrowsAsync<BucketNotFoundException>(async () => await _service.GetBucket(bucketName));
        }

        [Fact]
        public async Task DownloadNonExistingFileAsync()
        {
            var bucketName = TestFileInfo.Valid.BucketName;
            var fileCode = ObjectId.Empty.ToString();

            await Assert.ThrowsAsync<GridFSFileNotFoundException>(async () => await _service.GetFile(bucketName, fileCode));
        }

        [Fact]
        public async Task DownloadFromNonExistingBucketAsync()
        {
            var bucketName = "nonExistentBucket";
            var fileCode = ObjectId.Empty.ToString();

            await Assert.ThrowsAsync<GridFSFileNotFoundException>(async () => await _service.GetFile(bucketName, fileCode));
        }

        [Fact]
        public async Task DownloadExistingFileAsync()
        {
            var bucketName = TestFileInfo.Valid.BucketName;
            var fileCode = TestFileInfo.Valid.Id;
            var expected = (await _service.GetBucket(bucketName)).First(x => x.Id == fileCode);

            var (filename, file) = await _service.GetFile(bucketName, fileCode);

            Assert.Equal(expected.Filename, filename);
            Assert.Equal(expected.Size, file.Length);
        }

        [Fact]
        public async Task UpdateExistingFileAsync()
        {
            var bucketName = TestFileInfo.Valid.BucketName;
            var fileCode = TestFileInfo.Valid.Id;
            var settings = new FileDtoRequest()
            {
                Description = "test",
                KeepFor = 25
            };
            var initial = await _service.GetBucket(bucketName);

            await _service.SetExtraSettings(fileCode, settings);

            var result = await _service.GetBucket(bucketName);
            var modified = result.FirstOrDefault(x => x.Id == fileCode);

            Assert.NotEmpty(result);
            Assert.NotNull(modified);
            Assert.False(initial.SequenceEqual(result));
            Assert.NotEqual(initial, result);
            Assert.True(initial.Count == result.Count);
            Assert.True(modified.Description == settings.Description);
            Assert.True(modified.KeepTillDate == modified.UploadDate.AddMinutes(settings.KeepFor));
        }

        [Fact]
        public async void UpdateNonExistingFileAsync()
        {
            var fileCode = ObjectId.Empty.ToString();
            var settings = new FileDtoRequest()
            {
                Description = "test",
                KeepFor = 25
            };

            await Assert.ThrowsAsync<FileInfoNotFoundException>(async () => await _service.SetExtraSettings(fileCode, settings));
        }
    }
}