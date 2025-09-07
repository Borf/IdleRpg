namespace Rom.Core;

using IdleRpg.Game;
using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using System;
using System.Collections.Generic;
using TinyRpg.Maps;

public class GameCore : IGameCore
{
    public Type GetStats() => typeof(Stats);

    public StatModifier CalculateInitialStat(Enum s)
    {
        Stats stat = (Stats)s;
        return stat switch
        {
            Stats.Attack =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => currentStats[Stats.Level],
                },
            Stats.Defense =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => currentStats[Stats.Level],
                },
            Stats.MagicAttack =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => currentStats[Stats.Level],
                },
            Stats.MagicDefense =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => currentStats[Stats.Level],
                },
            Stats.MaxHp =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => 5 * currentStats[Stats.Level],
                },
            Stats.Dodge =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => 5 * currentStats[Stats.Level],
                },
            Stats.Accuracy =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => 5 * currentStats[Stats.Level],
                },
            _ => throw new Exception("Requested stat that is not implemented: " + stat.ToString()),
        };
    }
    //should this be a dictionary or a list indexed by an enum?
    private List<Map> Maps = new List<Map>();

    public List<Map> LoadMaps()
    {
        Maps = new List<Map>()
        {
            new WorldMap()
        };

        return Maps;
    }


    public Location SpawnLocation => new Location(10, 10) { MapInstance = Maps[0].MapInstance() };
}


public enum Stats
{
    [NotCalculated, Group("Core")]
    Level,

    [Group("Base")]
    Attack,
    [Group("Base")]
    Defense,
    [Group("Base")]
    MagicAttack,
    [Group("Base")]
    MagicDefense,
    [Group("Base")]
    Dodge,
    [Group("Base")]
    Accuracy,
    
    [NotCalculated, Group("Core")]
    Hp,
    [Group("Core", nameof(Hp))]
    MaxHp,

}


public enum Jobs
{
    Warrior,
    Mage,
    Archer,
    Healer,
}

[Flags]
public enum EquipSlots
{
    Weapon = 1<<0,
    Armor = 1<<1,
    Helm = 1<<2,
}