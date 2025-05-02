namespace StateMachine.StateMachineBase;

public interface IState<in TContext> where TContext : IStateContext
{
    Task Enter(TContext context, CancellationToken ct);
    Task Execute(TContext context, CancellationToken ct);
    Task Exit(TContext context, CancellationToken ct);
}