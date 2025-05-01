using StateMachine.StateMachineBase;

namespace StateMachine.Example;

public class StoppedState : IState<NetworkStateContext>
{
    public Task Run(NetworkStateContext context, CancellationToken ct)
    {
        context.StartConnect = true;
        return Task.CompletedTask;
    }
}