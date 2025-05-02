using StateMachine.AsyncEx;

namespace StateMachine.StateMachineBase;

public class StateMachineThread<TEnumState, TContext, TStateMachine> 
    where TContext : IStateContext 
    where TStateMachine : StateMachineDescription<TEnumState, TContext, TStateMachine>
    where TEnumState : struct, Enum
{
    private readonly StateMachineDescription<TEnumState, TContext, TStateMachine> _stateMachineDescription;
    private readonly StateMachineOptions _options;
    private CancellationTokenSource? _cts;

    public StateMachineThread(StateMachineDescription<TEnumState, TContext, TStateMachine> stateMachineDescription, StateMachineOptions options)
    {
        _stateMachineDescription = stateMachineDescription;
        _options = options;
    }
        
    public void Start()
    {
        Console.WriteLine("[Thread] Started");
        _cts = new CancellationTokenSource();
        MainLoop().Forget();
    }

    private async Task MainLoop()
    {
        if (_cts == null || _cts.IsCancellationRequested) return;  
            
        Console.WriteLine("[MainLoop] Start");
        try
        {
            while (!_cts.IsCancellationRequested)
            {
                // todo try cache ?
                if (_stateMachineDescription.TryGetNewState(out var newState))
                {
                    // old state
                    await _stateMachineDescription.ExitCurrentState(_cts.Token);
                    
                    _stateMachineDescription.SwitchState(newState!);

                    // new state
                    await _stateMachineDescription.EnterCurrentState(_cts.Token);
                    await _stateMachineDescription.ExecuteCurrentState(_cts.Token);
                }

                await Task.Delay(_options.DelayBetweenLoopIteration, _cts.Token);
            }
        }
        catch (OperationCanceledException) { }

        Console.WriteLine("[MainLoop] End");
    }

    public void Stop()
    {
        Console.WriteLine("[Thread] Stopped");
        _cts?.Cancel();
        _cts = null;
    }
}