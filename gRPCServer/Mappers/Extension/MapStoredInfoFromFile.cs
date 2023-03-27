using gRPCServer.Services.Utils;
using WebContract.Models.Stored;

namespace gRPCServer.Mappers.Extension
{
    public static class MapInfoFromFile
    {
        public static StoredFileInfo FromFile(this StoredFileInfo info, string bucket, IFormFile file, string id=null)
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
    }
}