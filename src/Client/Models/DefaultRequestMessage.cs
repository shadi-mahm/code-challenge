using TinCanPhone.Client.Contracts;

namespace TinCanPhone.Client.Models
{
    public record DefaultRequestMessage : IRequestMessage
    {
        public string Message { get; init; }
    }
}
