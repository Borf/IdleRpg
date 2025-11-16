namespace TinyRpg.Items.Misc;

using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using TinyRpg.Core;

public class BunnyEar : IItemTemplate
{
    public Enum Id => ItemIds.BunnyEar;
    public string Name => "Bunny's Ear";

    public string Description => "An ear of a bunny";

    public Image<Rgba32>? InventoryImage { get; set; }

    public string ImageFile => "BunnyEar.png";
    public Dictionary<Enum, int> Value => new() { { Stats.Money, 5 } };
}
