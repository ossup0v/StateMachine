using StateMachine.StateMachineBase;

namespace StateMachine.Example;

public class GetErrorState : IState<NetworkStateContext>
{
    public Task Run(NetworkStateContext context, CancellationToken ct)
    {
        context.GotError = false;
        return Task.CompletedTask;
    }
}