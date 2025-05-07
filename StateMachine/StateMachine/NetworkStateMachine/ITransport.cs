using Shared.Contracts;

namespace StateMachine.NetworkStateMachine;

public interface ITransport
{
    Task Connect(string connectionString, CancellationToken ct);
    Task Disconnect(CancellationToken ct);
    
    event Func<Response, Task> OnResponseReceived;
    
    Task Send(Request request, CancellationToken ct);
    Task<Response> SendAndWait(Request request, CancellationToken ct);
}