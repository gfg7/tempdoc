using gRPCServer.Models.CustomException;
using gRPCServer.Services.ErrorHandling;
using MongoDB.Driver;

namespace gRPCServer.Middleware
{
    public class WebErrorHandler
    {
        private RequestDelegate _request;
        private readonly ErrorHandler _errorHandler;
        public WebErrorHandler(RequestDelegate request, ErrorHandler errorHandler)
        {
            _request = request;
            _errorHandler = errorHandler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _errorHandler.MapErrorMessage<Task>(async () =>
                {
                    await _request.Invoke(context);
                    return null;
                });

            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(ex.Message);
            }
        }
    }
}
