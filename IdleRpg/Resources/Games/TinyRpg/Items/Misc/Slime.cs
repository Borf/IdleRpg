namespace TinyRpg.Items.Misc;

using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;

public class Slime : IItemTemplate
{
    public Enum Id => ItemIds.Slime;
    public string Name => "Slime";

    public string Description => "A sticky piece of slime";

    public Image<Rgba32>? InventoryImage { get; set; }

    public string ImageFile => "Slime.png";
}
