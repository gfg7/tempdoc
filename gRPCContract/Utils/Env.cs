namespace gRPCContract.Utils
{
    public static class Env
    {
        public static string Get(string paramName) => Environment.GetEnvironmentVariable(paramName.ToUpper()) ?? throw new Exception($"Environment variable \"{paramName}\" not set");
    }
}