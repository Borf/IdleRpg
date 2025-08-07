using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using System.Threading.Tasks;
namespace IdleRpg.Game;

public class GameService : ICoreHolder
{
    public const string CoreName = "TinyRpg";
    private readonly ILogger<GameService> _logger;
    private readonly BgTaskManager BgTaskManager;
    private CoreLoader CoreLoader;
    public List<IItem> Items { get; set; } = new();
    private List<Map> Maps = new();
    private List<MapInstance> MapInstances = new();

    public IGameCore GameCore { get; set; } = null!;
    public Type statsEnum { get; set; } = null!;
    private BgTask bgTask;

    public GameService(ILoggerFactory loggerFactory, BgTaskManager bgTaskManager)
    {
        _logger = loggerFactory.CreateLogger<GameService>();
        BgTaskManager = bgTaskManager;
        CoreLoader = new CoreLoader(CoreName, loggerFactory.CreateLogger<CoreLoader>(), this);
        bgTask = new BgTask("Main Game Loop", BackgroundLoop);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        //TODO: this should be done after every reload
        List<StatModifier> AllModifiers = new();

        var stats = Enum.GetValues(statsEnum).Cast<Enum>().ToList();
        foreach (var stat in stats)
        {
            //skip over stats with [Notcalculated]
            var attributes = stat.GetType().GetMember(stat.ToString()).First(m => m.DeclaringType == statsEnum).GetCustomAttributes(false);
            if (attributes.Any(a => a is NotCalculatedAttribute))
                continue;

            AllModifiers.Add(GameCore.CalculateInitialStat(stat));
        }

        foreach (var item in Items.Where(item => item.GetType().IsAssignableTo(typeof(IEquippable))).Select(item => (IEquippable)item))
            AllModifiers.AddRange(item.EquipEffects);

        _logger.LogInformation($"Found {AllModifiers.Count} modifiers in total");

        //TODO: move this somewhere else
        List<StatModifier> sortedModifiers = new();
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

        Maps = GameCore.LoadMaps(); //TODO: should happen more often?

        _logger.LogInformation($"Loaded {Maps.Count} maps");

        BgTaskManager.Run(bgTask);
        await Task.Yield();
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

            _logger.LogInformation("Tick");
            await Task.Delay(1000);
        }
    }
}