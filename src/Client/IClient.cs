using System.Threading.Tasks;

namespace TinCanPhone.Client
{
    public interface IClient
    {
        Task<string> SendAsync(string message);
    }
}
