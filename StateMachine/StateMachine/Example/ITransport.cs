namespace StateMachine.Example;

public interface ITransport
{
    Task Send(Request request, CancellationToken ct);
    Task<Response?> Receive(CancellationToken ct);
    
    // for testing purposes
    Task InsertResponseToQueue(Response response, CancellationToken ct);

}