using WebContract.Models.Stored;
using FileInfo = ProtoContract.Protos.FileInfo;
using Google.Protobuf.WellKnownTypes;

namespace gRPCServer.Mappers.Extension
{
    public static class FileInfoMapper
    {
        public static FileInfo ToProto(this StoredFileInfo source)
        {
            return new()
            {
                Description = source.Description,
                ExpireDate = source.KeepTillDate.ToTimestamp(),
                UploadDate = source.UploadDate.ToTimestamp(),
                Size = source.Size,
                Filename = source.Filename,
                FileBase = new()
                {
                    Code = source.Id
                }
            };
        }

    }
}