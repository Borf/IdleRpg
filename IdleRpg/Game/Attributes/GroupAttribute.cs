namespace IdleRpg.Game.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class GroupAttribute(string name, string? isMaxValueOf = null) : Attribute
{
    public string Name { get; } = name;
    public string? MaxValueOf { get; } = isMaxValueOf;
}
