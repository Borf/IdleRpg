namespace IdleRpg.Game.Core;

public interface ISkill
{
    int Id { get; }
    string Name { get; }
    SkillType SkillType { get; }
    bool CanCast(CharacterPlayer character);
}

public enum SkillType
{
    Passive,
    ActiveSelf,
    Target,
    TargetAoE,
    AoE
}
