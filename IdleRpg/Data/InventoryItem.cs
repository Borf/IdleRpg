using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdleRpg.Data.Db;

public class InventoryItem
{
    [Key]
    public required Guid Id { get; set; }
    public required Character Character { get; set; }
    [ForeignKey(nameof(Character))]
    public ulong CharacterId { get; set; }
    public required int ItemId { get; set; }
    //TODO: extra item properties, but need to be saved dynamically as the item properties are dynamic
}
