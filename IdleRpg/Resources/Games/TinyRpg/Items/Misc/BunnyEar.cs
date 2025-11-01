namespace TinyRpg.Items.Misc;

using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;

public class BunnyEar : IItemTemplate
{
    public Enum Id => ItemIds.BunnyEar;
    public string Name => "Bunny's Ear";

    public string Description => "An ear of a bunny";

    public Image<Rgba32>? InventoryImage { get; set; }

    public string ImageFile => "BunnyEar.png";
}
