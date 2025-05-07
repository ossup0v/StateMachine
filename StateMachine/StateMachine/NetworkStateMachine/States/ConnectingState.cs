using Shared.Contracts;
using StateMachine.StateMachineBase;

namespace StateMachine.NetworkStateMachine;

public class ConnectingState : IState<NetworkContext>
{
    public Task Enter(NetworkContext context, CancellationToken ct) => Task.CompletedTask;

    public async Task Execute(NetworkContext context, CancellationToken ct)
    {
        try
        {
            await context.Transport.Connect("ws://localhost:5000/ws/", ct);
            var response = await context.Transport.SendAndWait(new Request(999, "Auth"), ct);
            context.Logger.LogInformation($"Connection established {response}");
            context.Connected = true;
        }
        catch (Exception ex)
        {
            context.Error = ex;
            context.Logger.LogError($"Can't connect {ex.Message}");
        }
    }

    public Task Exit(NetworkContext context, CancellationToken ct) => Task.CompletedTask;
}