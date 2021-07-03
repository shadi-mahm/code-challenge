using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using TinCanPhone.Client;
using TinCanPhone.Common.Contracts;
using TinCanPhone.Client.Exceptions;
using TinCanPhone.Client.Models;
using TinCanPhone.Common;
using TinCanPhone.Protos;

namespace TinCanPhone.ClientRunner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var selectedChannel = SelectCommunicationChannel();

            // as HTTP/2 over TLS is not supported on macOS due to missing ALPN support, server runs on http.
            //If you run the application on Windows you can change 'server' protocol to https and nothing will be broken
            Uri serviceUri = new("http://localhost:5000");

            GrpcClient client = new(serviceUri);

            if (selectedChannel is CommunicationChannel.Unary)
            {
                Console.WriteLine("Unary method is selected.");

                PrintHelp();
                await HandleUnaryCall(client).ConfigureAwait(false);
                Console.WriteLine("Connection closed.");
            }
            else if (selectedChannel is CommunicationChannel.Bidirectional)
            {
                Console.WriteLine("Bidirectional method is selected.");

                PrintHelp();
                await HandleBidirectionalCall(client).ConfigureAwait(false);
                Console.WriteLine("Connection closed.");
            }
            else
            {
                Console.WriteLine("Invalid channel.");
            }
        }

        /// <summary>
        ///     Maintains an infinite loop over a collection of in-flight requests to a unary gRPC service.
        /// </summary>
        /// <remarks>
        ///     As I don't have any clue on how to close a connection in gRPC, I just break the loop after "Bye" command.
        /// </remarks>
        /// <param name="client"></param>
        /// <returns></returns>
        private static async Task HandleUnaryCall(IClient client)
        {
            List<Task> inflightRequests = new();

            bool exit = false;

            while (!exit)
            {
                var keyInfo = Console.ReadKey(true);
                var character = keyInfo.KeyChar;

                // all 3 different implementations of SendAsync are used to show they're doing the job well.
                switch (character)
                {
                    case 'h':
                        inflightRequests.Add(SendString(Messages.Hello));
                        break;
                    case 'p':
                        inflightRequests.Add(SendRequestMessage(Messages.Ping));
                        break;
                    case 'b':
                        inflightRequests.Add(SendCustomRequestMessage(Messages.Bye));
                        exit = true;
                        break;
                    case 'q':
                        exit = true;
                        break;
                    default:
                        inflightRequests.Add(SendString("invalid message"));
                        break;
                }
            }

            await Task.WhenAll(inflightRequests).ConfigureAwait(false);

            async Task SendString(string message)
            {
                try
                {
                    Console.WriteLine($"sending {message}...");

                    var result = await client.SendAsync(message).ConfigureAwait(false);

                    Console.WriteLine(result);
                }
                catch (ClientException ex)
                {
                    Console.WriteLine($"Client threw an exception: {ex.Message}");
                }
            }

            async Task SendRequestMessage(string message)
            {
                try
                {
                    IRequestMessage requestMessage = new DefaultRequestMessage { Message = message };

                    Console.WriteLine($"sending {requestMessage.Message}...");

                    var result = await client.SendAsync(requestMessage).ConfigureAwait(false);

                    Console.WriteLine(result.Response);
                }
                catch (ClientException ex)
                {
                    Console.WriteLine($"Client threw an exception: {ex.Message}");
                }
            }

            async Task SendCustomRequestMessage(string message)
            {
                try
                {
                    IRequestMessage requestMessage = new DefaultRequestMessage { Message = message };

                    Console.WriteLine($"sending {requestMessage.Message}...");

                    var result = await client.SendAsync<ExtendedReponseMessage>(requestMessage).ConfigureAwait(false);

                    Console.WriteLine(result.Response);
                }
                catch (ClientException ex)
                {
                    Console.WriteLine($"Client threw an exception: {ex.Message}");
                }
            }
        }

        /// <summary>
        ///     Using gRPC Bidirectional Call writes in and read from stream.
        /// </summary>
        /// <remarks>
        ///     As I don't have any clue on how to close a connection in gRPC, I just break the loop after "Bye" command.
        /// </remarks>
        /// <param name="client"></param>
        /// <returns></returns>
        private static async Task HandleBidirectionalCall(GrpcClient client)
        {
            using var call = client.GetBidirectionalCall();

            var task = Task.Run(async () =>
            {
                await foreach (var item in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(item.Response);
                }
            });

            bool exit = false;

            while (!exit)
            {
                var keyInfo = Console.ReadKey(true);
                var character = keyInfo.KeyChar;

                switch (character)
                {
                    case 'h':
                        await WriteMessage(Messages.Hello);
                        break;
                    case 'p':
                        await WriteMessage(Messages.Ping);
                        break;
                    case 'b':
                        await WriteMessage(Messages.Bye);
                        exit = true;
                        break;
                    case 'q':
                        exit = true;
                        break;
                    default:
                        await WriteMessage("invalid message");
                        break;
                }
            }

            await call.RequestStream.CompleteAsync();

            await task;

            async Task WriteMessage(string message)
            {
                Console.WriteLine($"sending {message}...");
                await call.RequestStream.WriteAsync(new RequestMessage { Message = message });
            }
        }

        private static CommunicationChannel SelectCommunicationChannel()
        {
            Console.WriteLine(@"Press:
u to use ""Unary Call"" 
b to use ""Bidirectional Call""
and any other key to quit");

            var keyInfo = Console.ReadKey(true);
            var character = keyInfo.KeyChar.ToString();

            if (string.Equals("u", character, StringComparison.OrdinalIgnoreCase))
            {
                return CommunicationChannel.Unary;
            }
            if (string.Equals("b", character, StringComparison.OrdinalIgnoreCase))
            {
                return CommunicationChannel.Bidirectional;
            }

            return CommunicationChannel.Unknown;
        }

        private static void PrintHelp()
        {
            Console.WriteLine(@"Press:
h for ""Hello"" message
p for ""Ping"" message
b for ""Bye"" message
q for quit");
        }
    }

    internal enum CommunicationChannel
    {
        Unary = 1,
        Bidirectional = 2,
        Unknown = 3
    }
}
