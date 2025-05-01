namespace StateMachine.StateMachineBase;

public class StateSwitcher<TEnumState, TContext> 
    where TEnumState : struct, Enum 
    where TContext : IStateContext
{
    private readonly Dictionary<TEnumState, Dictionary<TEnumState, Func<TContext, bool>>> _transactionConditions = new();
    private TEnumState _currentState;
        
    public StateSwitcher<TEnumState, TContext> To(Func<TContext, bool> switchCondition, TEnumState destinationState)
    {
        if (!_transactionConditions.TryGetValue(_currentState, out var currentStateTransactionConditions))
        {
            currentStateTransactionConditions = new();
            _transactionConditions.Add(_currentState, currentStateTransactionConditions);
        }

        if (!currentStateTransactionConditions.TryAdd(destinationState, switchCondition))
            throw new InvalidOperationException(
                $"Trying to add state transaction twice. from {_currentState} to {destinationState}");

        return this;
    }

    public void SetCurrentState(TEnumState enumState)
    {
        _currentState = enumState;
    }

    public bool TrySwitchState(TContext context, out TEnumState newState)
    {
        if (!_transactionConditions.TryGetValue(_currentState, out var currentStateTransactions))
            throw new InvalidOperationException($"Can't find transactions by current state {_currentState}");

        foreach (var (destinationState, transaction) in currentStateTransactions)
        {
            if (transaction(context))
            {
                context.Logger.LogInformation($"State:[{_currentState}=>{destinationState}]");
                _currentState = destinationState;
                newState = destinationState;
                return true;
            }
        }
            
        newState = default;
        return false;
    }
}