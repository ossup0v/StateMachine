using StateMachine.StateMachineBase;

namespace StateMachine.NetworkStateMachine;

public class ReadyState : IState<NetworkContext>
{
    public Task Enter(NetworkContext context, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public async Task Execute(NetworkContext context, CancellationToken ct)
    {
        await context.RequestSender.Send();
        await context.ResponseProcessor.Process();
    }

    public Task Exit(NetworkContext context, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}