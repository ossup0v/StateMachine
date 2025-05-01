using StateMachine.StateMachineBase;

namespace StateMachine.Example;

public class StoppingState : IState<NetworkStateContext>
{
    public Task Run(NetworkStateContext context, CancellationToken ct)
    {
        context.Connected = false;
        context.StartConnect = false;
        return Task.CompletedTask;
    }
}