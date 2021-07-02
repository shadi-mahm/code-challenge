using TinCanPhone.Common.Contracts;

namespace TinCanPhone.Client.Models
{
    public record DefaultRequestMessage : IRequestMessage
    {
        public string Message { get; init; }
    }
}
