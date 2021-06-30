using System;
using TinCanPhone.Client.Contracts;

namespace TinCanPhone.Client.Models
{
    public class ExtendedReponseMessage : IResponseMessage
    {
        public string Response { get; set; }
        public DateTimeOffset ResponseDateTime { get; set; } = DateTimeOffset.Now;
    }
}
