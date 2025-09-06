using Discord;
using Discord.Interactions;

namespace IdleRpg.Services.Discord.Modules;

public class BattleMenu : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("battle")]
    public async Task Battle()
    {
        await DeferAsync(ephemeral: true);
        //if no character exists, create it
        await ModifyOriginalResponseAsync(c =>
        {
            c.Components = new ComponentBuilderV2()
                .WithTextDisplay("### Main Menu > Battle")
                .WithSeparator()
                .WithMediaGallery(["https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/250740/ss_a8ed2612270b0080b514ddcf364f7142dc599581.600x338.jpg?t=1566831836"])
                .WithTextDisplay("Current settings")
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder("Start", "battle:start",            ButtonStyle.Success, emote: Emoji.Parse(":play_pause:")),
                    new ButtonBuilder("Stop", "battle:stop",              ButtonStyle.Danger,  emote: Emoji.Parse(":stop_button:")),
                    new ButtonBuilder("Settings", "battle:settings",      ButtonStyle.Primary, emote: Emoji.Parse(":gear:")),
                ])
                .WithActionRow([
                    new ButtonBuilder("Refresh", "battle",                ButtonStyle.Secondary, emote: Emoji.Parse(":arrows_counterclockwise:")),
                    new ButtonBuilder("Back", "start",                              ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_backward:")),
                ])
                .Build();
        });
    }


}
