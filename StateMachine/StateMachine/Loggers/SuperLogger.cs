namespace StateMachine.Loggers;

public class SuperLogger : ILogger
{
    private readonly List<ILogger> _loggers;

    public SuperLogger(List<ILogger> loggers)
    {
        _loggers = loggers;
    }
    
    public void LogInformation(string message)
    {
        foreach (var logger in _loggers)
        {
            logger.LogInformation(message);
        }
    }

    public void LogError(string message)
    {
        foreach (var logger in _loggers)
        {
            logger.LogError(message);
        }
    }
}