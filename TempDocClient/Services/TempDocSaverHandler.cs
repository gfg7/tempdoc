using Grpc.Core;
using gRPCContract.Interfaces;
using gRPCContract.Models.Request;
using gRPCContract.Models.Stored;
using gRPCContract.Protos;
using File = gRPCContract.Protos.File;

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



            return null;
        }

        public Task<(string, MemoryStream)> GetFile(string bucket, string code)
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



            return null;
        }

        public Task<StoredFileInfo> SetExtraSettings(string code, FileDtoRequest extra)
        {
            var result = _client.SetExtraSettingsAsync(new FileExtra() {
                BaseInfo = new BucketFileQuery() {
                    BucketBase = null,
                    FileBase = new FileBase() {
                        Code = code
                    }
                },
                Extra = new Extra() {
                    KeepFor = extra.KeepFor,
                    Description = extra.Description
                }
            });

            return null;
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

            return null;
        }
    }
}