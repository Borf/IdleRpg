using Discord;
using Discord.Interactions;
using IdleRpg.Game;
using IdleRpg.Game.Core;
using IdleRpg.Util;

namespace IdleRpg.Services.Discord.Modules;

public class ActionsMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private DiscordMessageBuilderService dmb;

    public ActionsMenu(GameService gameService, MapGeneratorService mapGenerator, DiscordMessageBuilderService dmb)
    {
        this.gameService = gameService;
        this.dmb = dmb;
    }

    [ComponentInteraction("actions")]
    public async Task Actions()
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        await dmb.ActionsMenu(Context.Interaction, character);
    }

}
