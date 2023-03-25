
namespace gRPCServer.Models.CustomException

{
    public class FileInfoNotFoundException : Exception
    {
        public FileInfoNotFoundException(string message = "Information about file not found") : base(message)
        {
        }
    }
}
