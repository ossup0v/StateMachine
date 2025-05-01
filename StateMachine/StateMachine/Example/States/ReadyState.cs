using StateMachine.StateMachineBase;

namespace StateMachine.Example;

public class ReadyState : IState<NetworkStateContext>
{
    public async Task Run(NetworkStateContext context, CancellationToken ct)
    {
        try
        {
            var request = await context.TryGetRequest() ?? new Request("Ping1");
            if (int.Parse(request.Name.Last().ToString()) % 3 == 0) throw new Exception($"Test exception on request {request.Name}");

            await context.Transport.Send(request, ct);
            ct.ThrowIfCancellationRequested();

            var response = await context.Transport.Receive(ct);
            ct.ThrowIfCancellationRequested();

            if (response == null) return;

            await context.ResponseProcessor.Process(response);
            
            ct.ThrowIfCancellationRequested();
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            context.GotError = true;
            context.Logger.LogError(exception.Message);
        }
    }
}