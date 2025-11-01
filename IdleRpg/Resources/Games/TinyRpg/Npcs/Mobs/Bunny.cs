using IdleRpg.Game.Core;
using System.Collections.Generic;
using System;
using TinyRpg.Items;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
namespace TinyRpg.Npcs.Mobs;

public class Bunny : Mob
{
    public override Enum Id => NpcIds.Bunny;
    public override string Name => "Bunny";
    public override string ImageFile => "Bunny.png";
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
        new ItemDrop() { Item = ItemIds.BunnyTail, DropChance = 0.2 },
        new ItemDrop() { Item = ItemIds.BunnyEar, DropChance = 0.1 },
    ];

    public override int Exp => 1;
}
