using Discord;
using Discord.Interactions;
using IdleRpg.Game;
using IdleRpg.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;

namespace IdleRpg.Services.Discord.Modules;

public class StartMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private MapGeneratorService mapGenerator;

    public StartMenu(GameService gameService, MapGeneratorService mapGenerator)
    {
        this.gameService = gameService;
        this.mapGenerator = mapGenerator;
    }

    [ComponentInteraction("start:new")]
    public async Task Start()
    {
        //if no character exists, create it and show intro
        var character = gameService.GetCharacter(Context.User.Id);


        await RespondAsync(null, ephemeral: true, components: new ComponentBuilderV2().WithTextDisplay("Loading...").Build());
        await UpdateStart();
    }

    [ComponentInteraction("start")]
    public async Task StartReplace()
    {
        //if(Context.Interaction.HasResponded)
        await DeferAsync(ephemeral: true);
        await UpdateStart();
    }

    public async Task UpdateStart()
    {
        var character = gameService.GetCharacter(Context.User.Id);
        using var mapStream = mapGenerator.GenerateMapImage(character, 128, 2).AsPngStream();


        await ModifyOriginalResponseAsync(c =>
        {
            c.Attachments = new List<FileAttachment>() { new FileAttachment(mapStream, "map.png") };
            c.Components = new ComponentBuilderV2()
                .WithTextDisplay("### Main Menu")
                .WithSeparator()
                .WithMediaGallery(["https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/250740/ss_a8ed2612270b0080b514ddcf364f7142dc599581.600x338.jpg?t=1566831836", "attachment://map.png"])
                .WithTextDisplay($"Your character:\n" +
                $"- Your character is {character.Status}\n" +
                $"- Your character is on {character.Location.MapInstance.Map.Name}, at {character.Location.X}, {character.Location.Y}\n" +
                "")
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder("Character", "character", ButtonStyle.Primary, emote: Emoji.Parse(":man_mage:")),
                    new ButtonBuilder("Battle", "battle", ButtonStyle.Primary, emote: Emoji.Parse(":crossed_swords:")),
                    new ButtonBuilder("Inventory", "inventory", ButtonStyle.Primary, emote: Emoji.Parse(":handbag:")),
                    new ButtonBuilder("Handbook", "handbook", ButtonStyle.Primary, emote: Emoji.Parse(":notebook_with_decorative_cover:")),
                    new ButtonBuilder("Quests", "quests", ButtonStyle.Primary, emote: Emoji.Parse(":compass:")),
                ])
                .WithActionRow([
                    new ButtonBuilder("Refresh", "start", ButtonStyle.Secondary, emote: Emoji.Parse(":arrows_counterclockwise:")),
                ])
                .Build();
        });
    }

}
