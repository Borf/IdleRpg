
namespace IdleRpg.Game.Core;

public class Character(IServiceProvider serviceProvider)
{
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public Dictionary<Enum, long> Stats { get; set; } = new();

    public Location Location { get; set; } = null!;
    public Status Status { get; set; } = Status.Idle;
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public void CalculateStats()
    {
        var gameService = ServiceProvider.GetRequiredService<GameService>();
        foreach (var modifier in gameService.sortedModifiers)
        {
            if (gameService.NotCalculatedStats.Contains(modifier.Stat))
                continue;
            Stats[modifier.Stat] = modifier.Calculation(Stats);
        }
    }
}


public enum Status
{
    Idle,
    Walking,
    Fighting
}