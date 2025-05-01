namespace StateMachine.StateMachineBase;

public abstract class StateMachineDescription<TEnumState, TContext, TStateMachine> 
    where TContext : IStateContext 
    where TStateMachine : StateMachineDescription<TEnumState, TContext, TStateMachine>
    where TEnumState : struct, Enum 
        
{
    private readonly StateSwitcher<TEnumState, TContext> _switcher = new (); 
    private readonly Dictionary<TEnumState, IState<TContext>> _states = new ();
    private IState<TContext>? _currentState;
    private TContext? _context;

    public void SetContext(TContext context)
    {
        if (_context != null) throw new InvalidOperationException("Trying to set context more than once.");
        _context = context;
    }
        
    public StateSwitcher<TEnumState, TContext> AddState<TState>(TEnumState enumState, TState state) where TState : IState<TContext>
    {
        _ = _context ?? throw new InvalidOperationException($"Trying to add state {enumState}. {state.GetType().Name} when context is null.");

        // first registered state is current state by default
        _currentState ??= state;
        
        // current state for switcher it state for make description. not for execute states
        _switcher.SetCurrentState(enumState);

        _states.Add(enumState, state);
        return _switcher;
    }

    public void TrySwitchState()
    {
        _ = _context ?? throw new InvalidOperationException("Trying to switch state when context is null.");

        if (_switcher.TrySwitchState(_context, out var newState))
        {
            if (!_states.TryGetValue(newState, out var newCurrentState))
                throw new InvalidOperationException(
                    $"Trying to switch to state {newState}, but have no state like this in states. All states:[{string.Join(", ", _states.Keys)}]");
                
            _currentState = newCurrentState;
        }
    }

    public async Task ExecuteCurrentState(CancellationToken ct)
    {
        _ = _context ?? throw new InvalidOperationException("Trying to execute current state when context is null.");
        _ = _currentState ?? throw new InvalidOperationException("Trying to execute current state when currentState is null.");

        await _currentState.Run(_context, ct);
    }
}