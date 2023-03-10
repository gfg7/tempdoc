using gRPCServer.Intefaces.Services;
using gRPCServer.Services.Management;
using Quartz;

namespace gRPCServer.Jobs
{
    public class DropExpiredJob : IJob
    {
        private readonly IBucketRemoval _service;

        public DropExpiredJob(IBucketRemoval service)
        {
            _service = service;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _service.FlushExpired();
        }
    }
}
