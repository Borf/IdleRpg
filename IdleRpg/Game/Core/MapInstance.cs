namespace IdleRpg.Game.Core;

public class MapInstance
{
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public required Map Map { get; set; }
    public List<Character> Characters { get; set; } = new();

    public IEnumerable<CharacterNPC> Npcs => Characters.Where(c => c is CharacterNPC).Select(c => (CharacterNPC)c);
    public IEnumerable<CharacterPlayer> Players => Characters.Where(c => c is CharacterPlayer).Select(c => (CharacterPlayer)c);

    List<BgTask> bgTasks = new();

    public void LoadNpcs(IGameCore gameCore, IServiceProvider serviceProvider)
    {
        var bgTaskManager = serviceProvider.GetRequiredService<BgTaskManager>();
        var coreHolder = serviceProvider.GetRequiredService<ICoreHolder>();
        var logger = serviceProvider.GetRequiredService<ILogger<MapInstance>>();
        foreach (var spawn in Map.Spawns)
        {
            var spawner = new Spawner()
            {
                SpawnTemplate = spawn,
                LastSpawnTime = DateTimeOffset.Now
            };
            spawner.Task = new BgTask($"Spawner {spawn.Mob} on {Map.Name}", async (token) =>
            {
                while (!token.IsCancellationRequested)
                {
                    spawner.SpawnedNpcs.RemoveAll(npc => !gameCore.IsAlive(npc));
                    if (spawner.SpawnedNpcs.Count < spawn.Amount)
                    {
                        //TODO: bound check!
                        var location = new Location(spawner.SpawnTemplate.Position.X, spawner.SpawnTemplate.Position.Y) { MapInstance = this };
                        var otherChars = GetCharactersAround(location, spawner.SpawnTemplate.Range);
                        while(location.X < 0 || location.X >= location.MapInstance.Map.Width ||
                              location.Y < 0 || location.Y >= location.MapInstance.Map.Height || 
                              otherChars.Any(c => c.Location.X == location.X && c.Location.Y == location.Y) || 
                              Map[location.X, location.Y].HasFlag(CellType.NotWalkable))
                            location = new Location(spawner.SpawnTemplate.Position.X + Random.Shared.Next(-spawner.SpawnTemplate.Range, spawner.SpawnTemplate.Range), spawner.SpawnTemplate.Position.Y + Random.Shared.Next(-spawner.SpawnTemplate.Range, spawner.SpawnTemplate.Range)) { MapInstance = this };
                        var npc = new CharacterNPC(serviceProvider, coreHolder.NpcTemplates[spawner.SpawnTemplate.Mob])
                        {
                            Spawner = spawner,
                            Location = location,
                            Id = (ulong)Random.Shared.NextInt64(), //TODO: check for clashes
                        };
                        logger.LogInformation($"Spawning {npc.Name} at {npc.Location.MapInstance.Map.Name}:{npc.Location.X},{npc.Location.Y}");
                        spawner.SpawnedNpcs.Add(npc);
                        this.SpawnCharacter(npc);
                        npc.Start(serviceProvider);
                    }
                    await Task.Delay(spawn.RespawnTime, token);
                }
            });

            bgTasks.Add(spawner.Task);
            bgTaskManager.Run(spawner.Task);
        }
    }

    private void SpawnCharacter(Character character)
    {
        this.Characters.Add(character);
        //TODO: trigger listeners?
    }

    public List<Character> GetCharactersAround(Location location, int range)
    {
        return Characters
            .ToList()
            .Where(c => c != null && Math.Abs(location.X - c.Location.X) < range && Math.Abs(location.Y - c.Location.Y) < range)
            .ToList();
    }
}

public class Spawner
{
    public required SpawnTemplate SpawnTemplate { get; set; }
    public DateTimeOffset LastSpawnTime { get; set; } = DateTimeOffset.MinValue;
    public BgTask Task { get; set; } = null!;
    public List<CharacterNPC> SpawnedNpcs = new();
}