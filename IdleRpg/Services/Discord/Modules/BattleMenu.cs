using Discord;
using Discord.Interactions;
using IdleRpg.Game;

namespace IdleRpg.Services.Discord.Modules;

public class BattleMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private ILogger<BattleMenu> logger;
    public BattleMenu(GameService gameService, MapGeneratorService mapGenerator, ILogger<BattleMenu> logger)
    {
        this.gameService = gameService;
        this.logger = logger;
    }

    [ComponentInteraction("battle")]
    public async Task Battle()
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        await ModifyOriginalResponseAsync(c =>
        {
            c.Components = new ComponentBuilderV2()
                .WithTextDisplay("### Main Menu > Battle")
                .WithSeparator()
                .WithTextDisplay($"Current settings:\n" +
                $"- Farm time: {character.NextFarmAction.TimeSpan}\n" +
                $"- Mobs: {(character.NextFarmAction.MobIds.Any() ? string.Join(", ", character.NextFarmAction.MobIds.Select(m => gameService.NpcTemplates[m].Name)) : "Any")}")
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder("Start", "battle:start",            ButtonStyle.Success, emote: Emoji.Parse(":play_pause:")),
                    new ButtonBuilder("Stop", "battle:stop",              ButtonStyle.Danger,  emote: Emoji.Parse(":stop_button:")),
                    new ButtonBuilder("Set Monsters", "battle:settings:mobs",ButtonStyle.Primary, emote: Emoji.Parse(":japanese_ogre:")),
                ])
                .WithActionRow([
                    new ButtonBuilder("1hr", "battle:settings:time:1",    character.NextFarmAction.TimeSpan.TotalHours == 1 ? ButtonStyle.Primary : ButtonStyle.Secondary, emote: Emoji.Parse(":clock1:")),
                    new ButtonBuilder("2hr", "battle:settings:time:2",    character.NextFarmAction.TimeSpan.TotalHours == 2 ? ButtonStyle.Primary : ButtonStyle.Secondary, emote: Emoji.Parse(":clock2:")),
                    new ButtonBuilder("5hr", "battle:settings:time:5",    character.NextFarmAction.TimeSpan.TotalHours == 5 ? ButtonStyle.Primary : ButtonStyle.Secondary, emote: Emoji.Parse(":clock5:")),
                    new ButtonBuilder("10hr", "battle:settings:time:10",  character.NextFarmAction.TimeSpan.TotalHours == 10? ButtonStyle.Primary : ButtonStyle.Secondary, emote: Emoji.Parse(":clock10:")),
                ])
                .WithActionRow([
                    new ButtonBuilder("Refresh", "battle",                ButtonStyle.Secondary, emote: Emoji.Parse(":arrows_counterclockwise:")),
                    new ButtonBuilder("Back", "start",                    ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_backward:")),
                ])
                .Build();
        });
    }


    [ComponentInteraction("battle:start")]
    public async Task BattleStart()
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        character.ActionQueue.QueueAction(character.NextFarmAction);
        character.NextFarmAction = new(character);
        await ModifyOriginalResponseAsync(c =>
        {
            c.Components = new ComponentBuilderV2()
                .WithTextDisplay("### Main Menu > Battle > Start")
                .WithSeparator()
                .WithTextDisplay($"You started farming mobs")
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder("Back", "start",                    ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_backward:")),
                ])
                .Build();
        });
    }

    [ComponentInteraction("battle:settings:time:*")]
    public async Task BattleSettingsTime(int time)
    {
        var character = gameService.GetCharacter(Context.User.Id);
        character.NextFarmAction.TimeSpan = TimeSpan.FromHours(time);
        await Battle();
    }

}
