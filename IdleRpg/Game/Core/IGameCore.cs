using IdleRpg.Util;

namespace IdleRpg.Game.Core;


public interface IGameCore
{
    Type GetStats();
    Type GetItemIdEnum();
    StatModifier CalculateStat(Enum s);
    List<Map> LoadMaps();

    public (Point position, string mapName) SpawnLocation { get; }
    void Damage(Character source, Character target, IDamageProperties damageProperties);
    bool IsAlive(Character chacater);
    void GainExp(Character character, IMonsterTemplate npcTemplate);
    ICharacterCreateCharOptions GetNewCharacterOptions();
    void InitializeCharacter(CharacterPlayer newCharacter, ICharacterCreateCharOptions options);
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
public class CharacterCreateCharOption
{
    public string Name { get; set; } = string.Empty;
    public int Minvalue { get; set; }
    public int MaxValue { get; set; }
    public List<string> Descriptions { get; set; } = new(); //index corresponds to value - MinValue
}
public interface ICharacterCreateCharOptions 
{
    Dictionary<string, CharacterCreateCharOption> Options { get; }
}

public static class CharacterCreateCharOptionsHelper
{
    public static string BuildString(this ICharacterCreateCharOptions options)
    {
        List<string> ret = new();
        foreach(var option in options.Options)
            ret.Add(option.Key + "@" + options.Get(option.Key));
        return string.Join("|", ret);
    }
    public static void FromString(this ICharacterCreateCharOptions options, string text)
    {
        var parts = text.Split('|');
        foreach(var part in parts)
        {
            var subparts = part.Split('@');
            if(subparts.Length != 2)
                continue;
            options.Set(subparts[0], int.Parse(subparts[1]));
        }
    }

    public static void Set(this ICharacterCreateCharOptions options, string key, int newValue)
    {
        options.GetType().GetProperty(key)?.SetValue(options, newValue);
    }

    public static int Get(this ICharacterCreateCharOptions options, string key)
    {
        return ((int?)options.GetType().GetProperty(key)?.GetValue(options))!.Value;
    }
}

public interface IGameCore2D : IGameCore
{
    IImageGenerator<CharacterPlayer, SpriteDirection> MapCharacterGenerator { get; }
    IImageGenerator<ICharacterCreateCharOptions> CharCreateCharacterGenerator { get; }
}
