namespace Rom.Core;
using System;


public class GameCore : IdleRpg.Game.Core.IGameCore
{
    public Type GetStats() => typeof(Stats);
}


public enum Stats
{
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
