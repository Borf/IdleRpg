namespace IdleRpg.Game.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class AffectsStatAttribute : Attribute
{
    public required Enum Stat { get; init; }
}
