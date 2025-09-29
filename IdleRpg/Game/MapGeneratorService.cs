using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace IdleRpg.Game;

public class MapGeneratorService
{
    public Image<Rgba32> GenerateMapImage(Character character, int size, int scale)
    {
        var map = character.Location.MapInstance.Map;

        Image<Rgba32> image = new Image<Rgba32>(size, size);
        var characters = character.Location.MapInstance.GetCharactersAround(character.Location, 100).Where(c => c != character);
        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                int xx = x/scale - image.Width/2/scale + character.Location.X;
                int yy = y/scale - image.Height/2/scale + character.Location.Y;
                Rgba32 color = new Rgba32(0, 0, 0, 0);
                if (xx < 0 || xx >= map.Width || yy < 0 || yy >= map.Width)
                    color = new Rgba32(0, 0, 0, 255);
                else if (xx == character.Location.X && yy == character.Location.Y)
                    color = new Rgba32(0, 255, 0, 255);
                else if (characters.Any(c => c.Location.X == xx && c.Location.Y == yy && c != character))
                    color = new Rgba32(255, 0, 0, 255);
                else if (map[xx, yy].HasFlag(Game.Core.CellType.Walkable))
                    color = new Rgba32(10, 10, 10, 255);
                else if (!map[xx, yy].HasFlag(Game.Core.CellType.Walkable))
                    color = new Rgba32(255, 255, 255, 255);
                image[x, y] = color;
            }
        }
        return image;
    }


    public Image<Rgba32> GenerateWorldMapImage(Character character, int centerX, int centerY, int zoom)
    {
        var map = character.Location.MapInstance.Map;
        Image<Rgba32> image = new Image<Rgba32>(map.Width / zoom, map.Height / zoom);

        int xOffset = map.Width / 2 + centerX;
        int yOffset = map.Height / 2 + centerY;

        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                int xx = xOffset + x - image.Width/2;
                int yy = yOffset + y - image.Height/2;

                Rgba32 color = new Rgba32(0, 0, 0, 0);
                if (xx == character.Location.X && yy == character.Location.Y)
                    color = new Rgba32(0, 255, 0, 255);
                else if (map[xx, yy].HasFlag(Game.Core.CellType.Walkable))
                    color = new Rgba32(10, 10, 10, 255);
                else if (!map[xx, yy].HasFlag(Game.Core.CellType.Walkable))
                    color = new Rgba32(255, 255, 255, 255);
                image[x, y] = color;
            }
        }
        image.Mutate(p => p.Resize(256, 256));

        for (int i = 0; i <= 256; i += 64)
        {
            image.Mutate(p => p.DrawLine(Color.Red, 2, new PointF(i-0.5f, -0.5f), new PointF(i-0.5f, 255.5f)));
            image.Mutate(p => p.DrawLine(Color.Red, 2, new PointF(-0.5f, i-0.5f), new PointF(255.5f, i - 0.5f)));
        }

        return image;
    }
}
