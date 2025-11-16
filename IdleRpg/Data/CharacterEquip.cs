using IdleRpg.Data.Db;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdleRpg.Data;

[PrimaryKey(nameof(CharacterId), nameof(ItemId))]
public class CharacterEquip
{
    public Character Character { get; set; } = null!;
    [ForeignKey(nameof(Character))]
    public ulong CharacterId { get; set; }
    public InventoryItem Item { get; set; } = null!;
    [ForeignKey(nameof(Item))]
    public Guid ItemId { get; set; }
    public string EquipSlot { get; set; } = string.Empty;//TODO: maybe optional?
}
