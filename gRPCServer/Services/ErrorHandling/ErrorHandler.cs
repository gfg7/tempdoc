using System.Runtime.InteropServices.ComTypes;
using MongoDB.Driver;

namespace gRPCServer.Services.ErrorHandling
{
    public class ErrorHandler
    {
        private readonly ILogger _logger;

        public ErrorHandler(ILogger<ErrorHandler> logger)
        {
            _logger = logger;
        }
        
        public async Task<TResponse> MapErrorMessage<TResponse>(Func<Task<object>> service)
         where TResponse : class
        {
            try
            {
                return (await service()) as TResponse;
            }
            catch (System.Exception ex)
            {
                var message = string.Empty;
                var level = LogLevel.Critical;

                switch (ex)
                {
                    case TimeoutException:
                        message = "Execution timeout. Something wrong with db availability";
                        break;
                    case MongoConnectionException:
                        message = "Connection failure. Check your client credentials and call out to our support";
                        break;
                    default:
                        level = LogLevel.Error;
                        message = ex.Message;
                        break;
                }

                _logger.Log(level, ex, message, service.Method.GetParameters());

                throw new Exception(message);
            }
        }
    }
}
