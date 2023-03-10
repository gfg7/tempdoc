using gRPCServer.Intefaces.Repository;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using Moq;
using System.Text;

namespace TempDocTest.Mock.Repository
{
    internal class FilesRepositoryMock : IFilesRepository
    {
        public Task DeleteByBucket() => Task.CompletedTask;

        public Task DeleteFile(ObjectId id) => Task.CompletedTask;

        public async Task<GridFSDownloadStream<ObjectId>> GetAsync(ObjectId id)
        {
            if (id == ObjectId.Empty)
            {
                throw new GridFSFileNotFoundException(id);
            }

            var testFile = "Really really long test file content";
            var binTestFile = Encoding.UTF8.GetBytes(testFile);

            var fileMock = new Mock<GridFSDownloadStream<ObjectId>>();
            // fileMock.SetupProperty(x=>x.Length, binTestFile.LongLength);
            // fileMock.SetupGet(x=>x.Length).Returns(binTestFile.LongLength);
            // fileMock.Setup(x=>x.Length).Returns(binTestFile.LongLength);
            // fileMock.Setup(x=> x.SetLength(binTestFile.LongLength))

            return fileMock.Object;
        }

        public void SetBucketName(string bucketName) { }

        public Task<ObjectId> Upsert(string filename, Stream file) => Task.FromResult(ObjectId.GenerateNewId(DateTime.Now));
    }
}
