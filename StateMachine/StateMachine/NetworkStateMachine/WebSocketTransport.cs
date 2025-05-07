using System.Net.WebSockets;
using Shared.Contracts;
using StateMachine.AsyncEx;

namespace StateMachine.NetworkStateMachine;

public class WebSocketTransport : ITransport
{
    private readonly AsyncLock _receiveLock = new();
    private readonly AsyncLock _sendLock = new();
    
    private MemoryStream _sendStream = new();

    private CancellationTokenSource? _receiveThreadCts;
    private ClientWebSocket? _socket;

    public async Task Connect(string connectionString, CancellationToken ct)
    {
        _receiveThreadCts = new CancellationTokenSource();
        var serverUri = new Uri(connectionString);
        _socket = new ClientWebSocket();
        
        await _socket.ConnectAsync(serverUri, ct);

        ReceiveThread(_receiveThreadCts.Token).Forget();
    }

    private async Task ReceiveThread(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && _socket != null)
        {
            var receiveBuffer = new byte[1024];
            var ms = new MemoryStream(1024);
            
            var result = await _socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            await ms.WriteAsync(receiveBuffer, 0, result.Count, ct);
            ms.Position = 0;
                
            var response = new Response();
            response.Deserialize(ms);
            await ms.FlushAsync(ct);
            
            Console.WriteLine($"[WS] Received: {response}");
            
            await OnResponseReceived(response);
        }
    }

    public async Task Disconnect(CancellationToken ct)
    {
        if (_socket == null) return;

        _receiveThreadCts?.CancelAsync();
        _receiveThreadCts = null;
        
        //var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        //timeout.CancelAfter(TimeSpan.FromSeconds(1));
        //await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", timeout.Token);
        //timeout.Dispose();
        _socket.Dispose();
        _socket = null;
    }

    public event Func<Response, Task> OnResponseReceived = _ => Task.CompletedTask;

    public async Task Send(Request request, CancellationToken ct)
    {
        if (_socket == null) throw new NullReferenceException($"Trying to send request {request}, but socket is null");
        //if (int.Parse(request.Name.Last().ToString()) % 3 == 0) throw new Exception($"Invalid request {request}");
        using var _ = await _sendLock.LockAsync();
        _sendStream = new MemoryStream(1024);
        request.Serialize(_sendStream);
        var buffer = _sendStream.ToArray();
        await _sendStream.FlushAsync(ct);
        
        await _socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, CancellationToken.None);
        
        Console.WriteLine($"[WS] Sent {request}");
    }

    public async Task<Response> SendAndWait(Request request, CancellationToken ct)
    {
        await Send(request, ct);
        Response? response = default;
        var wait = new AsyncEvent();

        Task ResponseReceived(Response rsp)
        {
            response = rsp;
            wait.FireEvent();
            Console.WriteLine($"[WS] Send And Wait Received {response}");
            return Task.CompletedTask;
        }

        OnResponseReceived += ResponseReceived;
        await wait.WaitAsync(ct);
        
        OnResponseReceived -= ResponseReceived;
        return response!;
    }
}