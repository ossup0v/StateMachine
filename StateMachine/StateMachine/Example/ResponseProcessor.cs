namespace StateMachine.Example;

public class ResponseProcessor : IResponseProcessor
{
    public Task Process(Response response)
    {
        Console.WriteLine($"{response} processed");
        return Task.CompletedTask;
    }
}