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
    }
}
