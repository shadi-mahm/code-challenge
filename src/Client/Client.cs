using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using TinCanPhone.Protos;

namespace TinCanPhone.Client
{
    public class Client : IClient
    {
        private readonly GrpcChannel _channel;

        private readonly Receiver.ReceiverClient _client;

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
