using IdleRpg.Game;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace TinyRpg.Core;

internal class DiscordHeaderGenerator : IImageGenerator<DiscordMenu, Character>
{
    public Image<Rgba32> GetImage(DiscordMenu source, Character options)
    {
        if (source == DiscordMenu.Main)
        {
            return Image.Load<Rgba32>(Path.Combine("Resources", "Games", "TinyRpg", "header2.png"));
        }
        return new Image<Rgba32>(1, 1);
    }
}