namespace TinyRpg.Items.Misc;

using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using TinyRpg.Core;

public class BunnyTail : IItemTemplate
{
    public Enum Id => ItemIds.BunnyTail;
    public string Name => "Bunny's Tail";

    public string Description => "The fluffy tail of a poor bunny";

    public Image<Rgba32>? InventoryImage { get; set; }

    public string ImageFile => "BunnyTail.png";
    public Dictionary<Enum, int> Value => new() { { Stats.Money, 5 } };
}
