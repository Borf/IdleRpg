namespace Rom.Core;

using IdleRpg.Game.Attributes;
using System;


public class GameCore : IdleRpg.Game.Core.IGameCore
{
    public Type GetStats() => typeof(Stats);
}


public enum Stats
{
    [StatAttribute]
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
}
