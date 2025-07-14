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
        return null!;
    }
}


public enum Stats
{
    Level,
    Str,
    Dex,
    Int,
    Vit,
    Luk,
    Agi,
    Hp,
    MaxHp,
    Sp,
    MaxSp,

    Attack,
    AttackPerc,
}
