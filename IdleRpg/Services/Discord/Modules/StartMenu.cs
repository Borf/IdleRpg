using Discord;
using Discord.Interactions;
using IdleRpg.Data.Db;
using IdleRpg.Game;
using IdleRpg.Game.Core;
using IdleRpg.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;

namespace IdleRpg.Services.Discord.Modules;

public class StartMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private DiscordMessageBuilderService dmb;

    public StartMenu(GameService gameService, MapGeneratorService mapGenerator, DiscordMessageBuilderService dmb)
    {
        this.gameService = gameService;
        this.dmb = dmb;
    }

    [ComponentInteraction("start:new")]
    public async Task Start()
    {
        var character = gameService.GetCharacter(Context.User.Id);
        await RespondAsync(null, ephemeral: true, components: new ComponentBuilderV2().WithTextDisplay("Loading...").Build());
        if (character == null)
            await dmb.CreateCharacter(Context.Interaction);
        else
            await dmb.StartMenu(Context.Interaction, character);
    }

    [ComponentInteraction("start")]
    public async Task StartReplace()
    {
        if(!Context.Interaction.HasResponded)
            await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        await dmb.StartMenu(Context.Interaction, character);
    }
}
