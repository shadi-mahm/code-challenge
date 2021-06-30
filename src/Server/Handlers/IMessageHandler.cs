using System.Threading.Tasks;

namespace TinCanPhone.Server.Handlers
{
    public interface IMessageHandler
    {
        bool CanHandle(string message);

        Task<MessageHandlerOutput> Handle();
    }
}
