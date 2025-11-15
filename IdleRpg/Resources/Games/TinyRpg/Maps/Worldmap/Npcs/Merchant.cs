using IdleRpg.Game.Core;
using System.Collections.Generic;
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TinyRpg.Maps.Worldmap.Npcs;

public class Merchant : INpcTemplate
{
    public string Name => "Merchant";

    public IdleRpg.Util.Point Position => new(739, 731);

    public Type Map => typeof(WorldMap);

    public Image<Rgba32>? Image { get; set; }

    public string ImageFile => "Merchant.png";

}
