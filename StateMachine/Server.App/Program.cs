using System.Net;
using System.Net.WebSockets;
using Shared.Contracts;

namespace Server.App;

class Program
{
    static async Task Main(string[] args)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5000/ws/");
        listener.Start();
        Console.WriteLine("WebSocket server started at ws://localhost:5000/ws/");

        while (true)
        {
            var httpContext = await listener.GetContextAsync();

            if (httpContext.Request.IsWebSocketRequest)
            {
                var wsContext = await httpContext.AcceptWebSocketAsync(null);
                Console.WriteLine("Client connected.");

                WebSocket webSocket = wsContext.WebSocket;

                try
                {
                    while (webSocket.State == WebSocketState.Open)
                    {
                        var buffer = new byte[1024];
                        var responseMs = new MemoryStream(1024);
                        var requestMs = new MemoryStream(1024);

                        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        var request = new Request();
                        await requestMs.WriteAsync(buffer, 0, result.Count);
                        requestMs.Position = 0;
                        request.Deserialize(requestMs);
                        requestMs.Flush();
                        Console.WriteLine($"Received: {request}");

                        var response = new Response(request.Id, request.Name);
                        response.Serialize(responseMs);
                        var responseBytes = responseMs.ToArray();
                        await responseMs.FlushAsync();
                        Console.WriteLine($"Sent: {response}");

                        await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Binary,
                            true, CancellationToken.None);
                    }

                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    Console.WriteLine("Client disconnected.");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                httpContext.Response.StatusCode = 400;
                httpContext.Response.Close();
            }
        }
    }
}