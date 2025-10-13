using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace IdleRpg.Services.Discord;

public static class ServiceHelper
{
    public static void AddDiscord(this IServiceCollection serviceCollection)
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged & ~GatewayIntents.GuildScheduledEvents & ~GatewayIntents.GuildInvites
        };
        var client = new DiscordSocketClient(config);
        serviceCollection.AddSingleton(client);
        serviceCollection.AddHostedService<DiscordService>();
        serviceCollection.AddSingleton<InteractionService>(s => new InteractionService(s.GetRequiredService<DiscordSocketClient>(), new InteractionServiceConfig()
        {
            
        }));
        serviceCollection.AddSingleton<InteractionHandler>();
        serviceCollection.AddHostedService<InteractionHandler>(s => s.GetRequiredService<InteractionHandler>());

    }
}
