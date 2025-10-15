using IdleRpg.Game.Core;

namespace IdleRpg.Game;

public class CharacterNPC : Character
{
//    public ICharacterState<CharacterNPC> State { get; set; }
    public INpcTemplate NpcTemplate { get; set; }
    BgTask AiTask;
    public Spawner? Spawner { get; set; }
    
    public CharacterNPC(IServiceProvider serviceProvider, INpcTemplate npcTemplate) : base(serviceProvider)
    {
        NpcTemplate = npcTemplate;
        Name = npcTemplate.Name;
        foreach (var stat in npcTemplate.Stats)
        {
            Stats[stat.Key] = stat.Value;
        }
        AiTask = new BgTask("AiTask " + Name, async (token) => await AiRunner(token, serviceProvider));
    }

    public void Start(IServiceProvider serviceProvider)
    {
        var bgTaskManager = serviceProvider.GetRequiredService<BgTaskManager>();
        bgTaskManager.Run(AiTask);
    }
    public async Task Die()
    {
        await AiTask.Cancel();
    }


    List<List<int>> offsets = [[0, -1], [0, 1], [-1, 0], [1, 0]];
    private async Task AiRunner(CancellationToken token, IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<CharacterNPC>>();
        while (!token.IsCancellationRequested)
        {
            var newPos = offsets[Random.Shared.Next(offsets.Count)];
            var newLocation = new Location(Location.X + newPos[0], Location.Y + newPos[1]) { MapInstance = Location.MapInstance };
            var otherChars = Location.MapInstance.GetCharactersAround(Location, 5);
            if (newLocation.X >= 0 && newLocation.X < Location.MapInstance.Map.Width &&
                newLocation.Y >= 0 && newLocation.Y < Location.MapInstance.Map.Height &&
                !otherChars.Any(c => c.Location.X == newLocation.X && c.Location.Y == newLocation.Y) && 
                Location.MapInstance.Map[newLocation.X, newLocation.Y].HasFlag(CellType.Walkable) && 
                (Spawner != null && Math.Abs(newLocation.X - Spawner.SpawnTemplate.Position.X) < Spawner.SpawnTemplate.Range && Math.Abs(newLocation.Y - Spawner.SpawnTemplate.Position.Y) < Spawner.SpawnTemplate.Range)
                )
            {
                Location = newLocation;
            }
            await Task.Delay(2000, token);
        }
    }

}
