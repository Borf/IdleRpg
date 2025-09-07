using Discord;
using Discord.Interactions;
using IdleRpg.Game;
using IdleRpg.Game.Attributes;
using IdleRpg.Util;

namespace IdleRpg.Services.Discord.Modules;

public class CharacterMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;

    public CharacterMenu(GameService gameService)
    {
        this.gameService = gameService;
    }

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


    [ComponentInteraction("character:stats")]
    public async Task CharacterStats()
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        //string stats = "";
        bool adjustable = false;
        Dictionary<string, string> statsPerGroup = new();

        foreach(var stat in character.Stats)
        {
            var group = ((Enum)stat.Key).GetAttributeOfType<IdleRpg.Game.Attributes.GroupAttribute>()?.Name ?? "";
            if(((Enum)stat.Key).GetAttributeOfType<AdjustableAttribute>() != null)
                adjustable = true;
            if (!statsPerGroup.ContainsKey(group))
                statsPerGroup[group] = "";
            statsPerGroup[group] += $"- `{stat.Key,-15}{stat.Value}`\n";
        }

        string stats = string.Join("\n", statsPerGroup.Select(g => $"## {g.Key}\n{g.Value}"));
        await ModifyOriginalResponseAsync(c =>
        {
            var cb = new ComponentBuilderV2()
                .WithTextDisplay("### Main Menu > Character > Stats")
                .WithSeparator()
                .WithMediaGallery(["https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/250740/ss_a8ed2612270b0080b514ddcf364f7142dc599581.600x338.jpg?t=1566831836"])
                .WithTextDisplay("Your stats:")
                .WithSeparator()
                .WithTextDisplay(stats);

            if (adjustable)
                cb.WithActionRow([
                        new ButtonBuilder("Change Stats", "character:stats:add",       ButtonStyle.Primary, emote: Emoji.Parse(":handbag:")),
                    ]);
            cb.WithActionRow([
                new ButtonBuilder("Refresh", "character:stats",     ButtonStyle.Secondary, emote: Emoji.Parse(":arrows_counterclockwise:")),
                new ButtonBuilder("Back", "character",              ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_backward:")),
            ]);

            c.Components = cb.Build();
                
        });
    }

}
