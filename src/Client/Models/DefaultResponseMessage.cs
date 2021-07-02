using TinCanPhone.Common.Contracts;

namespace TinCanPhone.Client.Models
{
    public record DefaultResponseMessage : IResponseMessage
    {
        public string Response { get; init; }
    }
}
