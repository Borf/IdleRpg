using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace IdleRpg.Game;

public class MapGeneratorService
{
    public Image<Rgba32> GenerateMapImage(Character character, int size, int scale)
    {
        var map = character.Location.MapInstance.Map;
        if (map.MapImage == null)
            return new Image<Rgba32>(0, 0);

        int mapImageCenterX = character.Location.X * map.MapImageSize;
        int mapImageCenterY = character.Location.Y * map.MapImageSize;

        int mapImageSize = size * scale;

        int mapImageX = mapImageCenterX - mapImageSize / 2;
        int mapImageY = mapImageCenterY - mapImageSize / 2;

        int cappedImageX = Math.Max(0, mapImageX);
        int cappedImageY = Math.Max(0, mapImageY);

        int mapImageSizeX = mapImageSize - (cappedImageX - mapImageX);
        int mapImageSizeY = mapImageSize - (cappedImageY - mapImageY);
        //TODO: border on the right side

        var scaled = map.MapImage.Clone(ip =>
        {
            ip.Crop(new Rectangle(cappedImageX, cappedImageY, mapImageSizeX, mapImageSizeY));
        });


        int center = size / 2;
        Image<Rgba32> image = new Image<Rgba32>(size, size);
        image.Mutate(ip => ip.DrawImage(scaled, new Point((cappedImageX - mapImageX), (cappedImageY - mapImageY)), 1.0f));

        var characters = character.Location.MapInstance.GetCharactersAround(character.Location, 100).Where(c => c != character);
        image.Mutate(ip => ip.Fill(Color.Blue, new RectangleF(center, center, 16, 16)));

        foreach(var c in characters)
        {
            if(c is CharacterNPC npc && !string.IsNullOrEmpty(npc.NpcTemplate.ImageFile) && npc.NpcTemplate.Image != null)
            {
                image.Mutate(ip => ip.DrawImage(npc.NpcTemplate.Image, new Point(center + (c.Location.X - character.Location.X) * 16, center + (c.Location.Y - character.Location.Y) * 16), 1.0f));
            }
            else
                image.Mutate(ip => ip.Fill(Color.Red, new RectangleF(center + (c.Location.X - character.Location.X) * 16, center + (c.Location.Y - character.Location.Y) * 16, 16, 16)));
        }
        return image;
    }


    //public Image<Rgba32> GenerateWorldMapImage(Character character, int centerX, int centerY, int zoom)
    //{
    //    var map = character.Location.MapInstance.Map;
    //    Image<Rgba32> image = new Image<Rgba32>(map.Width / zoom, map.Height / zoom);

    //    int xOffset = map.Width / 2 + centerX;
    //    int yOffset = map.Height / 2 + centerY;

    //    for (var y = 0; y < image.Height; y++)
    //    {
    //        for (var x = 0; x < image.Width; x++)
    //        {
    //            int xx = xOffset + x - image.Width/2;
    //            int yy = yOffset + y - image.Height/2;

    //            Rgba32 color = new Rgba32(0, 0, 0, 0);
    //            if (xx == character.Location.X && yy == character.Location.Y)
    //                color = new Rgba32(0, 255, 0, 255);
    //            else if (map[xx, yy].HasFlag(Game.Core.CellType.Walkable))
    //                color = new Rgba32(10, 10, 10, 255);
    //            else if (!map[xx, yy].HasFlag(Game.Core.CellType.Walkable))
    //                color = new Rgba32(255, 255, 255, 255);
    //            image[x, y] = color;
    //        }
    //    }
    //    image.Mutate(p => p.Resize(256, 256));

    //    for (int i = 0; i <= 256; i += 64)
    //    {
    //        image.Mutate(p => p.DrawLine(Color.Red, 2, new PointF(i-0.5f, -0.5f), new PointF(i-0.5f, 255.5f)));
    //        image.Mutate(p => p.DrawLine(Color.Red, 2, new PointF(-0.5f, i-0.5f), new PointF(255.5f, i - 0.5f)));
    //    }

    //    return image;
    //}
    public Image<Rgba32> GenerateWorldMapImage(Character character, int centerX, int centerY, int zoom)
    {
        var map = character.Location.MapInstance.Map;
        if (map.MapImage == null)
            return new Image<Rgba32>(0, 0);

        Image<Rgba32> image = map.MapImage.Clone();

        int xOffset = image.Width / 2 + centerX;
        int yOffset = image.Height / 2 + centerY;

        int sizeX = image.Width / zoom;
        int sizeY = image.Height / zoom;

        image.Mutate(m => m.Crop(new Rectangle(
            xOffset - (image.Width / 2)/zoom, 
            yOffset - (image.Height / 2) / zoom, 
            sizeX, 
            sizeY)));




        //for (var y = 0; y < image.Height; y++)
        //{
        //    for (var x = 0; x < image.Width; x++)
        //    {
        //        int xx = xOffset + x - image.Width / 2;
        //        int yy = yOffset + y - image.Height / 2;

        //        Rgba32 color = new Rgba32(0, 0, 0, 0);
        //        if (xx == character.Location.X && yy == character.Location.Y)
        //            color = new Rgba32(0, 255, 0, 255);
        //        else if (map[xx, yy].HasFlag(Game.Core.CellType.Walkable))
        //            color = new Rgba32(10, 10, 10, 255);
        //        else if (!map[xx, yy].HasFlag(Game.Core.CellType.Walkable))
        //            color = new Rgba32(255, 255, 255, 255);
        //        image[x, y] = color;
        //    }
        //}
        image.Mutate(p => p.Resize(256, 256));

        for (int i = 0; i <= 256; i += 64)
        {
            image.Mutate(p => p.DrawLine(Color.Red, 2, new PointF(i - 0.5f, -0.5f), new PointF(i - 0.5f, 255.5f)));
            image.Mutate(p => p.DrawLine(Color.Red, 2, new PointF(-0.5f, i - 0.5f), new PointF(255.5f, i - 0.5f)));
        }

        return image;
    }
}
