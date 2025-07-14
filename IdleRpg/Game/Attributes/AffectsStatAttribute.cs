namespace IdleRpg.Game.Attributes;

public class AffectsStatAttribute : Attribute
{
    public required Enum Stat { get; init; }
}
