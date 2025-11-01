using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace IdleRpg.Game.Core;

public interface IItemTemplate
{
    Enum Id { get; }
    string Name { get; }
    string Description { get; }
    Image<Rgba32>? InventoryImage { get; set;  }
    string ImageFile { get; }
}
