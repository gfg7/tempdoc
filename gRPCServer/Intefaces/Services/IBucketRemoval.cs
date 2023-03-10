namespace gRPCServer.Intefaces.Services
{
    public interface IBucketRemoval
    {
        Task DeleteFile(string bucket, string code);
        Task DropBucket(string bucket);
        Task FlushExpired();
    }
}