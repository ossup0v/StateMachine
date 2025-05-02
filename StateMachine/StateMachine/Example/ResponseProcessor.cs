using StateMachine.AsyncEx;

namespace StateMachine.Example;

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

            try
            {
                Console.WriteLine($"{response} processed");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _context.Logger.LogError($"Trying process response {response}. But got error {ex.Message}");
                _context.GotError = true;
            }
        }
    }
}