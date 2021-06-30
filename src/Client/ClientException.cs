using System;
namespace TinCanPhone.Client
{
    public class ClientException : ApplicationException
    {
        public ClientException(string message, Exception exception) : base(message, exception)
        {

        }
    }
}
