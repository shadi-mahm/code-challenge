using TinCanPhone.Client.Contracts;

namespace TinCanPhone.Client.Models
{
    public record DefaultResponseMessage : IResponseMessage
    {
        public string Response { get; init; }
    }
}
