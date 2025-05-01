using StateMachine.StateMachineBase;

namespace StateMachine.Example;

public class ConnectingState : IState<NetworkStateContext>
{
    public Task Run(NetworkStateContext context, CancellationToken ct)
    {
        context.Connected = true;
        return Task.CompletedTask;
    }
}