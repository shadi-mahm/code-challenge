using System.Threading.Tasks;
using TinCanPhone.Client.Contracts;

namespace TinCanPhone.Client
{
    public interface IClient
    {
        Task<string> SendAsync(string message);

        Task<IResponseMessage> SendAsync(IRequestMessage request);

        Task<TResponseMessage> SendAsync<TResponseMessage>(IRequestMessage request) where TResponseMessage : IResponseMessage, new();
    }
}
