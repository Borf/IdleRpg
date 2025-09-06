namespace IdleRpg.Game.Core;

public class Character
{
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public Dictionary<object, ulong> Stats { get; set; } = new();

    public Location? Location { get; set; }
    public Status Status { get; set; } = Status.Idle;

}


public enum Status
{
    Idle,
    Walking,
    Fighting
}