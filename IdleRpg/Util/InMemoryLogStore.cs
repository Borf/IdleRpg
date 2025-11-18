namespace IdleRpg.Util;

public class InMemoryLogStore
{
    private readonly int _capacity;
    private readonly Queue<string> _buffer = new();

    public InMemoryLogStore(int capacity = 50)
    {
        _capacity = capacity;
    }

    public void Add(string message)
    {
        _buffer.Enqueue(message);
        if (_buffer.Count > _capacity)
            _buffer.Dequeue();   // remove oldest
    }

    public IReadOnlyList<string> GetLogs()
        => _buffer.ToList();
}