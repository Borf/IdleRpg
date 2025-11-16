namespace TinyRpg.Items.Misc;

using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using TinyRpg.Core;

public class OwlFeather : IItemTemplate
{
    public Enum Id => ItemIds.OwlFeather;
    public string Name => "Owl Feather";

    public string Description => "The feather of an owl. It falls without a sound";

    public Image<Rgba32>? InventoryImage { get; set; }

    public string ImageFile => "OwlFeather.png";
    public Dictionary<Enum, int> Value => new() { { Stats.Money, 5 } };
}
