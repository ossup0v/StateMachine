namespace StateMachine.Example;

public interface IResponseProcessor
{
    Task Process(Response response);
}