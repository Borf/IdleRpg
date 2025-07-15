using IdleRpg.Game;
using IdleRpg.Game.Core;
using Rom.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rom.Items.Equipment.Weapons.Swords;
public class Cutlass : IEquippable
{
    public int Id => 10001;
    public string Name => "Cutlass";
    public string EquipDescription => "Attack + 3";
    public List<StatModifier> EquipEffects => new List<StatModifier>
    {
        new StatModifier
        {
            Stat = Stats.Attack,
            Calculation = (currentStats) => 3,
        }
    };

}
