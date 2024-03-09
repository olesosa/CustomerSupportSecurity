namespace CS.Security.Servises;

public class ExceptionLoggerProvider : ILoggerProvider
{
    private readonly StreamWriter _logFileWriter;

    public ExceptionLoggerProvider(StreamWriter logFileWriter)
    {
        _logFileWriter = logFileWriter ?? throw new ArgumentNullException(nameof(logFileWriter));
    }
    
    public void Dispose()
    {
        _logFileWriter.Dispose();
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new ExceptionLogger(categoryName, _logFileWriter);
    }
}