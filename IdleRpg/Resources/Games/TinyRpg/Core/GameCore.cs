namespace Rom.Core;

using IdleRpg.Game;
using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using System;
using System.Collections.Generic;

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
}


public enum Stats
{
    [NotCalculated]
    Level,

    Attack,
    Defense,
    MagicAttack,
    MagicDefense,
    Dodge,
    Accuracy,
    
    [NotCalculated]
    Hp,
    MaxHp,

}
