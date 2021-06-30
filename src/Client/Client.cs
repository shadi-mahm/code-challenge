using System;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace TinCanPhone.Client
{
    public class Client : IClient
    {
        private readonly GrpcChannel _channel;

        public Client(Uri serviceAddress)
        {
            _channel = GrpcChannel.ForAddress(serviceAddress);
        }

        public Task<string> SendAsync(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}
