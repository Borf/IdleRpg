using IdleRpg.Game;
using IdleRpg.Game.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rom.Items.Equipment.Weapons.Swords;
public class BasicAttack : ISkill
{
    public int Id => 10001;
    public string Name => "Basic Attack";
    public SkillType SkillType => SkillType.Target;

    public bool CanCast(CharacterPlayer character)
    {
        return true;
    }



}
