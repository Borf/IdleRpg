namespace IdleRpg.Game.Core;

public class ItemDrop
{
    public required Enum Item { get; init; }
    public required int DropChance { get; init; }
}
