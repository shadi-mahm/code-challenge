using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace TinCanPhone.Server.Tests.Helpers
{
    /// <summary>
    ///     Copied from grpc-dotnet by James Newton-King: https://github.com/grpc/grpc-dotnet/blob/master/examples/Tester/Tests/UnitTests/Helpers/TestServerCallContext.cs
    /// </summary>
    public class MockServerCallContext : ServerCallContext
    {
        private readonly Metadata _requestHeaders;
        private readonly CancellationToken _cancellationToken;
        private readonly Metadata _responseTrailers;
        private readonly AuthContext _authContext;
        private WriteOptions _writeOptions;

        public Metadata ResponseHeaders { get; private set; }

        private MockServerCallContext(Metadata requestHeaders, CancellationToken cancellationToken)
        {
            _requestHeaders = requestHeaders;
            _cancellationToken = cancellationToken;
            _responseTrailers = new Metadata();
            _authContext = new AuthContext(string.Empty, new Dictionary<string, List<AuthProperty>>());
        }

        protected override string MethodCore => "MethodName";
        protected override string HostCore => "HostName";
        protected override string PeerCore => "PeerName";
        protected override DateTime DeadlineCore { get; }
        protected override Metadata RequestHeadersCore => _requestHeaders;
        protected override CancellationToken CancellationTokenCore => _cancellationToken;
        protected override Metadata ResponseTrailersCore => _responseTrailers;
        protected override Status StatusCore { get; set; }
        protected override WriteOptions WriteOptionsCore { get => _writeOptions; set { _writeOptions = value; } }
        protected override AuthContext AuthContextCore => _authContext;

        protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
        {
            if (ResponseHeaders != null)
            {
                throw new InvalidOperationException("Response headers have already been written.");
            }

            ResponseHeaders = responseHeaders;
            return Task.CompletedTask;
        }

        public static MockServerCallContext Create(Metadata requestHeaders = null, CancellationToken cancellationToken = default)
        {
            return new MockServerCallContext(requestHeaders ?? new Metadata(), cancellationToken);
        }
    }
}
