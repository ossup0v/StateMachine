namespace StateMachine.Loggers;

public class FileLogger : ILogger
{
    private readonly string _tag;
    private const string PATH = "C:/.temp"; 
    private const string LOG_FILE = "log.txt";
    private readonly string _fullPath = Path.Combine(PATH, LOG_FILE); 

    public FileLogger(string tag)
    {
        _tag = tag;
    }
    
    public void LogInformation(string message)
    {
        TryCreateLogDir();
        var log = $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}][{_tag}][INF]:{message}{Environment.NewLine}";
        File.AppendAllText(_fullPath, log);
    }

    public void LogError(string message)
    {
        TryCreateLogDir();
        var log = $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}][{_tag}][ERR]:{message}{Environment.NewLine}";
        File.AppendAllText(_fullPath, log);
    }

    private void TryCreateLogDir()
    {
        if (!Directory.Exists(PATH)) Directory.CreateDirectory(PATH);
    }
}