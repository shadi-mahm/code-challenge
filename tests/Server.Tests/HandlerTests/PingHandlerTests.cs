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
            //act
            var result = _pingHandler.CanHandle(null);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CanHandle_EmptyArgument_ReturnsFalse()
        {
            //arrange
            var message = string.Empty;

            //act
            var result = _pingHandler.CanHandle(message);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CanHandle_InvalidArgument_ReturnsFalse()
        {
            //arrange
            var message = "invalid message";

            //act
            var result = _pingHandler.CanHandle(message);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CanHandle_ValidArgument_ReturnsTrue()
        {
            //arrange
            var message = Messages.Ping;

            //act
            var result = _pingHandler.CanHandle(message);

            //assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_ReturnsResponse()
        {
            //arrange
            var response = "Pong";

            //act
            var result = await _pingHandler.Handle();

            //assert
            Assert.NotNull(result);
            Assert.Equal(response, result.Response);
        }
    }
}

