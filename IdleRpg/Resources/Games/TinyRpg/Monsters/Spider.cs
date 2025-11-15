using IdleRpg.Game.Core;
using System.Collections.Generic;
using System;
using TinyRpg.Items;
using TinyRpg.Npcs;
namespace TinyRpg.Monsters;

public class Spider : Mob
{
    public override Enum Id => NpcIds.Spider;
    public override string Name => "Spider";
    public override string ImageFile => "Spider.png";
    public override Dictionary<Enum, int> Stats => new()
    {
        { Core.Stats.MaxHp, 20 },
        { Core.Stats.Hp, 20 },
        { Core.Stats.Level, 1 },
        { Core.Stats.Attack, 2 },
        { Core.Stats.Defense, 0 },
    };
    public override int AgroRange => 0;

    public override List<ItemDrop> ItemDrops => [
        new ItemDrop() { Item = ItemIds.SpiderLeg, DropChance = 0.125 },
        new ItemDrop() { Item = ItemIds.CobWeb, DropChance = 0.1 }
    ];
    public override int Exp => 1;
}
