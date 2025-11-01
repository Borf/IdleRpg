using System.Diagnostics.CodeAnalysis;

namespace IdleRpg.Game.Core;

public class InventoryItem
{
    public required Enum ItemId { get; set; }
    public required Guid Guid { get; set; }
    [SetsRequiredMembers]
    public InventoryItem(Enum itemId, Guid guid)
    {
        ItemId = itemId;
        Guid = guid;
    }
}
