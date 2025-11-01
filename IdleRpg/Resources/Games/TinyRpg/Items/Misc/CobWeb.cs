namespace TinyRpg.Items.Misc;

using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;

public class CobWeb : IItemTemplate
{
    public Enum Id => ItemIds.CobWeb;
    public string Name => "CobWeb";

    public string Description => "A piece of cobweb. It is very sticky";

    public Image<Rgba32>? InventoryImage { get; set; }

    public string ImageFile => "CobWeb.png";
}
