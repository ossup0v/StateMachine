namespace StateMachine.Loggers;

public class ConsoleLogger : ILogger
{
    private readonly string _tag;

    public ConsoleLogger(string tag)
    {
        _tag = tag;
    }
    
    public void LogInformation(string message)
    {
        Console.WriteLine($"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}][{_tag}][INF]:{message}");
    }

    public void LogError(string message)
    {
        Console.WriteLine($"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}][{_tag}][ERR]:{message}");
    }
}