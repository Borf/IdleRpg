using Discord;
using Discord.Interactions;
using IdleRpg.Game;
using IdleRpg.Game.Core;
using IdleRpg.Util;

namespace IdleRpg.Services.Discord.Modules;

public class CommandModule : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;

    public CommandModule(GameService gameService)
    {
        this.gameService = gameService;
    }

    [SlashCommand("setup", "Adds the control panel")]
    public async Task Setup()
    { 
        await RespondAsync("Creating...", ephemeral: true);

        using var header = ((IDiscordGame)gameService.GameCore).HeaderGenerator.GetImage(DiscordMenu.Main, null!);
        using var headerStream = header.AsPngStream();

        //TODO: get the introtext / image from the core
        await Context.Channel.SendFileAsync(new FileAttachment(headerStream, "header.png"), null,
            components: new ComponentBuilderV2()
                .WithTextDisplay("# Idle RPG")
                .WithMediaGallery(["attachment://header.png"])
                .WithSeparator()
                .WithTextDisplay("Play as a character in an emersive world. Fight monsters, get stronger, do quests, personalize your character and share the fun with other players")
                .WithActionRow(ar => ar
                    .WithButton("Start", "start:new", ButtonStyle.Success, emote: Emoji.Parse(":crossed_swords:"))
                    .WithButton("Settings", "settings:new", ButtonStyle.Secondary, emote: Emoji.Parse(":gear:"))
                    .WithButton("Help", "help", ButtonStyle.Secondary, emote: Emoji.Parse(":question:"))
                ).Build()
            );
    }

}
