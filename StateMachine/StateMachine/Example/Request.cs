namespace StateMachine.Example;

public class Request
{
    public string Name { get; }
        
    public Request(string name)
    {
        Name = name;
    }

    public override string ToString() => $"{Name}";
}

public class Response
{
    public string Name { get; }
        
    public Response(string name)
    {
        Name = name;
    }

    public override string ToString() => $"{Name}";
}