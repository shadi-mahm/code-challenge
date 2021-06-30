using System;

namespace TinCanPhone.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri serviceAddress = new("http://localhost:5000");

            var client = new Client(serviceAddress);
        }
    }
}
