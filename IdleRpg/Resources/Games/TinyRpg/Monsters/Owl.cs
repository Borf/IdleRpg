using IdleRpg.Game.Core;
using System.Collections.Generic;
using System;
using TinyRpg.Items;
using TinyRpg.Npcs;
namespace TinyRpg.Monsters;

public class Owl : Mob
{
    public override Enum Id => NpcIds.Owl;
    public override string Name => "Owl";
    public override string ImageFile => "Owl.png";
    public override Dictionary<Enum, int> Stats => new()
    {
        { Core.Stats.MaxHp, 7 },
        { Core.Stats.Hp, 7 },
        { Core.Stats.Level, 1 },
        { Core.Stats.Attack, 1 },
        { Core.Stats.Defense, 0 },
    };
    public override int AgroRange => 0;

    public override List<ItemDrop> ItemDrops => [
        new ItemDrop() { Item = ItemIds.OwlFeather, DropChance = 0.1 },
        new ItemDrop() { Item = ItemIds.ArmorShirt, DropChance = 0.05 },
    ];

    public override int Exp => 1;
}
