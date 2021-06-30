using System;
using System.Threading.Tasks;
using TinCanPhone.Common;

namespace TinCanPhone.Server.Handlers
{
    public class ByeHandler : IMessageHandler
    {
        public bool CanHandle(string message)
        {
            return string.Equals(Messages.Bye, message, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<MessageHandlerOutput> Handle()
        {
            return await Task.FromResult(new MessageHandlerOutput(Responses.Bye)).ConfigureAwait(false);
        }
    }
}
