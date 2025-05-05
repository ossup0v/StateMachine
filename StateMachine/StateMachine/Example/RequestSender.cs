using StateMachine.AsyncEx;

namespace StateMachine.Example;

public class RequestSender : IRequestSender
{
    private readonly NetworkContext _context;
    private readonly AsyncLock _lock = new();

    public RequestSender(NetworkContext context)
    {
        _context = context;
    }
    
    public async Task Send()
    {
        using var _ = await _lock.LockAsync();
        var transport = _context.Transport;
        
        try
        {
            while (!await _context.RequestsIsEmptyAsync() && !_context.GotError)
            {
                var request = await _context.DequeueRequest();

                if (_context.GotError)
                {
                    _context.Logger.LogError($"Trying to send request {request}, but got error.");
                    return;
                }

                if (!_context.PendingRequests.TryAdd(request.Id, request))
                {
                    _context.Logger.LogInformation($"Can't add request {request} to pending requests.");
                }
                else
                {
                    _context.Logger.LogInformation($"Request {request} added to pending requests.");
                }
                request.SentTime = DateTime.UtcNow;
                await transport.Send(request, CancellationToken.None);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _context.Logger.LogInformation($"Trying to send, but got error: {ex.Message}");
            _context.Error = ex;
        }
    }
}