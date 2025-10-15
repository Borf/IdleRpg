namespace IdleRpg.Game.Core;

public interface ISkill
{
    Enum Id { get; }
    string Name { get; }
    SkillType SkillType { get; }
    bool CanCast(CharacterPlayer character);
    IDamageProperties CalculateDamage(Character src, Character target); //TODO: should target matter here, or should that be used in the ICore.Damage function?
}

public enum SkillType
{
    Passive,
    ActiveSelf,
    Target,
    TargetAoE,
    AoE
}
