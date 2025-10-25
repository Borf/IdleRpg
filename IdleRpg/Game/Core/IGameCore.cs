using IdleRpg.Util;

namespace IdleRpg.Game.Core;


public interface IGameCore
{
    Type GetStats();
    StatModifier CalculateStat(Enum s);
    List<Map> LoadMaps();

    public (Point position, string mapName) SpawnLocation { get; }
    void Damage(Character source, Character target, IDamageProperties damageProperties);
    bool IsAlive(Character chacater);
    void GainExp(Character character, INpcTemplate npcTemplate);
}


public enum DiscordMenu
{
    Main
}
public interface IDiscordGame
{
    IImageGenerator<DiscordMenu, Character> HeaderGenerator { get; }

}


public enum SpriteDirection
{
    Down,
    Left,
    Right,
    Up
}

public interface IGameCore2D : IGameCore
{
    IImageGenerator<Character, SpriteDirection> MapCharacterGenerator { get; }
}
