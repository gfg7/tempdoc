using Google.Protobuf;
using Grpc.Core;
using gRPCServer.Mappers;
using gRPCServer.Mappers.Extension;
using ProtoContract.Protos;
using WebContract.Interfaces;
using File = ProtoContract.Protos.File;
using FileInfo = ProtoContract.Protos.FileInfo;

namespace gRPCServer.Services.ProtosHandler
{
    public class TempDocSaverHandler : TempDocSaver.TempDocSaverBase
    {
        private readonly IClientBucketManagement _service;

        public TempDocSaverHandler(IClientBucketManagement service)
        {
            _service = service;
        }

        public override async Task<BucketContent> UploadFiles(IAsyncStreamReader<BucketUpload> requestStream, ServerCallContext context)
        {
            var bucket = requestStream.Current.BucketBase.Name;
            var files = new FormFileCollection();

            await foreach (var item in requestStream.ReadAllAsync<BucketUpload>())
            {
                foreach (var file in item.File)
                {
                    var stream = new MemoryStream(file.ToByteArray());
                    var name = file.Filename;

                    var uploaded = new FormFile(
                        stream,
                        0,
                        file.File_.Length,
                        name,
                        name
                    );
                    files.Add(uploaded);
                }
            }

            var result = await _service.UploadFiles(bucket, files);

            return result.ToProto();
        }

        public override async Task GetFile(BucketFileQuery request, IServerStreamWriter<File> responseStream, ServerCallContext context)
        {
            var (filename, stream) = await _service.GetFile(request.BucketBase.Name, request.FileBase.Code);

            var file = new File()
            {
                Filename = filename,
                File_ = ByteString.FromStream(stream)
            };

            await responseStream.WriteAsync(file);
        }

        public override async Task GetBucket(BucketBase request, IServerStreamWriter<BucketContent> responseStream, ServerCallContext context)
        {
            var bucket = await _service.GetBucket(request.Name);
            
            await responseStream.WriteAsync(bucket.ToProto());
        }

        public override async Task<FileInfo> SetExtraSettings(FileExtra request, ServerCallContext context)
        {
            var extra = request.Extra.ToDto();
            var bucket = request.BaseInfo.BucketBase.Name;
            var code = request.BaseInfo.FileBase.Code;

            var result = await _service.SetExtraSettings(bucket, code, extra);

            return result.ToProto();
        }
    }
}
