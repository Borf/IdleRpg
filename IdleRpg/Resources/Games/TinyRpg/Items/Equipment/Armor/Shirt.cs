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

namespace TinyRpg.Items.Equipment.Armor;
public class Shirt : IEquipable, IItemTemplate
{
    public Enum Id => ItemIds.ArmorShirt;
    public string Name => "Shirt";
    public Enum EquipSlot => EquipSlots.Armor;
    public string EquipDescription => "Def + 3";
    public List<StatModifier> EquipEffects => new List<StatModifier>
    {
        new StatModifier
        {
            Stat = Stats.Attack,
            Calculation = (currentStats) => currentStats[Stats.Defense] + 3,
            Description = "Def + 3"
        },
        new StatModifier
        {
            Stat = Stats.LookShirt,
            Calculation = (currentStats) => currentStats[Stats.LookShirt] = 1
        }
    };

    public string Description => "A tattered shirt. It's a bit smelly";

    public Image<Rgba32>? InventoryImage { get; set; }

    public string ImageFile => "Shirt.png";

    public Dictionary<Enum, int> Value => new() { { Stats.Money, 25 } };
}