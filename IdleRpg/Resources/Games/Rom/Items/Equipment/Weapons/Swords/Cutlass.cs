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
    public string Name => "Cutlass";
    public string EquipDescription => "Attack + 10 × Dex";
    public List<StatModifier> EquipEffects => new List<StatModifier>
    {
        new StatModifier
        {
            Stat = Stats.Attack,
            StatsUsed = [ Stats.Dex ],
            Calculation = (currentStats) => 10 * currentStats[Stats.Dex],
        }
    };
}
