using gRPCServer.Intefaces.Services;
using Quartz;

namespace gRPCServer.Jobs
{
    public class DropExpiredJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IBucketRemoval _service;

        public DropExpiredJob(IBucketRemoval service, ILogger<DropExpiredJob> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation($"Job {nameof(DropExpiredJob)} started");
                await _service.FlushExpired();
                _logger.LogInformation($"Job {nameof(DropExpiredJob)} finished");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Job {nameof(DropExpiredJob)} failed", ex);
            }
        }
    }
}
