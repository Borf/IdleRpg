using IdleRpg.Game;
using IdleRpg.Game.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyRpg.Core;
using TinyRpg.Skills;

namespace Rom.Items.Equipment.Weapons.Swords;
public class BasicAttack : ISkill
{
    public Enum Id => Skills.BasicAttack;
    public string Name => "Basic Attack";
    public SkillType SkillType => SkillType.Target;

    public IDamageProperties CalculateDamage(Character src, Character target)
    {
        return new DamageProperties()
        {
            Damage = (int)Math.Max(1, src.Stats[TinyRpg.Core.Stats.Attack] - target.Stats[TinyRpg.Core.Stats.Defense])
        };
    }

    public bool CanCast(CharacterPlayer character)
    {
        return true;
    }



}
