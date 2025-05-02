using StateMachine.StateMachineBase;

namespace StateMachine.Example;

public class ReadyState : IState<NetworkContext>
{
    public Task Enter(NetworkContext context, CancellationToken ct)
    {
        context.OnRequestReceived += context.RequestSender.Send;
        context.OnResponseReceived += context.ResponseProcessor.Process;
        return Task.CompletedTask;
    }

    public Task Execute(NetworkContext context, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public Task Exit(NetworkContext context, CancellationToken ct)
    {
        context.OnRequestReceived -= context.RequestSender.Send;
        context.OnResponseReceived -= context.ResponseProcessor.Process;
        return Task.CompletedTask;
    }
}