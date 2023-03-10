using gRPCContract.Models.Request;
using gRPCContract.Models.Stored;
using Microsoft.AspNetCore.Http;

namespace gRPCContract.Interfaces
{
    public interface IClientBucketManagement
    {
        Task<List<StoredFileInfo>> GetBucket(string bucket);
        Task<(string, MemoryStream)> GetFile(string bucket, string code);
        Task<StoredFileInfo> SetExtraSettings(string code, FileDtoRequest extra);
        Task<List<StoredFileInfo>> UploadFiles(string bucket, IFormFileCollection files);
    }
}