using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TinCanPhone.Common;
using TinCanPhone.Protos;
using TinCanPhone.Server.Handlers;
using Xunit;

namespace TinCanPhone.Server.Tests.ServiceTests
{
    public class ReceiverServiceTests
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly ServiceProvider _serviceProvider;

        public ReceiverServiceTests()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddSingleton<IMessageHandler, HelloHandler>();
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task HandleUnaryMessages_ValidRequest_ReturnsValidResponse()
        {
            //arrange
            var handlers = _serviceProvider.GetServices<IMessageHandler>();
            var service = new ReceiverService(handlers, NullLogger<ReceiverService>.Instance);
            var expectedResponse = "Hi";

            //act
            var response = await service.HandleUnaryMessages(new RequestMessage { Message = Messages.Hello },
                MockServerCallContext.Create());

            //assert
            Assert.Equal(expectedResponse, response.Response);
        }

        [Fact]
        public async Task HandleUnaryMessages_NullHandler_ThrowsRpcException()
        {
            //arrange
            var handlers = _serviceProvider.GetServices<IMessageHandler>();
            var service = new ReceiverService(handlers, NullLogger<ReceiverService>.Instance);

            //assert
            await Assert.ThrowsAsync<RpcException>(() =>
                                    service.HandleUnaryMessages(new RequestMessage { Message = Messages.Bye }, MockServerCallContext.Create()));
        }

        [Fact]
        public async Task HandleUnaryMessages_DuplicateHandler_ThrowsRpcException()
        {
            //arrange
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IMessageHandler, SpamHandler>();
            serviceCollection.AddSingleton<IMessageHandler, HelloHandler>();

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            var handlers = serviceProvider.GetServices<IMessageHandler>();
            var service = new ReceiverService(handlers, NullLogger<ReceiverService>.Instance);

            //assert
            await Assert.ThrowsAsync<RpcException>(() =>
                                    service.HandleUnaryMessages(new RequestMessage { Message = Messages.Hello }, MockServerCallContext.Create()));
        }
    }

    internal class SpamHandler : IMessageHandler
    {
        public bool CanHandle(string message)
        {
            return true;
        }

        public async Task<MessageHandlerOutput> Handle()
        {   
            return await Task.FromResult(new MessageHandlerOutput(Responses.Hi)).ConfigureAwait(false);
        }
    }
}
