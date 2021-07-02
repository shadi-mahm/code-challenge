using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using TinCanPhone.Protos;
using TinCanPhone.Server.Handlers;

namespace TinCanPhone.Server
{
    public class ReceiverService : Receiver.ReceiverBase
    {
        private readonly IEnumerable<IMessageHandler> _handlers;
        private readonly ILogger<ReceiverService> _logger;

        public ReceiverService(IEnumerable<IMessageHandler> handlers, ILogger<ReceiverService> logger)
        {
            _handlers = handlers;
            _logger = logger;
        }

        public override async Task<ResponseMessage> HandleUnaryMessages(RequestMessage request, ServerCallContext context)
        {
            var trimmedMessage = request.Message?.Trim();

            try
            {
                var handler = _handlers.SingleOrDefault(a => a.CanHandle(trimmedMessage));

                if (handler is not null)
                {
                    var result = await handler.Handle().ConfigureAwait(false);

                    var responseMessage = new ResponseMessage { Response = result?.Response };

                    _logger.LogInformation($"HandleUnaryMessages: request: {request} - response: {responseMessage}");

                    return responseMessage;
                }

                _logger.LogError($"HandleUnaryMessages: Handler not found for request: {request}");

                throw new RpcException(new Status(StatusCode.InvalidArgument, "No proper handler was found."));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "HandleUnaryMessages: There's an exception while finding a handler.");

                throw new RpcException(new Status(StatusCode.Internal, "Internal error"));
            }
        }

        /// <summary>
        ///     According to https://docs.microsoft.com/en-us/aspnet/core/grpc/performance?view=aspnetcore-5.0#streaming logic
        ///     is required to restart stream if there is an error, but I'm not able to do so right now hence it writes error
        ///     instead of throwing exception.
        /// </summary>
        /// <remarks>
        ///     Inspired by https://docs.microsoft.com/en-us/aspnet/core/grpc/services?view=aspnetcore-5.0#bi-directional-streaming-method
        /// </remarks>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task HandleBidirectionalMessages(IAsyncStreamReader<RequestMessage> requestStream, IServerStreamWriter<ResponseMessage> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("HandleBidirectionalMessages: Stream started...");

            await foreach (var item in requestStream.ReadAllAsync())
            {
                var trimmedMessage = item.Message?.Trim();

                try
                {
                    var handler = _handlers.SingleOrDefault(a => a.CanHandle(trimmedMessage));

                    if (handler is not null)
                    {
                        var result = await handler.Handle().ConfigureAwait(false);

                        var responseMessage = new ResponseMessage { Response = result?.Response };

                        _logger.LogInformation($"HandleBidirectionalMessages: request: {item} - response: {responseMessage}");

                        await responseStream.WriteAsync(responseMessage);
                    }
                    else
                    {
                        _logger.LogError($"HandleBidirectionalMessages: Handler not found for request: {item}");

                        await responseStream.WriteAsync(new ResponseMessage { Response = "invalid command" });
                    }
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "HandleBidirectionalMessages: There's an exception while finding a handler.");

                    await responseStream.WriteAsync(new ResponseMessage { Response = "internal error" });
                }
            }

            _logger.LogInformation("HandleBidirectionalMessages: Stream closed.");
        }
    }
}
