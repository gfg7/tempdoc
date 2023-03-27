
namespace gRPCServer.Models.CustomException
{
    public class BucketNotFoundException : Exception
    {
        public BucketNotFoundException(string message = "Bucket do not exist") : base(message)
        {
        }
    }
}
