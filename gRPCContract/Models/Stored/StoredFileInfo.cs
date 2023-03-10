using gRPCContract.Models.Request;

namespace gRPCContract.Models.Stored
{
    public record StoredFileInfo : FileDtoRequest
    {
        public string Id { get; set; }
        public string BucketName { get; set; }
        public string Filename { get; set; }
        //bytes
        public long Size { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime KeepTillDate { get; set; }
    }
}
