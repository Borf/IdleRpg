using System.ComponentModel.DataAnnotations;

namespace IdleRpg.Data.Db;

public class Character
{
    [Key]
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<CharacterStat> Stats { get; set; } = new();
}
