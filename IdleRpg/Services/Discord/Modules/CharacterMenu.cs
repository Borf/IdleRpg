using Discord;
using Discord.Interactions;
using IdleRpg.Game;
using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using IdleRpg.Util;
using SixLabors.ImageSharp;

namespace IdleRpg.Services.Discord.Modules;

public class CharacterMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private MapGeneratorService mapGenerator;
    private ILogger<CharacterMenu> logger;
    public CharacterMenu(GameService gameService, MapGeneratorService mapGenerator, ILogger<CharacterMenu> logger)
    {
        this.gameService = gameService;
        this.mapGenerator = mapGenerator;
        this.logger = logger;
    }

    [ComponentInteraction("character")]
    public async Task Character()
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);

        using var header = ((IDiscordGame)gameService.GameCore).HeaderGenerator.GetImage(DiscordMenu.Main, character);
        using var headerStream = header.AsPngStream();

        await ModifyOriginalResponseAsync(c =>
        {
            c.Attachments = new List<FileAttachment>() { new FileAttachment(headerStream, "header.png") };
            c.Components = new ComponentBuilderV2()
                .WithMediaGallery(["attachment://header.png"])
                .WithNavigation("character")
                .WithTextDisplay("Your character is currently: " + character.State)
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder("Stats", "character:stats",       ButtonStyle.Primary, emote: Emoji.Parse(":1234:")),
                    new ButtonBuilder("Inventory", "character:inventory",ButtonStyle.Primary, emote: Emoji.Parse(":briefcase:")),
                    new ButtonBuilder("Skills", "character:skills",     ButtonStyle.Primary, emote: Emoji.Parse(":notebook_with_decorative_cover:"), isDisabled: true),
                    new ButtonBuilder("Equip", "character:equip",       ButtonStyle.Primary, emote: Emoji.Parse(":womans_clothes:"), isDisabled: true),
                    new ButtonBuilder("Dress Up", "character:dressup",  ButtonStyle.Primary, emote: Emoji.Parse(":compass:"), isDisabled: true),
//                    new ButtonBuilder("Jobs", "character:jobs",         ButtonStyle.Primary, emote: Emoji.Parse(":compass:")),
                ])
                //.WithActionRow([
                //])
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
        Dictionary<string, Dictionary<string, string>> statsPerGroup = new();
        character.CalculateStats();
        foreach (var stat in character.Stats)
        {
            if (((Enum)stat.Key).GetAttributeOfType<IdleRpg.Game.Attributes.HiddenAttribute>() != null)
                continue;
            var group = ((Enum)stat.Key).GetAttributeOfType<IdleRpg.Game.Attributes.GroupAttribute>();
            var groupStr = group?.Name ?? "";
            if(group?.MaxValueOf != null)
                continue;

            if (((Enum)stat.Key).GetAttributeOfType<AdjustableAttribute>() != null)
                adjustable = true;
            if (!statsPerGroup.ContainsKey(groupStr))
                statsPerGroup[groupStr] = new();
            statsPerGroup[groupStr][stat.Key.ToString()] = stat.Value.ToString();
        }

        foreach (var stat in character.Stats)
        {
            if (((Enum)stat.Key).GetAttributeOfType<IdleRpg.Game.Attributes.HiddenAttribute>() != null)
                continue;
            var group = ((Enum)stat.Key).GetAttributeOfType<IdleRpg.Game.Attributes.GroupAttribute>();
            var groupStr = group?.Name ?? "";
            if (group?.MaxValueOf == null)
                continue;
            statsPerGroup[groupStr][group?.MaxValueOf!] += " / " + stat.Value.ToString();
        }

        string stats = string.Join("\n", statsPerGroup.Select(g => $"## {g.Key}\n{string.Join("\n", g.Value.Select(kv => $"- `{kv.Key,-15}`{kv.Value}"))}"));
        await ModifyOriginalResponseAsync(c =>
        {
            var cb = new ComponentBuilderV2()
                .WithMediaGallery(["attachment://header.png"])
                .WithNavigation("character:stats")
                .WithTextDisplay(stats);

            if (adjustable)
                cb.WithActionRow([
                        new ButtonBuilder("Change Stats", "character:stats:add",       ButtonStyle.Primary, emote: Emoji.Parse(":handbag:")),
                    ]);
            c.Components = cb.Build();
                
        });
    }
}
