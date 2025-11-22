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

namespace TinyRpg.Items.Equipment.Pants;
public class Pants : IEquipable, IItemTemplate
{
    public Enum Id => ItemIds.ArmorShirt;
    public string Name => "Pants";
    public Enum EquipSlot => EquipSlots.Pants;
    public string EquipDescription => "Def + 2";
    public List<StatModifier> EquipEffects => new List<StatModifier>
    {
        new StatModifier
        {
            Stat = Stats.Attack,
            Calculation = (currentStats) => currentStats[Stats.Defense] + 2,
            Description = "Def + 2"
        },
        new StatModifier
        {
            Stat = Stats.LookPants,
            Calculation = (currentStats) => currentStats[Stats.LookPants] = 1
        }
    };

    public string Description => "Some pants you found. There's some holes in them";

    public Image<Rgba32>? InventoryImage { get; set; }

    public string ImageFile => "Pants.png";

    public Dictionary<Enum, int> Value => new() { { Stats.Money, 30 } };
}