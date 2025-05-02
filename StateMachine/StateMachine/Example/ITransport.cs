namespace StateMachine.Example;

public interface ITransport
{
    event Func<Response, Task> OnResponseReceived;
    Task Send(Request request, CancellationToken ct);
    
    // for testing purposes
    Task InsertResponseToQueue(Response response, CancellationToken ct);

}