using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace TinCanPhone.Server
{
    public class ReceiverService : Receiver.ReceiverBase
    {
        private readonly ILogger<ReceiverService> _logger;
        public ReceiverService(ILogger<ReceiverService> logger)
        {
            _logger = logger;
        }
        
        public override Task<ResponseMessage> HandleMessages(RequestMessage request, ServerCallContext context)
        {
            return Task.FromResult(new ResponseMessage
            {
                Response = "Hello " + request.Message
            });
        }
    }
}
