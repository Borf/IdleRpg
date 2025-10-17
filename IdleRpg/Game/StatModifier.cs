using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Identity.Client;
using System.Diagnostics.CodeAnalysis;

namespace IdleRpg.Game;

public class StatModifier
{
    public required Enum Stat { get; init; }
    public List<Enum> StatsUsed { get; init; } = new();
    public required Func<Dictionary<Enum, long>, long> Calculation { get; init; }
}

public class FixedStatModifier : StatModifier
{
    [SetsRequiredMembers]
    public FixedStatModifier(Enum stat, int value)
    {
        Stat = stat;
        Calculation = _ => value;
    }
}
