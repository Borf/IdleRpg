using IdleRpg.Game.Core;

namespace IdleRpg.Game;

public class Buff
{
    public required List<StatModifier> Modifiers { get; init; } = [];
    public DateTimeOffset StartTime { get; init; } = DateTimeOffset.Now;
    public TimeSpan Duration { get; init; } = TimeSpan.MaxValue;
    public required BuffSource Source { get; init; }
    public required Character AppliedBy { get; init; }
    public IEquipable? AppliedFromEquip { get; init; }
}

public enum BuffSource
{
    Equip,
    Skill
}