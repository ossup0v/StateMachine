namespace StateMachine.Example;

public class Request
{
    public int Id { get; set; }
    public string Name { get; }
    public DateTime SentTime { get; set; }
        
    public Request(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() => $"{Name} #{Id}";
}

public class Response
{
    public int? RequestId { get; set; }
    public string Name { get; }
        
    public Response(int? requestId, string name)
    {
        RequestId = requestId;
        Name = name;
    }

    public override string ToString() => $"{Name}";
}