using System;
using System.Threading.Tasks;
using TinCanPhone.Common;

namespace TinCanPhone.Server.Handlers
{
    public class HelloHandler : IMessageHandler
    {
        public bool CanHandle(string message)
        {
            return string.Equals(Messages.Hello, message, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<MessageHandlerOutput> Handle()
        {
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            return await Task.FromResult(new MessageHandlerOutput(Responses.Hi)).ConfigureAwait(false);
        }
    }
}
