using TinCanPhone.Client.Contracts;

namespace TinCanPhone.Client.Models
{
    public class DefaultResponseMessage : IResponseMessage
    {
        public string Response { get; set; }
    }
}
