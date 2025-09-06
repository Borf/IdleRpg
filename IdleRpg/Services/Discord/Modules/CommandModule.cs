using Discord;
using Discord.Interactions;

namespace IdleRpg.Services.Discord.Modules;

public class CommandModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("setup", "Adds the control panel")]
    public async Task Setup()
    { 
        await RespondAsync("Creating...", ephemeral: true);

        //TODO: get the introtext / image from the core
        await Context.Channel.SendMessageAsync("",
            embed: new EmbedBuilder()
                .WithImageUrl("https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/250740/ss_a8ed2612270b0080b514ddcf364f7142dc599581.600x338.jpg?t=1566831836")
                .WithTitle("Idle RPG!")
                .WithAuthor("Borf")
                .WithDescription("An immersive online world through discord! Just press the start button to get started!")
                .Build(),
            components: new ComponentBuilder()
                .WithButton("Start", "start:new", ButtonStyle.Success, emote: Emoji.Parse(":crossed_swords:"))
                .WithButton("Settings", "settings:new", ButtonStyle.Secondary, emote: Emoji.Parse(":gear:"))
                .WithButton("Help", "help", ButtonStyle.Secondary, emote: Emoji.Parse(":question:"))
                .Build()
                );
    }

}
