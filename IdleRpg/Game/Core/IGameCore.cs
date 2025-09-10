namespace IdleRpg.Game.Core;


public interface IGameCore
{
    Type GetStats();
    StatModifier CalculateInitialStat(Enum s);
    List<Map> LoadMaps();

    Location SpawnLocation { get; }
    void Damage(Character source, Character target, IDamageProperties damageProperties);
    bool IsAlive(Character chacater);
}