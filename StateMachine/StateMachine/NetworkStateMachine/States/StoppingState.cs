using StateMachine.StateMachineBase;

namespace StateMachine.NetworkStateMachine;

public class StoppingState : IState<NetworkContext>
{
    public Task Enter(NetworkContext context, CancellationToken ct) => Task.CompletedTask;

    public async Task Execute(NetworkContext context, CancellationToken ct)
    {
        await context.Transport.Disconnect(ct);
        context.Connected = false;
    }

    public Task Exit(NetworkContext context, CancellationToken ct) => Task.CompletedTask;
}