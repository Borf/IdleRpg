namespace IdleRpg.Util;

public class MemoryLogger : ILogger
{
    private readonly string _categoryName;
    private readonly InMemoryLogStore _store;

    public MemoryLogger(string categoryName, InMemoryLogStore store)
    {
        _categoryName = categoryName;
        _store = store;
    }

    public IDisposable BeginScope<TState>(TState state) => null;
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        var msg = formatter(state, exception);
        var line = $"{DateTime.Now:HH:mm:ss} [{logLevel}] {_categoryName}: {msg}";

        // Write to console
        //Console.WriteLine(line);

        // Store in memory
        _store.Add(line);
    }
}