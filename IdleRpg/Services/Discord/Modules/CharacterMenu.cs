using Discord;
using Discord.Interactions;

namespace IdleRpg.Services.Discord.Modules;

public class CharacterMenu : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("character")]
    public async Task Character()
    {
        await DeferAsync(ephemeral: true);
        //if no character exists, create it
        await ModifyOriginalResponseAsync(c =>
        {
            c.Components = new ComponentBuilderV2()
                .WithTextDisplay("### Main Menu > Character")
                .WithSeparator()
                .WithMediaGallery(["https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/250740/ss_a8ed2612270b0080b514ddcf364f7142dc599581.600x338.jpg?t=1566831836"])
                .WithTextDisplay("Your character is currently: ")
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder("Stats", "character:stats",       ButtonStyle.Primary, emote: Emoji.Parse(":handbag:")),
                    new ButtonBuilder("Skills", "character:skills",     ButtonStyle.Primary, emote: Emoji.Parse(":notebook_with_decorative_cover:")),
                    new ButtonBuilder("Equip", "character:equip",       ButtonStyle.Primary, emote: Emoji.Parse(":compass:")),
                    new ButtonBuilder("Jobs", "character:jobs",         ButtonStyle.Primary, emote: Emoji.Parse(":compass:")),
                    new ButtonBuilder("Dress Up", "character:dressup",  ButtonStyle.Primary, emote: Emoji.Parse(":compass:")),
                ])
                .WithActionRow([
                    new ButtonBuilder("Refresh", "character",           ButtonStyle.Secondary, emote: Emoji.Parse(":arrows_counterclockwise:")),
                    new ButtonBuilder("Back", "start",                  ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_backward:")),
                ])
                .Build();
        });
    }


}
