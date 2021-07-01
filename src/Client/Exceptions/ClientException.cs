using System;
namespace TinCanPhone.Client.Exceptions
{
    public class ClientException : ApplicationException
    {
        public ClientException(string message, Exception exception) : base(message, exception)
        {

        }
    }
}
