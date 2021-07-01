using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using TinCanPhone.Client;
using TinCanPhone.Client.Contracts;
using TinCanPhone.Client.Exceptions;
using TinCanPhone.Client.Models;
using TinCanPhone.Common;
using TinCanPhone.Protos;

namespace TinCanPhone.ClientRunner
{
    class Program
    {
        private const bool UseUnary = true;

        static async Task Main(string[] args)
        {
            Console.WriteLine(@"Press:
h for ""Hello"" message
p for ""Ping"" message
b for ""Bye"" message
q for quit");

            Uri serviceUri = new("http://localhost:5000");

            GrpcClient client = new(serviceUri);

            if (UseUnary)
            {
                await HandleUnaryCall(client).ConfigureAwait(false);
            }
            else
            {
                await HandleBidirectionalCall(client).ConfigureAwait(false);
            }

            Console.WriteLine("Connection closed.");
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
    }
}
