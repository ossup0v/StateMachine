using StateMachine.Loggers;

namespace StateMachine.StateMachineBase;

public interface IStateContext
{
    ILogger Logger { get; }
}