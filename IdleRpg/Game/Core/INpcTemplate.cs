using IdleRpg.Util;
using Newtonsoft.Json;
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
    NpcFeatures Features { get; } //TODO: can this be done with pure interfaces?
}

public interface INpcMerchant
{
    List<MerchantItem>? MerchantItems { get; }
}

public class MerchantItem
{
    public required Enum ItemId { get; init; }
    public required int Price { get; init; }
}

public interface INpcDialog
{
    public string Dialog { get; }
}

[Flags]
public enum NpcFeatures
{
    Merchant = 1<<0,
    Dialog = 1<<1,

}