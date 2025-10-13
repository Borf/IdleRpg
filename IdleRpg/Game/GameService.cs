using IdleRpg.Data;
using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using IdleRpg.Game.PlayerState;
using IdleRpg.Util;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
namespace IdleRpg.Game;

public class GameService : ICoreHolder
{
    public const string CoreName = "TinyRpg";
    private readonly ILogger<GameService> _logger;
    private readonly BgTaskManager BgTaskManager;
    private readonly IServiceProvider serviceProvider;
    private CoreLoader CoreLoader;
    public Dictionary<Enum, INpc> NpcTemplates { get; set; } = new();
    public List<IItem> ItemTemplates { get; set; } = new();

    private List<Map> Maps = new();
    private List<MapInstance> MapInstances = new();
    private List<CharacterPlayer> Players = new();
    public IGameCore GameCore { get; set; } = null!;
    public Type statsEnum { get; set; } = null!;
    public List<StatModifier> sortedModifiers { get; set; } = new();
    public List<Enum> NotCalculatedStats { get; set; } = new();
    private BgTask bgTask;

    public GameService(ILoggerFactory loggerFactory, BgTaskManager bgTaskManager, IServiceProvider services)
    {
        _logger = loggerFactory.CreateLogger<GameService>();
        BgTaskManager = bgTaskManager;
        CoreLoader = new CoreLoader(CoreName, loggerFactory.CreateLogger<CoreLoader>(), this);
        bgTask = new BgTask("Main Game Loop", BackgroundLoop);
        this.serviceProvider = services;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        //TODO: this should be done after every reload
        List<StatModifier> AllModifiers = new();
        NotCalculatedStats = new();
        var stats = Enum.GetValues(statsEnum).Cast<Enum>().ToList();
        foreach (var stat in stats)
        {
            //skip over stats with [Notcalculated]
            var attributes = stat.GetType().GetMember(stat.ToString()).First(m => m.DeclaringType == statsEnum).GetCustomAttributes(false);
            if (attributes.Any(a => a is NotCalculatedAttribute))
            {
                NotCalculatedStats.Add(stat);
                continue;
            }
            AllModifiers.Add(GameCore.CalculateInitialStat(stat));
        }

        foreach (var item in ItemTemplates.Where(item => item.GetType().IsAssignableTo(typeof(IEquippable))).Select(item => (IEquippable)item))
            AllModifiers.AddRange(item.EquipEffects);

        _logger.LogInformation($"Found {AllModifiers.Count} modifiers in total");

        sortedModifiers = new();
        while (AllModifiers.Count > 0)
        {
            List<StatModifier> toRemove = new();
            foreach (var modifier in AllModifiers)
            {
                if (modifier.StatsUsed.All(m => !AllModifiers.Any(mm => mm.Stat == m)))
                {
                    sortedModifiers.Add(modifier);
                    toRemove.Add(modifier);
                }
            }
            AllModifiers.RemoveAll(m => toRemove.Contains(m));
        }
        _logger.LogInformation($"Sorted {sortedModifiers.Count} modifiers in total");

        Maps = GameCore.LoadMaps();
        SemaphoreSlim LoadSemaphore = new SemaphoreSlim(6); // 6 maps at the same time
        List<Task> loadTasks = new();
        foreach (var map in Maps)
            loadTasks.Add(Task.Run(async () => { await LoadSemaphore.WaitAsync(); try { _logger.LogInformation($"Loading map {map.Name}"); map.Load(); map.Loaded = true; } finally { LoadSemaphore.Release(); } }));
        await Task.WhenAll(loadTasks.ToArray());

        foreach(var map in Maps.Where(m => m.InstanceType == InstanceType.NoInstance))
            _ = map.MapInstance(GameCore, serviceProvider);

        _logger.LogInformation($"Loaded {Maps.Count} maps");

        BgTaskManager.Run(bgTask);
        await Task.Yield();
    }


    public CharacterPlayer GetCharacter(ulong id)
    {
        var character = Players.FirstOrDefault(c => c.Id == id);
        if (character == null)
        {
            character = LoadCharacter(id);
            if (character == null)
                character = CreateCharacter(id);
            if (character != null)
                Players.Add(character);
        }

        character?.CalculateStats();
        return character ?? throw new Exception("Could not get character");
    }
    public CharacterPlayer? LoadCharacter(ulong id)
    {
        using var scope = serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dbCharacter = context.Characters.Include(c => c.Stats).FirstOrDefault(c => c.Id == id);
        if(dbCharacter == null)
            return null;

        _logger.LogInformation($"Loading character {dbCharacter.Name} from database", dbCharacter);
        var character = new CharacterPlayer(serviceProvider)
        {
            Id = id,
            Name = dbCharacter.Name,
            Location = GetLocation(GameCore.SpawnLocation), //TODO
            Stats = dbCharacter.Stats.ToDictionary(s => (Enum)Enum.Parse(statsEnum, s.Stat), s => s.Value),
        };
        character.Location.MapInstance.Characters.Add(character);

        return character;
    }

    private Location GetLocation((Point position, string mapName) spawnLocation)
    {
        var map = Maps.First(map => map.Name == spawnLocation.mapName);
        var instance = map.MapInstance(GameCore, serviceProvider);
        return new Location(spawnLocation.position.X, spawnLocation.position.Y)
        {
            MapInstance = instance
        };
    }

    public CharacterPlayer CreateCharacter(ulong id)
    {
        _logger.LogInformation($"Creating character {id}");
        CharacterPlayer newCharacter = new CharacterPlayer(serviceProvider)
        {
            Id = id,
            Stats = NotCalculatedStats.ToDictionary(s => s, s => 1L), //initialize with 1? or use gamecore initial stat calculation
            Location = GetLocation(GameCore.SpawnLocation), //TODO
        };
        newCharacter.Location.MapInstance.Characters.Add(newCharacter);
        return newCharacter;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Game service is stopping.");
        CoreLoader.Dispose();
        await bgTask.Cancel();
    }

    public async Task BackgroundLoop(CancellationToken cancellationToken)
    {
        while(true)
        {
            if(GameCore == null)
            {
                await Task.Delay(1000);
                continue;
            }

            //_logger.LogInformation("Tick");
            await Task.Delay(1000);
        }
    }
}