using Discord;
using Discord.Interactions;
using IdleRpg.Game;
using IdleRpg.Game.Core;
using IdleRpg.Util;

namespace IdleRpg.Services.Discord.Modules;

public class ActionsBattleMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private ILogger<ActionsBattleMenu> logger;
    private DiscordMessageBuilderService dmb;
    public ActionsBattleMenu(GameService gameService, MapGeneratorService mapGenerator, ILogger<ActionsBattleMenu> logger, DiscordMessageBuilderService dmb)
    {
        this.gameService = gameService;
        this.logger = logger;
        this.dmb = dmb;
    }

    [ComponentInteraction("actions:battle")]
    public async Task Battle()
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        using var header = ((IDiscordGame)gameService.GameCore).HeaderGenerator.GetImage(DiscordMenu.Main, character);
        using var headerStream = header.AsPngStream();

        await ModifyOriginalResponseAsync(c =>
        {
            c.AddAttachment(new FileAttachment(headerStream, "header.png"));
            c.Components = new ComponentBuilderV2()
                .WithMediaGallery(["attachment://header.png"])
                .WithNavigation("actions:battle")
                .WithSeparator()
                .WithTextDisplay($"Current settings:\n" +
                $"- Farm time: {character.NextFarmAction.TimeSpan}\n" +
                $"- Mobs: {(character.NextFarmAction.MobIds.Any() ? string.Join(", ", character.NextFarmAction.MobIds.Select(m => gameService.MonsterTemplates[m].Name)) : "Any")}")
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder("Start", "actions:battle:start",            ButtonStyle.Success, emote: Emoji.Parse(":play_pause:")),
                    new ButtonBuilder("Set Monsters", "actions:battle:settings:mobs",ButtonStyle.Primary, emote: Emoji.Parse(":japanese_ogre:")),
                ])
                .WithActionRow([
                    new ButtonBuilder("1hr", "actions:battle:settings:time:1",    character.NextFarmAction.TimeSpan.TotalHours == 1 ? ButtonStyle.Primary : ButtonStyle.Secondary, emote: Emoji.Parse(":clock1:")),
                    new ButtonBuilder("2hr", "actions:battle:settings:time:2",    character.NextFarmAction.TimeSpan.TotalHours == 2 ? ButtonStyle.Primary : ButtonStyle.Secondary, emote: Emoji.Parse(":clock2:")),
                    new ButtonBuilder("5hr", "actions:battle:settings:time:5",    character.NextFarmAction.TimeSpan.TotalHours == 5 ? ButtonStyle.Primary : ButtonStyle.Secondary, emote: Emoji.Parse(":clock5:")),
                    new ButtonBuilder("10hr", "actions:battle:settings:time:10",  character.NextFarmAction.TimeSpan.TotalHours == 10? ButtonStyle.Primary : ButtonStyle.Secondary, emote: Emoji.Parse(":clock10:")),
                ])
                .Build();
        });
        headerStream.Dispose();
    }


    [ComponentInteraction("actions:battle:start")]
    public async Task BattleStart()
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        character.ActionQueue.QueueAction(character.NextFarmAction);
        character.NextFarmAction = new(character);

        await dmb.ActionsMenu(Context.Interaction, character, "You started battling");
    }

    [ComponentInteraction("actions:battle:settings:time:*")]
    public async Task BattleSettingsTime(int time)
    {
        var character = gameService.GetCharacter(Context.User.Id);
        character.NextFarmAction.TimeSpan = TimeSpan.FromHours(time);
        await Battle();
    }

}
