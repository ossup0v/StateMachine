namespace StateMachine.StateMachineBase;

public class StateMachineOptions
{
    public TimeSpan DelayBetweenLoopIteration { get; }

    public StateMachineOptions(TimeSpan delayBetweenLoopIteration)
    {
        DelayBetweenLoopIteration = delayBetweenLoopIteration;
    }
}