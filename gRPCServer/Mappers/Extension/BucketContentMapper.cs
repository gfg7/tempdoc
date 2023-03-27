using ProtoContract.Protos;
using WebContract.Models.Stored;

namespace gRPCServer.Mappers.Extension
{
    public static class BucketContentMapper
    {
        public static BucketContent ToProto(this IEnumerable<StoredFileInfo> source)
        {
            var content = new BucketContent()
            {
                BucketBase = new()
                {
                    Name = source.First().BucketName
                }
            };

            var stored = source.Select(x => x.ToProto());

            content.Stored.AddRange(stored);

            return content;
        }
    }
}