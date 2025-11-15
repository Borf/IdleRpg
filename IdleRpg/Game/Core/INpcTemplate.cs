using IdleRpg.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Point = IdleRpg.Util.Point;

namespace IdleRpg.Game.Core;

public interface INpcTemplate
{
    Point Position { get; }
    Type Map { get; }
    string Name { get; }
    Image<Rgba32>? Image { get; set; }
    string ImageFile { get; }
}
