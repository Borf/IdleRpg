
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Runtime.Loader;
using IdleRpg.Game.Core;

namespace IdleRpg.Game;

public class GameService : IHostedService
{
    private readonly ILogger<GameService> _logger;
    private Task bgTask;
    IGameCore GameCore;
    private CoreLoader CoreLoader;

    public GameService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<GameService>();
        CoreLoader = new CoreLoader("Rom", loggerFactory.CreateLogger<CoreLoader>(), c => GameCore = c);
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Game service is starting.");
        bgTask = Task.Run(BackgroundLoop, cancellationToken);
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