using System.ComponentModel.DataAnnotations;

namespace IdleRpg.Data.Db;

public class Character
{
    [Key]
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Map { get; set; } = string.Empty;
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public List<CharacterStat> Stats { get; set; } = [];
    public List<InventoryItem> Inventory { get; set; } = [];
}
