namespace IdleRpg.Game.Core;

public interface IEquippable : IItem
{
    public string EquipDescription { get; }
    public List<StatModifier> EquipEffects { get; }
}
