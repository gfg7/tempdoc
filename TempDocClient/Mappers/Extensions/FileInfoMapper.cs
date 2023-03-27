using WebContract.Models.Stored;
using FileInfo = ProtoContract.Protos.FileInfo;

namespace TempDocClient.Mappers.Extensions;

public static class FileInfoMapper
{
    public static StoredFileInfo FromProto(this FileInfo source, string bucket) {
        return new() {
            Id = source.FileBase.Code,
            Filename = source.Filename,
            BucketName = bucket,
            Size = source.Size,
            Description = source.Description,
            UploadDate = source.UploadDate.ToDateTime(),
            KeepTillDate = source.ExpireDate.ToDateTime(),
            KeepFor = ((int)(source.ExpireDate - source.UploadDate).ToTimeSpan().TotalMinutes)
        };
    } 
}
