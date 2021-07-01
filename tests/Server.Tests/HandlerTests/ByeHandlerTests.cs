using System.Threading.Tasks;
using TinCanPhone.Common;
using TinCanPhone.Server.Handlers;
using Xunit;

namespace TinCanPhone.Server.Tests.HandlerTests
{
    public class ByeHandlerTests
    {
        private readonly IMessageHandler _byeHandler;

        public ByeHandlerTests()
        {
            _byeHandler = new ByeHandler();
        }

        [Fact]
        public void CanHandle_NullArgument_ReturnsFalse()
        {
            //act
            var result = _byeHandler.CanHandle(null);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CanHandle_EmptyArgument_ReturnsFalse()
        {
            //arrange
            var message = string.Empty;

            //act
            var result = _byeHandler.CanHandle(message);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CanHandle_InvalidArgument_ReturnsFalse()
        {
            //arrange
            var message = "invalid message";

            //act
            var result = _byeHandler.CanHandle(message);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CanHandle_ValidArgument_ReturnsTrue()
        {
            //arrange
            var message = Messages.Bye;

            //act
            var result = _byeHandler.CanHandle(message);

            //assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_ReturnsResponse()
        {
            //arrange
            var response = "Bye";

            //act
            var result = await _byeHandler.Handle();

            //assert
            Assert.NotNull(result);
            Assert.Equal(response, result.Response);
        }
    }
}
