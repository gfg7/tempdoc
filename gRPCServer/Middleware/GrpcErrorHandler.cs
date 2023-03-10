using Grpc.Core;
using Grpc.Core.Interceptors;
using gRPCServer.Services.ErrorHandling;

namespace gRPCServer.Middleware
{
    public class GrpcErrorHandler : Interceptor
    {
        private readonly ErrorHandler _errorHandler;
        public GrpcErrorHandler(ErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }
        
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            return await _errorHandler.MapErrorMessage<TResponse>(async () => await continuation(request, context));
        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            return await _errorHandler.MapErrorMessage<TResponse>(async () => await continuation(requestStream, context));
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            await _errorHandler.MapErrorMessage<TResponse>(async () =>
            {
                await continuation(request, responseStream, context);
                return null;
            });
        }
    }
}
