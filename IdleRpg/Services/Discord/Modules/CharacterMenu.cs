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
            c.Embed = new EmbedBuilder()
                .WithImageUrl("https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/250740/ss_a8ed2612270b0080b514ddcf364f7142dc599581.600x338.jpg?t=1566831836")
                .WithTitle("Idle RPG - Main Menu")
                .Build();
            c.Components = new ComponentBuilder()
                .WithButton("Stats", "character:stats",     ButtonStyle.Primary, emote: Emoji.Parse(":man_mage:"))
                .WithButton("Skills", "character:skills",   ButtonStyle.Primary, emote: Emoji.Parse(":crossed_swords:"))
                .WithButton("Equipment", "character:equip", ButtonStyle.Primary, emote: Emoji.Parse(":crossed_swords:"))
                .WithButton("Jobs", "character:jobs",       ButtonStyle.Secondary, emote: Emoji.Parse(":compass:"))
                .WithButton("Dress Up", "character:dressup",ButtonStyle.Secondary, emote: Emoji.Parse(":compass:"))
                .Build();
        });
    }


}
