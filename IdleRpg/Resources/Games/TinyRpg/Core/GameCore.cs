namespace TinyRpg.Core;

using IdleRpg.Game;
using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using IdleRpg.Util;
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
            Stats.MaxHp =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => 5 * currentStats[Stats.Level],
                },
            Stats.MaxSp =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => 3 * currentStats[Stats.Level],
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
    public List<Map> LoadMaps()
    { 
        return new List<Map>()
        {
            new WorldMap()
        };
    }


    public (Point position, string mapName) SpawnLocation => (new Point(18, 21), nameof(WorldMap));

    public void Damage(Character source, Character target, IDamageProperties damageProperties)
    {
        var properties = damageProperties as DamageProperties ?? throw new Exception();
        target.Stats[Stats.Hp] = Math.Max(0, target.Stats[Stats.Hp] - properties.Damage);
    }
    public bool IsAlive(Character character) => character.Stats[Stats.Hp] > 0;

}




// How do we determine death condition?
// How do we handle multiple exp systems?
public enum Stats
{
    [NotCalculated, Group("Core")]
    Level,
    [NotCalculated, Group("Core")]
    Exp,

    [Group("Base")]
    Attack,
    [Group("Base")]
    Defense,
    [Group("Base")]
    Dodge,
    [Group("Base")]
    Accuracy,
    
    [NotCalculated, Group("Core")]
    Hp,
    [Group("Core", nameof(Hp))]
    MaxHp,
    [NotCalculated, Group("Core")]
    Sp,
    [Group("Core", nameof(Sp))]
    MaxSp,
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


public class DamageProperties : IDamageProperties
{
    public int Damage { get; set; }
}