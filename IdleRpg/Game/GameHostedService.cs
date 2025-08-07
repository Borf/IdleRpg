using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;

namespace IdleRpg.Game;

public class GameHostedService : IHostedService
{
    private readonly ILogger<GameHostedService> _logger;
    private readonly GameService gameService;

    public GameHostedService(ILogger<GameHostedService> logger, GameService gameService)
    {
        _logger = logger;
        this.gameService = gameService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Game service is starting.");
        //TODO: load from list, to determine what maps to load and which one to instantiate immediately
        await gameService.StartAsync(cancellationToken);
        await Task.Yield();
    }



    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Game service is stopping.");
        await gameService.StopAsync(cancellationToken);
    }

}
