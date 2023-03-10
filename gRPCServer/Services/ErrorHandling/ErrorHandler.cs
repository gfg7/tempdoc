using MongoDB.Driver;

namespace gRPCServer.Services.ErrorHandling {
    public class ErrorHandler {
        public async Task<TResponse> MapErrorMessage<TResponse>(Func<Task<object>> service)
         where TResponse : class
        {
            try
            {
                return (await service()) as TResponse;
            }
            catch (System.Exception ex)
            {
                var message = ex.Message;

                if (ex is TimeoutException )
                {
                    message = "Execution timeout\nSomething wrong with db availability";
                }

                if (ex is MongoConnectionException)
                {
                    message = "Connection failure\nCheck your client credentials and call out to our support";
                }

                throw new Exception(message);
            }
        }
    }
}
