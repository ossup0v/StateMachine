using StateMachine.AsyncEx;
using StateMachine.Loggers;
using StateMachine.StateMachineBase;

namespace StateMachine.Example;

public class NetworkStateContext : IStateContext
{
    public bool Connected { get; set; }
    public bool StartConnect { get; set; }
    public bool GotError { get; set; }
    public ILogger Logger { get; set; }

    private readonly Queue<Request> _requestsToSend = new ();
    private readonly AsyncLock _lock = new();
    public readonly ITransport Transport = new WebSocketTransport();
    public readonly IResponseProcessor ResponseProcessor = new ResponseProcessor();

    public NetworkStateContext(ILogger logger)
    {
        Logger = logger;
    }

    public async Task<Request?> TryGetRequest()
    {
        using var _ = await _lock.LockAsync();
        _requestsToSend.TryDequeue(out var request);
        return request;
    }

    public async Task AddRequest(Request request)
    {
        using var _ = await _lock.LockAsync();
        _requestsToSend.Enqueue(request);
    }
}