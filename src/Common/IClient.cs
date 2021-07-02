using System.Threading.Tasks;
using TinCanPhone.Common.Contracts;

namespace TinCanPhone.Common
{
    public interface IClient
    {
        Task<string> SendAsync(string message);

        Task<IResponseMessage> SendAsync(IRequestMessage request);

        Task<TResponseMessage> SendAsync<TResponseMessage>(IRequestMessage request) where TResponseMessage : IResponseMessage, new();
    }
}
