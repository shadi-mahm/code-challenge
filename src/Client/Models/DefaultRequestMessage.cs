using TinCanPhone.Client.Contracts;

namespace TinCanPhone.Client.Models
{
    public class DefaultRequestMessage : IRequestMessage
    {
        public string Message { get; set; }
    }
}
