using Discord;
using Discord.Interactions;
using IdleRpg.Game;
using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using IdleRpg.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using static System.Net.Mime.MediaTypeNames;

namespace IdleRpg.Services.Discord.Modules;

public class CharacterMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private MapGeneratorService mapGenerator;
    public CharacterMenu(GameService gameService, MapGeneratorService mapGenerator)
    {
        this.gameService = gameService;
        this.mapGenerator = mapGenerator;
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
                    new ButtonBuilder("Move", "character:move",  ButtonStyle.Primary, emote: Emoji.Parse(":compass:")),
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


    [ComponentInteraction("character:move")]
    public async Task CharacterMove()
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);

        using var mapStream = mapGenerator.GenerateMapImage(character, 512, 1).AsPngStream();


        await ModifyOriginalResponseAsync(c =>
        {
            c.Attachments = new List<FileAttachment>() { new FileAttachment(mapStream, "map.png") };
            c.Components = new ComponentBuilderV2()
                .WithTextDisplay("### Main Menu > Character > Move")
                .WithSeparator()
                .WithTextDisplay($"Your character:\n" +
                $"- Your character is on {character.Location.MapInstance.Map.Name}, at {character.Location.X}, {character.Location.Y}\n")
                .WithMediaGallery(["attachment://map.png"])
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder("Walk around", "character:move:walk", ButtonStyle.Secondary, emote: Emoji.Parse(":person_walking:")),
                    new ButtonBuilder("Move on worldmap", "character:move:worldmap", ButtonStyle.Secondary, emote: Emoji.Parse(":map:")),
                ])
                .WithActionRow([
                    new ButtonBuilder("Refresh", "character:move", ButtonStyle.Secondary, emote: Emoji.Parse(":arrows_counterclockwise:")),
                    new ButtonBuilder("Back", "character", ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_backward:")),
                ])
                .Build();
        });
    }


    [ComponentInteraction("character:move:worldmap")]
    public async Task CharacterWorldmap() => await CharacterWorldmap(0, 0, 1);
    [ComponentInteraction("character:move:worldmap:*:*:*")]
    public async Task CharacterWorldmap(int centerX, int centerY, int zoom)
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        using var mapStream = mapGenerator.GenerateWorldMapImage(character, centerX, centerY, zoom).AsPngStream();
        var map = character.Location.MapInstance.Map;
        int xOffset = map.MapImage.Width / 2 + centerX;
        int yOffset = map.MapImage.Height / 2 + centerY;
        int sizeX = map.MapImage.Width / zoom;
        int sizeY = map.MapImage.Height / zoom;
        var rect = new Rectangle(xOffset - (map.MapImage.Width / 2) / zoom, yOffset - (map.MapImage.Height / 2) / zoom, sizeX, sizeY);
        Console.WriteLine($"Rect: {rect}");
        var rect2 = new Rectangle(rect.X / map.MapImageSize, rect.Y / map.MapImageSize, sizeX / map.MapImageSize, sizeY / map.MapImageSize);
        Console.WriteLine($"=> Rect2: {rect2}");

        await ModifyOriginalResponseAsync(c =>
        {
            c.Attachments = new List<FileAttachment>() { new FileAttachment(mapStream, "map.png") };
            c.Components = new ComponentBuilderV2()
                .WithTextDisplay("### Main Menu > Character > Move > Worldmap")
                .WithSeparator()
                .WithTextDisplay($"Your character:\n" +
                $"- Your character is on {character.Location.MapInstance.Map.Name}, at {character.Location.X}, {character.Location.Y}\n" +
                $"- Map: {centerX} {centerY} {zoom}\n" +
                $"- Viewing tiles: {rect2.X},{rect2.Y} - {rect2.X+rect2.Width},{rect2.Y + rect2.Height}")
                .WithMediaGallery(["attachment://map.png"])
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder("A1", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:A1", ButtonStyle.Secondary),
                    new ButtonBuilder("A2", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:A2", ButtonStyle.Secondary),
                    new ButtonBuilder("A3", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:A3", ButtonStyle.Secondary),
                    new ButtonBuilder("A4", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:A4", ButtonStyle.Secondary),
                ])                          
                .WithActionRow([            
                    new ButtonBuilder("B1", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:B1", ButtonStyle.Secondary),
                    new ButtonBuilder("B2", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:B2", ButtonStyle.Secondary),
                    new ButtonBuilder("B3", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:B3", ButtonStyle.Secondary),
                    new ButtonBuilder("B4", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:B4", ButtonStyle.Secondary),
                ])                          
                .WithActionRow([            
                    new ButtonBuilder("C1", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:C1", ButtonStyle.Secondary),
                    new ButtonBuilder("C2", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:C2", ButtonStyle.Secondary),
                    new ButtonBuilder("C3", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:C3", ButtonStyle.Secondary),
                    new ButtonBuilder("C4", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:C4", ButtonStyle.Secondary),
                ])                          
                .WithActionRow([            
                    new ButtonBuilder("D1", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:D1", ButtonStyle.Secondary),
                    new ButtonBuilder("D2", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:D2", ButtonStyle.Secondary),
                    new ButtonBuilder("D3", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:D3", ButtonStyle.Secondary),
                    new ButtonBuilder("D4", $"character:move:worldmap2:{centerX}:{centerY}:{zoom}:D4", ButtonStyle.Secondary),
                ])
                .WithActionRow([
                    new ButtonBuilder("Move", $"character:move:worldmapmove:{centerX}:{centerY}:{zoom}", ButtonStyle.Primary, emote: Emoji.Parse(":arrow_right:")),
                    new ButtonBuilder("Refresh", $"character:move:worldmap:{centerX}:{centerY}:{zoom}", ButtonStyle.Secondary, emote: Emoji.Parse(":arrows_counterclockwise:")),
                    new ButtonBuilder("Back", "character:move", ButtonStyle.Secondary, emote: Emoji.Parse(":arrow_backward:")),
                ])
                .Build();
        });
    }

    [ComponentInteraction("character:move:worldmap2:*:*:*:*")]
    public async Task CharacterWorldmapMove(int centerX, int centerY, int zoom, string newTile)
    {
        var character = gameService.GetCharacter(Context.User.Id);
        var map = character.Location.MapInstance.Map;
        if (map.MapImage == null)
            return;

        var eightX = map.MapImage.Width / zoom / 8;
        var eightY = map.MapImage.Height / zoom / 8;
        Console.WriteLine($"EightX: {eightX}");


        //-3, -1, 1, 3
        var y = ("ABCD".IndexOf(newTile[0]) * 2 - 3);
        var x = ("1234".IndexOf(newTile[1]) * 2 - 3);

        var newCenterX = centerX + eightX * x;
        var newCenterY = centerY + eightY * y;
        var newZoom = zoom * 4;

        //if (eightX < 100)
        //{
        //    int xOffset = map.MapImage.Width / 2 + newCenterX;
        //    int yOffset = map.MapImage.Height / 2 + newCenterY;
        //    int sizeX = map.MapImage.Width / newZoom;
        //    int sizeY = map.MapImage.Height / newZoom;
        //    var rect = new Rectangle(xOffset - (map.MapImage.Width / 2) / zoom, yOffset - (map.MapImage.Height / 2) / zoom, sizeX, sizeY);
        //    Console.WriteLine($"Rect: {rect}");
        //    var rect2 = new Rectangle(rect.X / map.MapImageSize, rect.Y / map.MapImageSize, sizeX / map.MapImageSize, sizeY / map.MapImageSize);
        //    Console.WriteLine($"=> Rect2: {rect2}");

        //    int X = Random.Shared.Next(rect2.X, rect2.X + rect2.Width);
        //    int Y = Random.Shared.Next(rect2.Y, rect2.Y + rect2.Height);
        //    while (!map[X, Y].HasFlag(CellType.Walkable))
        //    {
        //        X = Random.Shared.Next(rect2.X, rect2.X + rect2.Width);
        //        Y = Random.Shared.Next(rect2.Y, rect2.Y + rect2.Height);
        //    }
        //    Console.WriteLine($"Moving from {character.Location.X}, {character.Location.Y} to {X}, {Y}");

        //    character.WalkTo(new Location(X, Y, character.Location));

        //    //await CharacterMove();
        //    await CharacterWorldmap(centerX, centerY, zoom);
        //    return;
        //}

        await CharacterWorldmap(newCenterX, newCenterY, newZoom);
    }


    [ComponentInteraction("character:move:worldmapmove:*:*:*")]
    public async Task CharacterWorldmapDoMove(int centerX, int centerY, int zoom)
    {
        var character = gameService.GetCharacter(Context.User.Id);
        var map = character.Location.MapInstance.Map;
        if (map.MapImage == null)
            return;


        int xOffset = map.MapImage.Width / 2 + centerX;
        int yOffset = map.MapImage.Height / 2 + centerY;
        int sizeX = map.MapImage.Width / zoom;
        int sizeY = map.MapImage.Height / zoom;
        var rect = new Rectangle(xOffset - (map.MapImage.Width / 2) / zoom, yOffset - (map.MapImage.Height / 2) / zoom, sizeX, sizeY);
        Console.WriteLine($"Rect: {rect}");
        var rect2 = new Rectangle(rect.X / map.MapImageSize, rect.Y / map.MapImageSize, sizeX / map.MapImageSize, sizeY / map.MapImageSize);
        Console.WriteLine($"=> Rect2: {rect2}");

        int X = Random.Shared.Next(rect2.X, rect2.X + rect2.Width);
        int Y = Random.Shared.Next(rect2.Y, rect2.Y + rect2.Height);
        int i = 0;
        while (!map[X, Y].HasFlag(CellType.Walkable))
        {
            X = Random.Shared.Next(rect2.X, rect2.X + rect2.Width+1);
            Y = Random.Shared.Next(rect2.Y, rect2.Y + rect2.Height+1);
            if(i++ > 100)
            {
                await CharacterWorldmap(centerX, centerY, zoom);
                return;
            }
        }
        Console.WriteLine($"Moving from {character.Location.X}, {character.Location.Y} to {X}, {Y}");

        character.WalkTo(new Location(X, Y, character.Location));

        //await CharacterMove();
        await CharacterWorldmap(centerX, centerY, zoom);
    }





    [ComponentInteraction("character:move:walk")]
    public async Task CharacterWalk()
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        using var mapStream = mapGenerator.GenerateMapImage(character, 512, 1).AsPngStream();
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



    [ComponentInteraction("character:move:walk:*")]
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
