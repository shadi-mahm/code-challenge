using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using TinCanPhone.Common.Contracts;
using TinCanPhone.Client.Exceptions;
using TinCanPhone.Client.Models;
using TinCanPhone.Protos;
using TinCanPhone.Common;

namespace TinCanPhone.Client
{
    public class GrpcClient : IClient
    {
        private readonly GrpcChannel _channel;

        private readonly Receiver.ReceiverClient _client;

        public GrpcClient(Uri serviceAddress)
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

        public async Task<IResponseMessage> SendAsync(IRequestMessage request)
        {
            var result = await SendAsync<DefaultResponseMessage>(request);

            return result;
        }

        /// <summary>
        /// Note: Method is not able to instantiate response message without new() as TResponseMessage constraint
        /// </summary>
        /// <typeparam name="TResponseMessage"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResponseMessage> SendAsync<TResponseMessage>(IRequestMessage request) where TResponseMessage : IResponseMessage, new()
        {
            try
            {
                var result = await SendAsync(request.Message);

                TResponseMessage responseMessage = new() { Response = result };

                return responseMessage;
            }
            catch
            {
                throw;
            }
        }

        public AsyncDuplexStreamingCall<RequestMessage, ResponseMessage> GetBidirectionalCall()
        {
            var call = _client.HandleBidirectionalMessages();

            return call;
        }
    }
}
