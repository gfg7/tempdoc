using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace gRPCServer.Intefaces.Repository
{
    public interface IFilesRepository
    {
        Task DeleteByBucket();
        Task DeleteFile(ObjectId id);
        Task<GridFSDownloadStream<ObjectId>> GetAsync(ObjectId id);
        void SetBucketName(string bucketName);
        Task<ObjectId> Upsert(string filename, Stream file);
    }
}