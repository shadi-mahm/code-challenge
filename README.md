# Tin Can Phone
## Introduction
You may had created some sort of this basic communication device in your childhood.
In this modern implementation, the solution is built on top of gRPC with 2 different approaches.
I decided to implement challenge's `IClient` interface using Unary gRPC method because I think the essence of a request/response behaviour is more like a gRPC unary call rather than Bidirectional streaming. 
However I have also implemented a bidirectional streaming to utilize the full potential of gRPC.
 - **Unary call**
  In this implementation client sends a single request to the server and gets a single response back. In addition the client maintains in-flight request collection to handle responses as soon as they're returned from the server.
  
 - **Bidirectional streaming**
  In this implementation client and server sends a sequence of messages using a read and write stream. This implementation guarantees response order relative to requests.
  For example a request stream of `Hello - Ping - Ping - Hello - Ping` is accepted non-blockingly and the response `Hi - Pong - Pong - Hi - Pong` is asynchronously streamed back to client.

## Project structure
An overview of the solution:
- `TinCanPhone.Common`: contains shared contracts.
- `TinCanPhone.Client`: a gRPC implementation of `IClient`.
- `TinCanPhone.ClientRunner`: a console application to demonstrate client functionality.
- `TinCanPhone.Server`: a gRPC server on top of ASP.NET Core based on challenge criteria.
- `TinCanPhone.Server.Tests`: a test project containing unit tests for server.

Issues/comments are noted in summary tags in the source code.

## Technologies
gRPC

.NET 5.0

ASP.NET Core

## How to run
Run Server Application via Visual Studio (`F5`) or command-line (`dotnet run`).

Run Client Runner console via Visual Studio (`F5`) or command-line (`dotnet run`).

Then you can choose between 2 communication methods to send and receive messages.

## Issues
  - Since HTTP/2 over TLS is not supported on macOS due to missing ALPN support, I used http protocol for server application.
  If you run the application on Windows you can change server protocol to https and nothing will be broken.
  - Although one can easily close connection in HTTP/1.x via `Connection` header, with gRPC technology over HTTP/2 I couldn't manage to close underlying connection from   server as it was requested in the challenge's criteria.
