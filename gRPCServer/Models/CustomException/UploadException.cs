
namespace gRPCServer.Models.CustomException

{
    public class UploadException : Exception
    {
        public UploadException(string message = "Something happened during upload. Please, try again") : base(message)
        {
        }
    }
}