namespace TinyRpg.Items.Usables;

using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;

public class RedPotion : IUsable
{
    public Enum Id => TinyRpg.Items.ItemIds.RedPotion;
    public string Name => "Red Potion";

    public string Description => "A small potion that heals 10 HP";

    public Image<Rgba32>? InventoryImage { get; set; }

    public string ImageFile => "RedPotion.png";
}
