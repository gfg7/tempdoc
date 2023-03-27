using ProtoContract.Protos;
using WebContract.Models.Request;

namespace TempDocClient.Mappers.Extensions;

public static class FileExtraMapper
{
    public static FileDtoRequest FromProto(this Extra source)
    {
        return new()
        {
            KeepFor = source.KeepFor ?? 0,
            Description = source.Description
        };
    }
}
