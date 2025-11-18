using IdleRpg.Game;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace TinyRpg.Core;

public class CharacterGenerator : IImageGenerator<CharacterPlayer, SpriteDirection>, IImageGenerator<ICharacterCreateCharOptions>
{
    //can not do DI for now
    public Image<Rgba32> GetImage(CharacterPlayer source, SpriteDirection direction)
    {
        Image<Rgba32> image = new Image<Rgba32>(16, 16);

        {
            int bodyId = (int)source.Stats[Stats.LookBody];
            using var body = Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "CharacterArt", "Body", "Body.png"));
            body.Mutate(ip => ip.Crop(new Rectangle(0,0,16,16)));
            image.Mutate(ip => ip.DrawImage(body, 1.0f));
        }

        {
            int faceId = (int)source.Stats[Stats.LookFace];
            using var face = Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "CharacterArt", "Face", $"{faceId}.png"));
            face.Mutate(ip => ip.Crop(new Rectangle(0, 0, 16, 16)));
            image.Mutate(ip => ip.DrawImage(face, 1.0f));
        }

        {
            int hairId = (int)source.Stats[Stats.LookHair];
            using var hair = Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "CharacterArt", "Hair", $"{hairId}.png"));
            hair.Mutate(ip => ip.Crop(new Rectangle(0, 0, 16, 16)));
            image.Mutate(ip => ip.DrawImage(hair, 1.0f));
        }

        if(source.Stats.ContainsKey(Stats.LookShirt))
        {
            int shirtId = (int)source.Stats[Stats.LookShirt];
            if (shirtId > 0)
            {
                using var shirt = Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "CharacterArt", "Shirt", $"{shirtId}.png"));
                shirt.Mutate(ip => ip.Crop(new Rectangle(0, 0, 16, 16)));
                image.Mutate(ip => ip.DrawImage(shirt, 1.0f));
            }
        }

        if (source.Stats.ContainsKey(Stats.LookPants))
        {
            int pantsId = (int)source.Stats[Stats.LookPants];
            //if (pantsId > 0)
            {
                using var pants = Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "CharacterArt", "Pants", $"{pantsId}.png"));
                pants.Mutate(ip => ip.Crop(new Rectangle(0, 0, 16, 16)));
                image.Mutate(ip => ip.DrawImage(pants, 1.0f));
            }
        }

        return image;
    }

    public Image<Rgba32> GetImage(ICharacterCreateCharOptions options)
    {
        var charOptions = (CharacterCreateCharOptions)options;
        Image<Rgba32> image = new Image<Rgba32>(16, 16);

        {
            using var body = Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "CharacterArt", "Body", "Body.png"));
            body.Mutate(ip => ip.Crop(new Rectangle(0, 0, 16, 16)));
            image.Mutate(ip => ip.DrawImage(body, 1.0f));
        }

        {
            using var body = Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "CharacterArt", "Face", $"{charOptions.HeadId}.png"));
            body.Mutate(ip => ip.Crop(new Rectangle(0, 0, 16, 16)));
            image.Mutate(ip => ip.DrawImage(body, 1.0f));
        }

        {
            using var body = Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "CharacterArt", "Hair", $"{charOptions.HairId}.png"));
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