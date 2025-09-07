using System.ComponentModel.DataAnnotations;

namespace IdleRpg.Data.Db;

public class CharacterStat
{
    [Key]
    public int Id { get; set; }
    public required Character Character { get; set; }
    public string Stat { get; set; } = string.Empty;
    public long Value { get; set; }
}
