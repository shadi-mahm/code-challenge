using System.Threading.Tasks;
using TinCanPhone.Common;
using TinCanPhone.Server.Handlers;
using Xunit;

namespace TinCanPhone.Server.Tests.HandlerTests
{
    public class PingHandlerTests
    {
        private readonly IMessageHandler _pingHandler;

        public PingHandlerTests()
        {
            _pingHandler = new PingHandler();
        }

        [Fact]
        public void CanHandle_NullArgument_ReturnsFalse()
        {
            //logic
            var result = _pingHandler.CanHandle(null);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CanHandle_EmptyArgument_ReturnsFalse()
        {
            //arrange
            var message = string.Empty;

            //logic
            var result = _pingHandler.CanHandle(message);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CanHandle_InvalidArgument_ReturnsFalse()
        {
            //arrange
            var message = "invalid message";

            //logic
            var result = _pingHandler.CanHandle(message);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CanHandle_ValidArgument_ReturnsTrue()
        {
            //arrange
            var message = Messages.Ping;

            //logic
            var result = _pingHandler.CanHandle(message);

            //assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_ReturnsResponse()
        {
            //arrange
            var response = "Pong";

            //logic
            var result = await _pingHandler.Handle();

            //assert
            Assert.NotNull(result);
            Assert.Equal(response, result.Response);
        }
    }
}

