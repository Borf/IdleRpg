using Discord;
using Discord.Interactions;
using IdleRpg.Game;
using IdleRpg.Game.Core;
using IdleRpg.Util;
using SixLabors.ImageSharp;

namespace IdleRpg.Services.Discord.Modules;

public class ActionsWalkMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private ILogger<ActionsMenu> logger;
    private MapGeneratorService mapGenerator;
    private DiscordMessageBuilderService dmb;

    public ActionsWalkMenu(GameService gameService, MapGeneratorService mapGenerator, ILogger<ActionsMenu> logger, DiscordMessageBuilderService dmb)
    {
        this.gameService = gameService;
        this.logger = logger;
        this.mapGenerator = mapGenerator;
        this.dmb = dmb;
    }

    [ComponentInteraction("actions:walk")]
    public async Task CharacterWorldmap()
    {
        var character = gameService.GetCharacter(Context.User.Id);
        var map = character.Location.MapInstance.Map;

        var x = Math.Max(0, character.Location.X - 32);
        var y = Math.Max(0, character.Location.Y - 32);
        int width = 64;
        int height = 64;
        await CharacterWorldmap(x,y,width,height);
    }

    [ComponentInteraction("actions:walk_:*:*:*:*")]
    public async Task CharacterWorldmap(int x, int y, int width, int height)
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        var map = character.Location.MapInstance.Map;
        using var mapImage = mapGenerator.GenerateWorldMapImage(character, new Rectangle(x, y, width, height));
        using var mapStream = mapImage.AsPngStream();

        await ModifyOriginalResponseAsync(c =>
        {
            c.Attachments = new List<FileAttachment>() { new FileAttachment(mapStream, "map.png") };
            c.Components = new ComponentBuilderV2()
                .WithNavigation(Context.Interaction)
//                .WithTextDisplay($"Your character:\n" +
//                $"- Your character is on {character.Location.MapInstance.Map.Name}, at {character.Location.X}, {character.Location.Y}\n" +
//                $"- Map: {x} {y} {width} {height}\n")
                .WithMediaGallery(["attachment://map.png"])
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder("A1", $"actions:walk_2:{x}:{y}:{width}:{height}:A1", ButtonStyle.Secondary),
                    new ButtonBuilder("A2", $"actions:walk_2:{x}:{y}:{width}:{height}:A2", ButtonStyle.Secondary),
                    new ButtonBuilder("A3", $"actions:walk_2:{x}:{y}:{width}:{height}:A3", ButtonStyle.Secondary),
                    new ButtonBuilder("A4", $"actions:walk_2:{x}:{y}:{width}:{height}:A4", ButtonStyle.Secondary),
                ])
                .WithActionRow([
                    new ButtonBuilder("B1", $"actions:walk_2:{x}:{y}:{width}:{height}:B1", ButtonStyle.Secondary),
                    new ButtonBuilder("B2", $"actions:walk_2:{x}:{y}:{width}:{height}:B2", ButtonStyle.Secondary),
                    new ButtonBuilder("B3", $"actions:walk_2:{x}:{y}:{width}:{height}:B3", ButtonStyle.Secondary),
                    new ButtonBuilder("B4", $"actions:walk_2:{x}:{y}:{width}:{height}:B4", ButtonStyle.Secondary),
                ])
                .WithActionRow([
                    new ButtonBuilder("C1", $"actions:walk_2:{x}:{y}:{width}:{height}:C1", ButtonStyle.Secondary),
                    new ButtonBuilder("C2", $"actions:walk_2:{x}:{y}:{width}:{height}:C2", ButtonStyle.Secondary),
                    new ButtonBuilder("C3", $"actions:walk_2:{x}:{y}:{width}:{height}:C3", ButtonStyle.Secondary),
                    new ButtonBuilder("C4", $"actions:walk_2:{x}:{y}:{width}:{height}:C4", ButtonStyle.Secondary),
                ])
                .WithActionRow([
                    new ButtonBuilder("D1", $"actions:walk_2:{x}:{y}:{width}:{height}:D1", ButtonStyle.Secondary),
                    new ButtonBuilder("D2", $"actions:walk_2:{x}:{y}:{width}:{height}:D2", ButtonStyle.Secondary),
                    new ButtonBuilder("D3", $"actions:walk_2:{x}:{y}:{width}:{height}:D3", ButtonStyle.Secondary),
                    new ButtonBuilder("D4", $"actions:walk_2:{x}:{y}:{width}:{height}:D4", ButtonStyle.Secondary),
                ])
                .WithActionRow([
                    new ButtonBuilder(" ", $"actions:walk_2:{x}:{y}:{width}:{height}:Z", ButtonStyle.Secondary, emote: Emoji.Parse(":mag:")),
                    new ButtonBuilder(" ", $"actions:walk_2:{x}:{y}:{width}:{height}:N", ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_up:")),
                    new ButtonBuilder(" ", $"actions:walk_2:{x}:{y}:{width}:{height}:E", ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_right:")),
                    new ButtonBuilder(" ", $"actions:walk_2:{x}:{y}:{width}:{height}:S", ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_down:")),
                    new ButtonBuilder(" ", $"actions:walk_2:{x}:{y}:{width}:{height}:W", ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_left:")),
                ])
                .Build();
        });
    }

    [ComponentInteraction("actions:walk_2:*:*:*:*:*")]
    public async Task CharacterWorldmapMove(int x, int y, int width, int height, string newTile)
    {
        var character = gameService.GetCharacter(Context.User.Id);
        var map = character.Location.MapInstance.Map;

        if (newTile == "Z")
        {
            x = Math.Max(0, x - width / 2);
            y = Math.Max(0, y - height / 2);
            width = Math.Min(map.Width, width*2);
            height = Math.Min(map.Width, height*2);
        }
        else if ("NESW".Contains(newTile))
        {
            if (newTile == "N")
                y = Math.Max(0, y - height / 2);
            else if (newTile == "S")
                y = Math.Min(map.Height, x + height / 2);
            else if (newTile == "W")
                x = Math.Max(0, x - width / 2);
            else if(newTile == "E")
                x = Math.Min(map.Width, x + width / 2);
        }
        else if (newTile.Length == 2)
        {
            //-3, -1, 1, 3
            var yy = ("ABCD".IndexOf(newTile[0]));
            var xx = ("1234".IndexOf(newTile[1]));

            width = width / 4;
            height = height / 4;

            x += xx * width;
            y += yy * width;

            if (width < 16)
            {
                int X = Random.Shared.Next(x, x + width);
                int Y = Random.Shared.Next(y, y + height);
                while (!map[X, Y].HasFlag(CellType.Walkable))
                {
                    X = Random.Shared.Next(x, x + width);
                    Y = Random.Shared.Next(y, y + height);
                }
                logger.LogInformation($"Moving character {character.Name} to {X}, {Y}");
                await character.ActionQueue.ClearActions(); //TODO: only clear if walking..otherwise ask question?
                character.WalkTo(new Location(X, Y, character.Location));
                await DeferAsync(ephemeral: true);
                await dmb.ActionsMenu(Context.Interaction, character, "You started walking");
                return;
            }
        }
        await CharacterWorldmap(x, y, width, height);
    }







    [ComponentInteraction("actions:move:walk")]
    public async Task CharacterWalk()
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        using var mapImage = mapGenerator.GenerateMapImage(character, 512, 1);
        using var mapStream = mapImage.AsPngStream();
        await ModifyOriginalResponseAsync(c =>
        {
            c.Attachments = new List<FileAttachment>() { new FileAttachment(mapStream, "map.png") };
            c.Components = new ComponentBuilderV2()
                .WithTextDisplay("### Main Menu > Character > Move > Walk")
                .WithSeparator()
                .WithTextDisplay($"Your character:\n" +
                $"- Your character is on {character.Location.MapInstance.Map.Name}, at {character.Location.X}, {character.Location.Y}\n")
                .WithMediaGallery(["attachment://map.png"])
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder(" ", "character:move:walk:1", ButtonStyle.Secondary, emote: Emoji.Parse(":blue_square:")),
                    new ButtonBuilder(" ", "character:move:walk:u", ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_up:")),
                    new ButtonBuilder(" ", "character:move:walk:2", ButtonStyle.Secondary, emote: Emoji.Parse(":blue_square:")),
                ])
                .WithActionRow([
                    new ButtonBuilder(" ", "character:move:walk:l", ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_left:")),
                    new ButtonBuilder(" ", "character:move:walk:d", ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_down:")),
                    new ButtonBuilder(" ", "character:move:walk:r", ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_right:")),
                ])
                .WithActionRow([
                    new ButtonBuilder("Refresh", "character:move:walk", ButtonStyle.Secondary, emote: Emoji.Parse(":arrows_counterclockwise:")),
                    new ButtonBuilder("Back", "character:move", ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_backward:")),
                ])
                .Build();
        });
    }



    [ComponentInteraction("actions:move:walk:*")]
    public async Task CharacterDoWalk(string direction)
    {
        var character = gameService.GetCharacter(Context.User.Id);
        if (direction == "u")
            character.Location.Y--;
        else if (direction == "d")
            character.Location.Y++;
        else if (direction == "l")
            character.Location.X--;
        else if (direction == "r")
            character.Location.X++;
        await CharacterWalk();
    }
}
