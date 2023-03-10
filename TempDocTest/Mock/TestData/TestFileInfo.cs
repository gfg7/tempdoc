using gRPCContract.Models.Stored;
using gRPCContract.Utils;
using MongoDB.Bson;
using System.Text;

namespace TempDocTest.Mock.TestData
{
    public class TestFileInfo
    {
        public static StoredFileInfo Valid => new()
        {
            Filename = "TestFile.txt",
            Description = null,
            UploadDate = DateTime.Now,
            KeepFor = int.Parse(Env.Get("DEFAULT_TEMP_TIME")),
            BucketName = "bucket",
            Size = Encoding.UTF8.GetByteCount("Really really long test file content"),
            Id = "63f36b9fde6df9e3aad9bfc6"
        };

        public List<StoredFileInfo> Data { get; set; } = new()
        {
            Valid,
            new StoredFileInfo()
        {
            Filename = "TestFile.txt",
                Description = null,
                UploadDate = DateTime.Now,
                KeepFor = int.Parse(Env.Get("DEFAULT_TEMP_TIME")),
                BucketName = "test",
                Size = 1,
                Id = ObjectId.GenerateNewId().ToString()
            },
            new StoredFileInfo()
        {
            Filename = "TestFile1.txt",
                Description = "test",
                UploadDate = DateTime.Now,
                KeepFor = 5,
                BucketName = "bucket",
                Size = 100,
                Id = ObjectId.GenerateNewId().ToString()
            }
    };
    }
}
