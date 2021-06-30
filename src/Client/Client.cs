using System;
using System.Threading.Tasks;
using Grpc.Core;
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

            _client = new Receiver.ReceiverClient(_channel);
        }

        public async Task<string> SendAsync(string message)
        {
            var request = new RequestMessage { Message = message };

            try
            {
                var result = await _client.HandleUnaryMessagesAsync(request);

                return result.Response;
            }
            catch (RpcException ex)
            {
                throw new ClientException("Error occured while calling gRPC service.", ex);
            }
        }
    }
}
