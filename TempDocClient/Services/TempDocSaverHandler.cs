using Grpc.Core;
using ProtoContract.Protos;
using TempDocClient.Mappers.Extensions;
using WebContract.Interfaces;
using WebContract.Models.Request;
using WebContract.Models.Stored;
using File = ProtoContract.Protos.File;

namespace TempDocClient.Services
{
    class TempDocSaverHandler : IClientBucketManagement
    {
        private readonly TempDocSaver.TempDocSaverClient _client;

        public TempDocSaverHandler(TempDocSaver.TempDocSaverClient client)
        {
            _client = client;
        }

        public async Task<List<StoredFileInfo>> GetBucket(string bucket)
        {
            var stream = _client.GetBucket(new BucketBase()
            {
                Name = bucket
            }).ResponseStream;

            var result = new List<StoredFileInfo>();

            while (await stream.MoveNext())
            {
                var stored = stream.Current.Stored;
                result.AddRange(stored.Select(x=> x.FromProto(bucket)));
            }

            return result;
        }

        public async Task<(string, MemoryStream)> GetFile(string bucket, string code)
        {
            var stream = _client.GetFile(new BucketFileQuery()
            {
                BucketBase = new BucketBase()
                {
                    Name = bucket
                },
                FileBase = new FileBase()
                {
                    Code = code
                }
            }).ResponseStream;

            MemoryStream content = null;
            string filename = string.Empty;

            while (await stream.MoveNext())
            {
                content = new MemoryStream(stream.Current.File_.ToByteArray());
                filename = stream.Current.Filename;
            }

            return (filename, content);
        }

        public async Task<StoredFileInfo> SetExtraSettings(string bucket, string code, FileDtoRequest extra)
        {
            var result = await _client.SetExtraSettingsAsync(new FileExtra()
            {
                BaseInfo = new BucketFileQuery()
                {
                    BucketBase = null,
                    FileBase = new FileBase()
                    {
                        Code = code
                    }
                },
                Extra = new Extra()
                {
                    KeepFor = extra.KeepFor,
                    Description = extra.Description
                }
            });

            return result.FromProto(bucket);
        }

        public async Task<List<StoredFileInfo>> UploadFiles(string bucket, IFormFileCollection files)
        {
            var call = _client.UploadFiles();

            foreach (var item in files)
            {
                var upload = new BucketUpload()
                {
                    BucketBase = new BucketBase()
                    {
                        Name = bucket
                    }
                };

                upload.File.Add(new File()
                {
                    Filename = item.FileName,
                    File_ = await Google.Protobuf.ByteString.FromStreamAsync(item.OpenReadStream())
                });

                await call.RequestStream.WriteAsync(upload);
            }

            await call.RequestStream.CompleteAsync();
            var result = await call.ResponseAsync;

            var storedInfo = result.Stored.Select(x => x.FromProto(bucket));

            return storedInfo.ToList();
        }
    }
}