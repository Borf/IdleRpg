namespace IdleRpg.Game.Core;

public interface IEquipable
{
    public string EquipDescription { get; }
    public List<StatModifier> EquipEffects { get; }
    public Enum EquipSlot { get; } //type unsafe but okay}
}