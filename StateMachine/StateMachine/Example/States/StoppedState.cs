using StateMachine.StateMachineBase;

namespace StateMachine.Example;

public class StoppedState : IState<NetworkContext>
{
    public Task Enter(NetworkContext context, CancellationToken ct) => Task.CompletedTask;

    public Task Execute(NetworkContext context, CancellationToken ct)
    {
        context.StartConnect = true;
        return Task.CompletedTask;
    }
    
    public Task Exit(NetworkContext context, CancellationToken ct) => Task.CompletedTask;
}