namespace StateMachine.StateMachineBase;

public interface IState<in TContext> where TContext : IStateContext
{
    Task Run(TContext context, CancellationToken ct);
}