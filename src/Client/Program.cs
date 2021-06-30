using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinCanPhone.Common;

namespace TinCanPhone.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine(@"Press:
h for ""Hello"" message
p for ""Ping"" message
b for ""Bye"" message
q for quit");

            Uri serviceUri = new("http://localhost:5000");

            IClient client = new Client(serviceUri);

            await HandleUnaryCall(client).ConfigureAwait(false);
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
                        inflightRequests.Add(WriteInConsoleAndSend(Messages.Hello));
                        break;
                    case 'p':
                        inflightRequests.Add(WriteInConsoleAndSend(Messages.Ping));
                        break;
                    case 'b':
                        inflightRequests.Add(WriteInConsoleAndSend(Messages.Bye));
                        exit = true;
                        break;
                    case 'q':
                        exit = true;
                        break;                  
                    default:
                        inflightRequests.Add(WriteInConsoleAndSend("invalid message"));
                        break;
                }
            }

            await Task.WhenAll(inflightRequests).ConfigureAwait(false);

            async Task WriteInConsoleAndSend(string message)
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
        }
    }
}
