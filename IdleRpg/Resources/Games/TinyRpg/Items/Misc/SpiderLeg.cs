namespace TinyRpg.Items.Misc;

using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;

public class SpiderLeg : IItemTemplate
{
    public Enum Id => ItemIds.SpiderLeg;
    public string Name => "Spider Leg";

    public string Description => "One out of 8 legs of a spider";

    public Image<Rgba32>? InventoryImage { get; set; }

    public string ImageFile => "SpiderLeg.png";
}
