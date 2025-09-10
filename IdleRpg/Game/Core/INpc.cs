namespace IdleRpg.Game.Core;

public interface INpc
{
    Enum Id { get; }
    string Name { get; }
    Dictionary<Enum, int> Stats { get; }
    List<ItemDrop> ItemDrops { get; }
}
