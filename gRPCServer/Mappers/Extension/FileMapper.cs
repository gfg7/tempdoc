using Google.Protobuf;
using File = ProtoContract.Protos.File;

namespace gRPCServer.Mappers.Extension
{
    public static class FileMapper
    {
        public static File ToProto(this MemoryStream stream, string filename)
        {
            return new()
            {
                File_ = ByteString.FromStream(stream),
                Filename = filename
            };
        }
    }
}