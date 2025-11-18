using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using System.Reflection;
using IResult = Discord.Interactions.IResult;

namespace IdleRpg.Services.Discord;

public class InteractionHandler : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _handler;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    private readonly ILogger<InteractionHandler> _logger;

    public InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services, IConfiguration config, ILogger<InteractionHandler> logger)
    {
        _client = client;
        _handler = handler;
        _services = services;
        _configuration = config;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Ready += ReadyAsync;
        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        _client.InteractionCreated += HandleInteraction;
        _handler.InteractionExecuted += HandleInteractionExecute;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task ReadyAsync()
    {
        // Register the commands globally.
        // alternatively you can use _handler.RegisterCommandsGloballyAsync() to register commands to a specific guild.
        await _handler.RegisterCommandsGloballyAsync();
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            string name = "";
            if (interaction is SocketCommandBase interact)
                name = interact.CommandName;
            else if (interaction.Data.GetType().Name == "MessageComponentInteractionData")
                name = interaction.Data.GetType().GetProperty("CustomId")?.GetValue(interaction.Data)?.ToString() ?? "";

            _logger.LogInformation($"Got interaction {name} from {interaction.User.Username} in {interaction.Channel.Name}");
            var context = new SocketInteractionContext(_client, interaction);
            var result = await _handler.ExecuteCommandAsync(context, _services);
            if (!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        await interaction.RespondAsync("You do not have permission to run this command.", ephemeral: true);
                        break;
                    default:
                        await interaction.RespondAsync("Something went wrong running this command:\n" + result.ErrorReason, ephemeral: true);
                        break;
                }
        }
        catch (Exception e)
        {
            if(interaction.HasResponded)
                await interaction.ModifyOriginalResponseAsync(m => m.Content = $"Error: \n```\n{e}\n```");
            else
                await interaction.RespondAsync($"Error: \n```\n{e}\n```");
            //if (interaction.Type is InteractionType.ApplicationCommand)
            //    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
        }
    }

    private Task HandleInteractionExecute(ICommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        _logger.LogTrace("Handling interaction execution?");
        if (!result.IsSuccess)
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                default:
                    break;
            }

        return Task.CompletedTask;
    }




}