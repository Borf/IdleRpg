using IdleRpg.Game;
using IdleRpg.Game.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyRpg.Core;
using TinyRpg.Items;

namespace Rom.Items.Equipment.Weapons.Swords;
public class Cutlass : IEquippable
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
        }
    };

}
