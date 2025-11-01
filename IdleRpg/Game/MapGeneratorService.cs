using IdleRpg.Game.Core;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace IdleRpg.Game;

public class MapGeneratorService
{
    private Dictionary<Map, ImageCache> ImageCaches = new();

    private Font font;
    private GameService gameService;

    public MapGeneratorService(GameService gameService)
    {
        this.font = SystemFonts.CreateFont("Tahoma", 20);
        this.gameService = gameService;
    }

    public Image<Rgba32> GenerateMapImage(Character character, int size, int zoom)
    {
        var map = character.Location.MapInstance.Map;

        Rectangle rect = new Rectangle(
            (character.Location.X - size/2), 
            (character.Location.Y - size/2),
            size,
            size);

        var image = GenerateMapImage(map, rect, zoom);
        int localTileSize = Math.Max(1, map.MapImageTileSize / (zoom + 1));

        var characters = character.Location.MapInstance.GetCharactersAround(character.Location, 100);
        foreach (var c in characters)
        {
            if (c is CharacterNPC npc && !string.IsNullOrEmpty(npc.NpcTemplate.ImageFile) && npc.NpcTemplate.Image != null)
            {
                image.Mutate(ip => ip.DrawImage(npc.NpcTemplate.Image, new Point(
                    (c.Location.X - rect.X) * map.MapImageTileSize,  //TODO: zoom should be factored in here too
                    (c.Location.Y - rect.Y) * map.MapImageTileSize), 1.0f));
            }
            else if (c is CharacterPlayer player)
            {
                using var sprite = ((IGameCore2D)gameService.GameCore).MapCharacterGenerator.GetImage(player, SpriteDirection.Down);

                image.Mutate(ip => ip.DrawImage(sprite, new Point(
                    (c.Location.X - rect.X) * map.MapImageTileSize,  //TODO: zoom should be factored in here too
                    (c.Location.Y - rect.Y) * map.MapImageTileSize), 1.0f));
            }
        }
        return image;
    }



    public Image<Rgba32> GenerateMapImage(Map map, Rectangle rect, int zoom)
    {
        if (!ImageCaches.ContainsKey(map))
            ImageCaches[map] = new();

        var image = new Image<Rgba32>((rect.Width * map.MapImageTileSize) / (1 << zoom), (rect.Height * map.MapImageTileSize) / (1 << zoom));
        int tilesPerImage = (map.MapImageBigTileSize / map.MapImageTileSize) << zoom;

        int minX = rect.X / tilesPerImage;
        int minY = rect.Y / tilesPerImage;

        int xi = 0;
        int yi = 0;

        int fac = Math.Max(1, 16>>zoom);

        for(int x = minX; x <= (rect.X + rect.Width) / tilesPerImage; x++)
        {
            for(int y = minY; y <= (rect.Y + rect.Height) / tilesPerImage; y++)
            {
                int destX = (x * tilesPerImage - rect.X) * fac;
                int destY = (y * tilesPerImage - rect.Y) * fac;
                var cacheEntry = ImageCaches[map].Load(map, zoom, x, y);
                if (cacheEntry.Image != null)
                {
                    image.Mutate(ip => ip.DrawImage(cacheEntry.Image, new Point(destX, destY), 1.0f));
                }
                if(xi == 0)
                    yi++;
            }
            xi++;
        }

        //image.Mutate(ip => ip.DrawText($"Zoom: {zoom}, iterations {xi},{yi}", font, Color.Pink, new PointF(10, 10)));

        return image;
    }

    public Image<Rgba32> GenerateWorldMapImage(Character character, Rectangle rectangle)
    {
        var map = character.Location.MapInstance.Map;
        //0 => 1024/16 = 64 tiles wide
        //1 => 128
        //2 => 256
        //3 => 512
        //4 => 1024
        //5 => 2048
        //6 => 4048
        int tilesPerImage = map.MapImageBigTileSize / map.MapImageTileSize;
        int zoom = 0;
        while(rectangle.Width > tilesPerImage)
        {
            zoom++;
            tilesPerImage *= 2;
        }

        var image = GenerateMapImage(map, rectangle, zoom);

        int fac = 1;
        while (fac * image.Width < 512 || fac * image.Height < 512)
            fac *= 2;
        if(fac != 1)
            image.Mutate(p => p.Resize(image.Width*fac, image.Height*fac));


        for (int i = 0; i <= image.Width; i += image.Width / 4)
            image.Mutate(p => p.DrawLine(Color.Red, 2, new PointF(i - 0.5f, -0.5f), new PointF(i - 0.5f, image.Height)));
        for (int i = 0; i <= image.Height; i += image.Height / 4)
            image.Mutate(p => p.DrawLine(Color.Red, 2, new PointF(-0.5f, i - 0.5f), new PointF(image.Width, i - 0.5f)));

        return image;
    }
}


class ImageCache : List<ImageCacheEntry>
{
    public ImageCacheEntry Load(Map map, int zoom, int x, int y)
    {
        var entry = this.FirstOrDefault(e => e.Zoom == zoom && e.X == x && e.Y == y); //TODO: single equals? hash or bitcalc?
        if (entry != null)
            return entry;

        string tilePath = System.IO.Path.Combine(map.MapImagePath, zoom.ToString(), $"map_{y}_{x}.png");

        entry = new ImageCacheEntry()
        {
            X = x,
            Y = y,
            Zoom = zoom,
            Image = File.Exists(tilePath) ? Image.Load<Rgba32>(tilePath) : null
        };

        if(this.Count > 30)
            this.Clear(); //should only remove oldest
        this.Add(entry);
        return entry;
    }
}

class ImageCacheEntry
{
    public required int Zoom { get; set; }
    public required int X { get; set; }
    public required int Y { get; set; }
    public required Image<Rgba32>? Image { get; set; }
}