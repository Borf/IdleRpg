using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace IdleRpg.Game.Core;

public interface IMonsterTemplate
{
    Enum Id { get; }
    string Name { get; }
    Dictionary<Enum, int> Stats { get; }
    List<ItemDrop> ItemDrops { get; }
    Image<Rgba32>? Image { get; set; }
    string ImageFile { get; }
}