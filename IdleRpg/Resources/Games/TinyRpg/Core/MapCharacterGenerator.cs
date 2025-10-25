using IdleRpg.Game;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace TinyRpg.Core;

public class MapCharacterGenerator : IImageGenerator<Character, SpriteDirection>
{
    //can not do DI for now
    public Image<Rgba32> GetImage(Character source, SpriteDirection direction)
    {
        Image<Rgba32> image = new Image<Rgba32>(16, 16);

        {
            using var body = Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "CharacterArt", "Body", "Body.png"));
            body.Mutate(ip => ip.Crop(new Rectangle(0,0,16,16)));
            image.Mutate(ip => ip.DrawImage(body, 1.0f));
        }

        {
            using var body = Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "CharacterArt", "Face", "2.png"));
            body.Mutate(ip => ip.Crop(new Rectangle(0, 0, 16, 16)));
            image.Mutate(ip => ip.DrawImage(body, 1.0f));
        }

        {
            using var body = Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "CharacterArt", "Hair", "1.png"));
            body.Mutate(ip => ip.Crop(new Rectangle(0, 0, 16, 16)));
            image.Mutate(ip => ip.DrawImage(body, 1.0f));
        }

        {
            using var body = Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "CharacterArt", "Shirt", "1.png"));
            body.Mutate(ip => ip.Crop(new Rectangle(0, 0, 16, 16)));
            image.Mutate(ip => ip.DrawImage(body, 1.0f));
        }

        {
            using var body = Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "CharacterArt", "Pants", "1.png"));
            body.Mutate(ip => ip.Crop(new Rectangle(0, 0, 16, 16)));
            image.Mutate(ip => ip.DrawImage(body, 1.0f));
        }

        return image;
    }
}