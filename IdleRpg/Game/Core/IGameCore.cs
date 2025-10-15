using IdleRpg.Util;

namespace IdleRpg.Game.Core;


public interface IGameCore
{
    Type GetStats();
    StatModifier CalculateInitialStat(Enum s);
    List<Map> LoadMaps();

    public (Point position, string mapName) SpawnLocation { get; }
    void Damage(Character source, Character target, IDamageProperties damageProperties);
    bool IsAlive(Character chacater);
    void GainExp(Character character, INpcTemplate npcTemplate);
}