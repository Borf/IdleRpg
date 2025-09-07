using Microsoft.Identity.Client;

namespace IdleRpg.Game;

public class StatModifier
{
    public required Enum Stat { get; init; }
    public List<Enum> StatsUsed { get; init; } = new();
    public required Func<Dictionary<Enum, long>, long> Calculation { get; init; }
}
