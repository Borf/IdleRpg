namespace IdleRpg.Util;

public class MemoryLoggerProvider : ILoggerProvider
{
    private readonly InMemoryLogStore _store;

    public MemoryLoggerProvider(InMemoryLogStore store)
    {
        _store = store;
    }

    public ILogger CreateLogger(string categoryName)
        => new MemoryLogger(categoryName, _store);

    public void Dispose() { }
}