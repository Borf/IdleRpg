using IdleRpg.Game.Core;
using System.Collections.Generic;
using System;
using TinyRpg.Items;
using TinyRpg.Npcs;
namespace TinyRpg.Monsters;

public class Slime : Mob
{
    public override Enum Id => NpcIds.Slime;
    public override string Name => "Slime";
    public override string ImageFile => "Slime.png";
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
        new ItemDrop() { Item = ItemIds.Slime, DropChance = 0.25 },
        new ItemDrop() { Item = ItemIds.WeaponCutlass, DropChance = 0.01 },
        ];

    public override int Exp => 1;
}
