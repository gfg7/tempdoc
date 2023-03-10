
namespace gRPCContract.Models.ErrorModel
{
    public class FileInfoNotFoundException : Exception
    {
        public FileInfoNotFoundException(string message = "Information about file not found") : base(message)
        {
        }
    }
}
