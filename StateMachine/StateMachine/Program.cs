#nullable enable
using StateMachine.AsyncEx;
using StateMachine.Example;
using StateMachine.Loggers;
using StateMachine.StateMachineBase;

namespace StateMachine;

class Program
{
    static async Task Main()
    {
        var network = new NetworkStateMachineDescription();

        using var cnx = new NetworkContext(new WebSocketTransport(), new ConsoleLogger("Network"));
        
        network.SetContext(cnx);

        network.AddState(NetworkState.Stopped, new StoppedState())
            .To(context => context.StartConnect, NetworkState.Connecting)
            .To(context => context.GotError, NetworkState.GotError);

        network.AddState(NetworkState.Connecting, new ConnectingState())
            .To(context => context.Connected, NetworkState.Ready)
            .To(context => context.GotError, NetworkState.GotError);

        network.AddState(NetworkState.Ready, new ReadyState())
            .To(context => !context.Connected, NetworkState.Stopping)
            .To(context => context.GotError, NetworkState.GotError);

        network.AddState(NetworkState.Stopping, new StoppingState())
            .To(context => !context.Connected && !context.StartConnect, NetworkState.Stopped)
            .To(context => context.GotError, NetworkState.GotError);

        network.AddState(NetworkState.GotError, new GetErrorState())
            .To(context => !context.GotError, NetworkState.Stopped);

        var options = new StateMachineOptions(TimeSpan.FromMilliseconds(500));
        var networkThread = new StateMachineThread<NetworkState, NetworkContext, NetworkStateMachineDescription>(network, options);
        
        networkThread.Start();
        SendRequestsThread(cnx).Forget();
        AddResponsesThread(cnx).Forget();
        Console.WriteLine("Press any key Stop Thread...");
        Console.ReadLine();
        networkThread.Stop();
        Console.WriteLine("Press any key to exit...");
        Console.ReadLine();
    }

    private static volatile int _requestIndex = 1; 
    private static volatile int _responseIndex = 1; 
    
    private static Task SendRequestsThread(NetworkContext cnx)
    {
        return Task.Run(async () =>
        {
            while (_requestIndex < 20)
            {
                await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(1, 3)));
                await cnx.AddRequest(new Request($"request #{_requestIndex}"));
                Interlocked.Increment(ref _requestIndex);
            }
        });
    }
    
    private static Task AddResponsesThread(NetworkContext cnx)
    {
        return Task.Run(async () =>
        {
            while (_responseIndex < 20)
            {
                await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(1, 3)));
                await cnx.Transport.InsertResponseToQueue(new Response($"Response #{_responseIndex}"),
                    CancellationToken.None);
                Interlocked.Increment(ref _responseIndex);
            }
        });
    }
    
}