using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using TinCanPhone.Server.Handlers;
using Xunit;

namespace TinCanPhone.Server.Tests.HandlerTests
{
    public class StrategyTests
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly ServiceProvider _serviceProvider;

        public StrategyTests()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddSingleton<IMessageHandler, HelloHandler>();
            _serviceCollection.AddSingleton<IMessageHandler, ByeHandler>();
            _serviceCollection.AddSingleton<IMessageHandler, PingHandler>();
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void InvalidMessage_ReturnsNull()
        {
            //arange
            var message = "invalid message";

            //logic
            var handlers = _serviceProvider.GetServices<IMessageHandler>();
            var handler = handlers.SingleOrDefault(a => a.CanHandle(message));

            //assert
            Assert.Null(handler);
        }

        [Theory]
        [InlineData("Hello")]
        [InlineData("Bye")]
        [InlineData("Ping")]
        public void ValidMessage_ReturnsHandler(string input)
        {
            //logic
            var handlers = _serviceProvider.GetServices<IMessageHandler>();
            var handler = handlers.SingleOrDefault(a => a.CanHandle(input));

            //assert
            Assert.NotNull(handler);
        }
    }
}
