using IdleRpg.Game.Core;
using System.Collections.Generic;
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TinyRpg.Npcs.Npcs.StarterArea;

public class Merchant : IMonsterTemplate
{
    public Enum Id => NpcIds.NpcStarterVillageMerchant;

    public string Name => "Merchant";

    public Dictionary<Enum, int> Stats => [];

    public List<ItemDrop> ItemDrops => [];

    public Image<Rgba32>? Image { get; set; }

    public string ImageFile => "Merchant.png";
}
