using IdleRpg.Game.Core;
using System.Collections.Generic;
using System;
using TinyRpg.Items;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
namespace TinyRpg.Npcs.Mobs;

public class Slime : Mob
{
    public override Enum Id => NpcIds.Slime;
    public override string Name => "Slime";
    public override string ImageFile => "slime.png";
    public override Dictionary<Enum, int> Stats => new()
    {
        { Core.Stats.MaxHp, 4 },
        { Core.Stats.Hp, 4 },
        { Core.Stats.Level, 1 },
        { Core.Stats.Attack, 1 },
        { Core.Stats.Defense, 0 },
    };
    public override int AgroRange => 0;

    public override List<ItemDrop> ItemDrops => [ new ItemDrop() { Item = ItemIds.RedPotion, DropChance = 1 }];

    public override int Exp => 1;
}
