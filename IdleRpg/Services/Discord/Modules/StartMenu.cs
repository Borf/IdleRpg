using Discord;
using Discord.Interactions;

namespace IdleRpg.Services.Discord.Modules;

public class StartMenu : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("start:new")]
    public async Task Start()
    {
        //if no character exists, create it and show intro
        await RespondAsync("",
            embed: new EmbedBuilder()
                .WithImageUrl("https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/250740/ss_a8ed2612270b0080b514ddcf364f7142dc599581.600x338.jpg?t=1566831836")
                .WithTitle("Idle RPG - Main Menu")
                .WithDescription("Your character is currently: ...")
                .Build(),
            components: new ComponentBuilder()
                .WithButton("Character", "character", ButtonStyle.Primary, emote: Emoji.Parse(":man_mage:"))
                .WithButton("Battle", "battle", ButtonStyle.Primary, emote: Emoji.Parse(":crossed_swords:"))
                .WithButton("Inventory", "inventory", ButtonStyle.Primary, emote: Emoji.Parse(":handbag:"))
                .WithButton("Handbook", "handbook", ButtonStyle.Primary, emote: Emoji.Parse(":notebook_with_decorative_cover:"))
                .WithButton("Quests", "quests", ButtonStyle.Primary, emote: Emoji.Parse(":compass:"))
                .Build(),
            ephemeral: true
                );
    }

    [ComponentInteraction("start")]
    public async Task StartReplace()
    {
        await DeferAsync(ephemeral: true);
        await ModifyOriginalResponseAsync(c =>
        {
            c.Embed = new EmbedBuilder()
                .WithImageUrl("https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/250740/ss_a8ed2612270b0080b514ddcf364f7142dc599581.600x338.jpg?t=1566831836")
                .WithTitle("Idle RPG - Main Menu")
                .Build();
            c.Components = new ComponentBuilder()
                .WithButton("Character", "character", ButtonStyle.Primary, emote: Emoji.Parse(":man_mage:"))
                .WithButton("Battle", "battle", ButtonStyle.Primary, emote: Emoji.Parse(":crossed_swords:"))
                .WithButton("Inventory", "inventory", ButtonStyle.Primary, emote: Emoji.Parse(":handbag:"))
                .WithButton("Handbook", "handbook", ButtonStyle.Primary, emote: Emoji.Parse(":notebook_with_decorative_cover:"))
                .WithButton("Quests", "quests", ButtonStyle.Primary, emote: Emoji.Parse(":compass:"))
                .Build();
        });
    }

}
