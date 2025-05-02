using StateMachine.AsyncEx;

namespace StateMachine.Example;

public class WebSocketTransport : ITransport
{
    private readonly AsyncLock _receiveLock = new();

    public event Func<Response, Task> OnResponseReceived = _ => Task.CompletedTask;

    public Task Send(Request request, CancellationToken ct)
    {
        if (int.Parse(request.Name.Last().ToString()) % 3 == 0) throw new Exception($"Invalid request {request}");
        
        Console.WriteLine($"[WS] Sent {request.Name}");
        return Task.CompletedTask;
    }

    public async Task InsertResponseToQueue(Response response, CancellationToken ct)
    {
        using var _ = await _receiveLock.LockAsync(ct);
        await OnResponseReceived(response);
    }
}