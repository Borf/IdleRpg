namespace TinyRpg.Items.Misc;

using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using TinyRpg.Core;

public class FrogLeg : IItemTemplate
{
    public Enum Id => ItemIds.FrogLeg;
    public string Name => "Frog Leg";

    public string Description => "A leg of a frog. It looks strong";

    public Image<Rgba32>? InventoryImage { get; set; }

    public string ImageFile => "FrogLeg.png";
    public Dictionary<Enum, int> Value => new() { { Stats.Money, 5 } };
}
