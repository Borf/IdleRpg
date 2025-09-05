
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace IdleRpg.Services.Discord;

public class DiscordService : IHostedService
{
    private DiscordSocketClient client;
    private IConfiguration config;
    private ILogger<DiscordService> logger;

    public DiscordService(DiscordSocketClient discordClient, IConfiguration config, ILogger<DiscordService> logger)
    {
        this.client = discordClient;
        this.config = config;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        client.Log += Log;

        await client.LoginAsync(TokenType.Bot, config["bottoken"]);
        await client.StartAsync();
    }

    private Task Log(LogMessage message)
    {
        var severity = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information
        };

        logger.Log(severity, message.Exception, $"[{message.Source}] {message.Message}");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
