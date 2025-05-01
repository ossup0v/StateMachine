using StateMachine.AsyncEx;

namespace StateMachine.Example;

public class WebSocketTransport : ITransport
{
    private readonly Queue<Response> _responses = new();
    private readonly AsyncLock _receiveLock = new();
    
    public Task Send(Request request, CancellationToken ct)
    {
        Console.WriteLine($"[WS] Sent {request.Name}");
        return Task.CompletedTask;
    }

    public async Task<Response?> Receive(CancellationToken ct)
    {
        using var _ = await _receiveLock.LockAsync(ct);
        _responses.TryDequeue(out var response);
        return response;
    }

    public async Task InsertResponseToQueue(Response response, CancellationToken ct)
    {
        using var _ = await _receiveLock.LockAsync(ct);
        _responses.Enqueue(response);
    }
}