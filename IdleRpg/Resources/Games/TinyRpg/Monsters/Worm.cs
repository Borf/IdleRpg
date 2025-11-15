using IdleRpg.Game.Core;
using System.Collections.Generic;
using System;
using TinyRpg.Items;
using TinyRpg.Npcs;
namespace TinyRpg.Monsters;

public class Worm : Mob
{
    public override Enum Id => NpcIds.Worm;
    public override string Name => "Worm";
    public override string ImageFile => "Worm.png";
    public override Dictionary<Enum, int> Stats => new()
    {
        { Core.Stats.MaxHp, 4 },
        { Core.Stats.Hp, 4 },
        { Core.Stats.Level, 1 },
        { Core.Stats.Attack, 1 },
        { Core.Stats.Defense, 0 },
    };
    public override int AgroRange => 0;

    public override List<ItemDrop> ItemDrops => [
        new ItemDrop() { Item = ItemIds.Slime, DropChance = 0.2 },
        new ItemDrop() { Item = ItemIds.RedPotion, DropChance = 0.1 },
    ];
    public override int Exp => 1;
}
