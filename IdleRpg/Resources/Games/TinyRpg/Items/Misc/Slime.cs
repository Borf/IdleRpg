namespace TinyRpg.Items.Misc;

using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using TinyRpg.Core;

public class Slime : IItemTemplate
{
    public Enum Id => ItemIds.Slime;
    public string Name => "Slime";

    public string Description => "A sticky piece of slime";

    public Image<Rgba32>? InventoryImage { get; set; }

    public string ImageFile => "Slime.png";
    public Dictionary<Enum, int> Value => new() { { Stats.Money, 5 } };
}
