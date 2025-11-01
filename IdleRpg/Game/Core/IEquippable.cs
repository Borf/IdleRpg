namespace IdleRpg.Game.Core;

public interface IEquippable : IItemTemplate
{
    public string EquipDescription { get; }
    public List<StatModifier> EquipEffects { get; }
    public Enum EquipSlot { get; } //type unsafe but okay
}
