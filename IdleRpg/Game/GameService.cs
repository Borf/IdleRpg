
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Runtime.Loader;
using IdleRpg.Game.Core;
using Microsoft.Identity.Client;

namespace IdleRpg.Game;

public class GameService : IHostedService
{
    public const string CoreName = "Rom";
    private readonly ILogger<GameService> _logger;
    private Task? bgTask;
    IGameCore GameCore = null!;
    private CoreLoader CoreLoader;
    private List<Map> Maps = new();
    private List<MapInstance> MapInstances = new();

    public GameService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<GameService>();
        CoreLoader = new CoreLoader(CoreName, loggerFactory.CreateLogger<CoreLoader>(), c => GameCore = c);
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Game service is starting.");
        bgTask = Task.Run(BackgroundLoop, cancellationToken);

        //TODO: load from list, to determine what maps to load and which one to instantiate immediately

        Maps.Add(new Map("WorldMap"));




    }



    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Game service is stopping.");
        CoreLoader.Dispose();
        return Task.CompletedTask;
    }


    public async Task BackgroundLoop()
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