namespace IdleRpg.Game;

public class LogEntry
{
    public required LogCategory Category { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public required string Message { get; set; } = string.Empty;
}

public enum LogCategory
{
    Character,
    Battle,
    Walking
}