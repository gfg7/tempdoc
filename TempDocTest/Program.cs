using gRPCContract.Models.Stored;
using gRPCServer.Intefaces.Repository;
using gRPCServer.Services.Management;
using Microsoft.Extensions.DependencyInjection;
using TempDocTest.Mock.Repository;
using TempDocTest.Mock.TestData;

namespace TempDocTest
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            Environment.SetEnvironmentVariable("DEFAULT_MAX_COUNT", "10");
            Environment.SetEnvironmentVariable("DEFAULT_TEMP_TIME", "15");
            Environment.SetEnvironmentVariable("CHUNK_SIZE", "1024");
            Environment.SetEnvironmentVariable("DEFAULT_MAX_COUNT", "10");
            Environment.SetEnvironmentVariable("DEFAULT_MAX_FILE_SIZE", null);
            Environment.SetEnvironmentVariable("DEFAULT_MAX_BODY_SIZE", null);

            services.AddScoped<TestFileInfo>();

            services.AddScoped<IFilesRepository, FilesRepositoryMock>();
            services.AddScoped(typeof(IInfoRepository<StoredFileInfo>), typeof(InfoRepositoryMock));

            services.AddScoped<BucketManagementService>();
        }
    }
}