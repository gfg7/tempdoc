using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using MongoDB.Driver;
using WebContract.Models.Stored;
using gRPCServer.Services.Utils;
using WebContract.Models.Request;
using ProtoContract.Protos;
using File = ProtoContract.Protos.File;
using FileInfo = ProtoContract.Protos.FileInfo;

namespace gRPCServer.Mappers
{
    public static class Mapper
    {

        public static StoredFileInfo MapInfoFromFile(this StoredFileInfo info, string bucket, IFormFile file, string id=null)
        {
            var now = DateTime.UtcNow;
            var expiry = info.KeepFor == 0 ? int.Parse(Env.Get("DEFAULT_TEMP_TIME")) : info.KeepFor;

            var response = new StoredFileInfo
            {
                Id = id ?? info.Id,
                BucketName = bucket,
                Filename = file.FileName,
                Size = file.Length,
                UploadDate = now,
                Description = info.Description,
                KeepFor = expiry,
                KeepTillDate = now.AddMinutes(expiry)
            };

            return response;
        }
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

        public static File ToProto(this MemoryStream stream, string filename)
        {
            return new()
            {
                File_ = ByteString.FromStream(stream),
                Filename = filename
            };
        }

        public static BucketContent ToProto(this IEnumerable<StoredFileInfo> source)
        {
            var content = new BucketContent()
            {
                BucketBase = new()
                {
                    Name = source.First().BucketName
                }
            };

            var stored = source.Select(ToProto);

            content.Stored.AddRange(stored);

            return content;
        }

        public static FileDtoRequest ToDto(this Extra source)
        {
            return new()
            {
                Description = source.Description,
                KeepFor = source.KeepFor.HasValue? source.KeepFor.GetValueOrDefault() :  int.Parse(Env.Get("DEFAULT_TEMP_TIME"))
            };
        }
    }
}
