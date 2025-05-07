#nullable enable
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Shared.Contracts;
using StateMachine.AsyncEx;
using StateMachine.Loggers;
using StateMachine.NetworkStateMachine;
using StateMachine.StateMachineBase;

namespace StateMachine;

class Program
{
    static async Task Main()
    {

        var network = new NetworkStateMachineDescription();

        using var cnx = new NetworkContext(new WebSocketTransport(), new ConsoleLogger("Network"));

        network.SetContext(cnx);

        network.AddState(NetworkState.Connecting, new ConnectingState())
            .To(context => context.GotError, NetworkState.GotError)
            .To(context => context.Connected, NetworkState.Ready);

        network.AddState(NetworkState.Ready, new ReadyState())
            .To(context => context.GotError, NetworkState.GotError)
            .To(context => !context.Connected, NetworkState.Stopping);

        network.AddState(NetworkState.Stopping, new StoppingState())
            .To(context => context.GotError, NetworkState.GotError)
            .To(context => !context.Connected, NetworkState.Stopped);

        network.AddState(NetworkState.GotError, new GotErrorState())
            .To(context => !context.GotError, NetworkState.Stopping);

        network.AddState(NetworkState.Stopped, new StoppedState())
            .To(context => context.GotError, NetworkState.GotError)
            .To(context => !context.Connected, NetworkState.Connecting);

        var options = new StateMachineOptions(TimeSpan.FromMilliseconds(500));
        var networkThread = new StateMachineThread<NetworkState, NetworkContext, NetworkStateMachineDescription>(network, options);

        networkThread.Start();
        SendRequestsThread(cnx).Forget();
        HealthCheckThread(cnx).Forget();
        Console.WriteLine("Press any key Stop Thread...");
        Console.ReadLine();
        networkThread.Stop();
        Console.WriteLine("Press any key to exit...");
        Console.ReadLine();
    }

    private static volatile int _requestIndex = 1; 
    private static volatile int _responseIndex = 1;
    private static readonly ConcurrentBag<int> SentRequestIds = new();
    
    // client
    private static Task SendRequestsThread(NetworkContext cnx)
    {
        return Task.Run(async () =>
        {
            while (_requestIndex < 20)
            {
                await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(1, 3)));
                var request = new Request(_requestIndex, "Test");
                await cnx.AddRequest(request);
                SentRequestIds.Add(request.Id);
                Interlocked.Increment(ref _requestIndex);
            }
        });
    }
    
    private static Task HealthCheckThread(NetworkContext cnx)
    {
        return Task.Run(async () =>
        {
            while (true)
            {
                foreach (var (id, request) in cnx.PendingRequests.ToArray())
                {
                    var passedTime = DateTime.UtcNow - request.SentTime;
                    if (passedTime > TimeSpan.FromSeconds(1))
                    {
                        cnx.Logger.LogError($"Get timeout for request {request} with id {id}. Set timeout exception.");
                        cnx.SetTimeoutException(request);
                    }
                }

                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }
        });
    }
    // client
    
}