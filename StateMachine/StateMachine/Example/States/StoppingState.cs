using StateMachine.StateMachineBase;

namespace StateMachine.Example;

public class StoppingState : IState<NetworkContext>
{
    public Task Enter(NetworkContext context, CancellationToken ct) => Task.CompletedTask;

    public Task Execute(NetworkContext context, CancellationToken ct)
    {
        context.Connected = false;
        context.StartConnect = false;
        return Task.CompletedTask;
    }

    public Task Exit(NetworkContext context, CancellationToken ct) => Task.CompletedTask;
}