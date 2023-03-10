
namespace gRPCContract.Models.Request
{
    public record FileDtoRequest
    {
        public int KeepFor { get; set; }
        public string? Description { get; set; }
    }
}
