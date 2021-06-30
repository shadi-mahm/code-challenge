using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using TinCanPhone.Protos;
using TinCanPhone.Server.Handlers;

namespace TinCanPhone.Server
{
    public class ReceiverService : Receiver.ReceiverBase
    {
        private readonly IEnumerable<IMessageHandler> _handlers;

        public ReceiverService(IEnumerable<IMessageHandler> handlers)
        {
            _handlers = handlers;
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

                    return responseMessage;
                }

                throw new RpcException(new Status(StatusCode.InvalidArgument, "No proper handler was found."));
            }
            catch (InvalidOperationException)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Internal error"));
            }
        }
    }
}
