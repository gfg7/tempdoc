using gRPCServer.Services.Utils;
using ProtoContract.Protos;
using WebContract.Models.Request;

namespace gRPCServer.Mappers.Extension
{
    public static class ExtraMapper
    {
        public static FileDtoRequest ToDto(this Extra source)
        {
            return new()
            {
                Description = source.Description,
                KeepFor = source.KeepFor.HasValue ? source.KeepFor.GetValueOrDefault() : int.Parse(Env.Get("DEFAULT_TEMP_TIME"))
            };
        }
    }
}