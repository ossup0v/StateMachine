using StateMachine.StateMachineBase;

namespace StateMachine.Example;

public class GotErrorState : IState<NetworkContext>
{
    public Task Enter(NetworkContext context, CancellationToken ct) => Task.CompletedTask;

    public async Task Execute(NetworkContext context, CancellationToken ct)
    {
        context.Error = null;
        
        var pending = context.PendingRequests.ToArray();
        context.PendingRequests.Clear();

        foreach (var (id, request) in pending)
        {
            request.SentTime = DateTime.UtcNow;
            if (!context.ReceivedResponses.Contains(id))
            {
                await context.RequestQueue.EnqueueAsync(request, ct);
            }
            else
            {
                context.Logger.LogInformation($"Already received response for request {request}. So, don't add it to the queue to resend.");
            }
        }
    }

    public Task Exit(NetworkContext context, CancellationToken ct) => Task.CompletedTask;
}