using StateMachine.AsyncEx;

namespace StateMachine.NetworkStateMachine;

public class ResponseProcessor : IResponseProcessor
{
    private readonly NetworkContext _context;
    private readonly AsyncLock _lock = new ();
    
    public ResponseProcessor(NetworkContext context)
    {
        _context = context;
    }
    
    public async Task Process()
    {
        using var _ = await _lock.LockAsync();
        while (!await _context.ResponsesIsEmptyAsync() && !_context.GotError)
        {
            var response = await _context.DequeueResponse();
            
            if (response.RequestId.HasValue)
            {
                if (_context.PendingRequests.TryRemove(response.RequestId.Value, out var _))
                {
                    _context.Logger.LogInformation($"Removed pending request {response.RequestId}");
                }
                else
                {
                    _context.Logger.LogInformation($"Can't remove pending request {response.RequestId}");
                }
            }

            try
            {
                if (response.RequestId.HasValue) _context.ReceivedResponses.Add(response.RequestId.Value);
                Console.WriteLine($"{response} processed");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _context.Logger.LogError($"Trying process response {response}. But got error {ex.Message}");
                _context.Error = ex;
            }
        }
    }
}