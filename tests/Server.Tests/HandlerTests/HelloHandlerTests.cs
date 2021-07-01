using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TinCanPhone.Common;
using TinCanPhone.Server.Handlers;
using Xunit;

namespace TinCanPhone.Server.Tests.HandlerTests
{
    public class HelloHandlerTests
    {
        private readonly IMessageHandler _helloHandler;

        public HelloHandlerTests()
        {
            _helloHandler = new HelloHandler();
        }

        [Fact]
        public void CanHandle_NullArgument_ReturnsFalse()
        {
            //act
            var result = _helloHandler.CanHandle(null);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CanHandle_EmptyArgument_ReturnsFalse()
        {
            //arrange
            var message = string.Empty;

            //act
            var result = _helloHandler.CanHandle(message);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CanHandle_InvalidArgument_ReturnsFalse()
        {
            //arrange
            var message = "invalid message";

            //act
            var result = _helloHandler.CanHandle(message);

            //assert
            Assert.False(result);
        }

        [Fact]
        public void CanHandle_ValidArgument_ReturnsTrue()
        {
            //arrange
            var message = Messages.Hello;

            //act
            var result = _helloHandler.CanHandle(message);

            //assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_ReturnsResponseWithDelay()
        {
            //arrange
            var stopWatch = new Stopwatch();
            var response = "Hi";

            //act
            stopWatch.Start();
            var result = await _helloHandler.Handle();
            var timeSpan = stopWatch.Elapsed;

            //assert
            Assert.NotNull(result);
            Assert.Equal(response, result.Response);
            Assert.True(timeSpan >= TimeSpan.FromSeconds(1));
        }
    }
}
