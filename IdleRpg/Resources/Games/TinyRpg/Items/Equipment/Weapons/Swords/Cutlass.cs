using IdleRpg.Game;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyRpg.Core;
using TinyRpg.Items;

namespace TinyRpg.Items.Equipment.Weapons.Swords;
public class Cutlass : IEquipable, IItemTemplate
{
    public Enum Id => ItemIds.WeaponCutlass;
    public string Name => "Cutlass";
    public Enum EquipSlot => EquipSlots.Weapon;
    public string EquipDescription => "Attack + 3";
    public List<StatModifier> EquipEffects => new List<StatModifier>
    {
        new StatModifier
        {
            Stat = Stats.Attack,
            Calculation = (currentStats) => currentStats[Stats.Attack] + 3,
            Description = "Attack + 3"
        }
    };

    public string Description => "A sharp little blade";

    public Image<Rgba32>? InventoryImage { get; set; }

    public string ImageFile => "Cutlass.png";

    public Dictionary<Enum, int> Value => new() { { Stats.Money, 100 } };
}