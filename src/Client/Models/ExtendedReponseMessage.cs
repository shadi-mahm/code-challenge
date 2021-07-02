using System;
using TinCanPhone.Common.Contracts;

namespace TinCanPhone.Client.Models
{
    public record ExtendedReponseMessage : IResponseMessage
    {
        public string Response { get; init; }
        public DateTimeOffset ResponseDateTime { get; init; } = DateTimeOffset.Now;
    }
}
