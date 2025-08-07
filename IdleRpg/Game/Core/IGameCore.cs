namespace IdleRpg.Game.Core;


public interface IGameCore
{
    Type GetStats();
    StatModifier CalculateInitialStat(Enum s);
    List<Map> LoadMaps();

}