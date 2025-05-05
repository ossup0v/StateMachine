using System.Collections.Concurrent;
using StateMachine.AsyncEx;
using StateMachine.Loggers;
using StateMachine.StateMachineBase;

namespace StateMachine.Example;

public class NetworkContext : IStateContext, IDisposable
{
    public bool Connected { get; set; }
    public bool StartConnect { get; set; }
    public bool GotError => Error != null;
    
    public Exception? Error { get; set; }
    
    public ILogger Logger { get; set; }

    public readonly ITransport Transport;

    public readonly ConcurrentBag<int> ReceivedResponses = new (); 
    public readonly ConcurrentDictionary<int, Request> PendingRequests = new();

    public readonly AsyncProducerConsumerQueue<Request> RequestQueue = new();
    public event Func<Task> OnRequestReceived = () => Task.CompletedTask;
    public readonly IRequestSender RequestSender;
    
    private readonly AsyncProducerConsumerQueue<Response> _responseQueue = new();
    public event Func<Task> OnResponseReceived = () => Task.CompletedTask;
    public readonly IResponseProcessor ResponseProcessor;


    public NetworkContext(ITransport transport, ILogger logger)
    {
        RequestSender = new RequestSender(this);
        ResponseProcessor = new ResponseProcessor(this);
        Logger = logger;
        Transport = transport;
        transport.OnResponseReceived += ResponseReceived;
    }

    public async Task AddRequest(Request request)
    {
        await RequestQueue.EnqueueAsync(request);
        await OnRequestReceived();
    }

    private async Task ResponseReceived(Response response)
    {
        await _responseQueue.EnqueueAsync(response);
        await OnResponseReceived();
    }

    public Task<bool> RequestsIsEmptyAsync()
    {
        return RequestQueue.IsEmptyAsync();
    }

    public Task<Request> DequeueRequest()
    {
        return RequestQueue.DequeueAsync();
    }

    public Task<bool> ResponsesIsEmptyAsync()
    {
        return _responseQueue.IsEmptyAsync();
    }

    public Task<Response> DequeueResponse()
    {
        return _responseQueue.DequeueAsync();
    }

    public void Dispose()
    {
        Transport.OnResponseReceived -= ResponseReceived;
    }

    public void SetTimeoutException(Request request)
    {
        Error = new Exception($"Timeout from request {request}");
    }
}