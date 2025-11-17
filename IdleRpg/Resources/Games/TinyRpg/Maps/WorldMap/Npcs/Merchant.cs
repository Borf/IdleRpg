using IdleRpg.Game.Core;
using System.Collections.Generic;
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TinyRpg.Items;

namespace TinyRpg.Maps.WorldMap.Npcs;

public class Merchant : INpcTemplate, INpcMerchant, INpcDialog
{
    public string Name => "Merchant";

    public IdleRpg.Util.Point Position => new(739, 731);

    public Type Map => typeof(WorldMap);

    public Image<Rgba32>? Image { get; set; }

    public string ImageFile => "Merchant.png";

    public List<MerchantItem>? MerchantItems => [ new() { ItemId = ItemIds.RedPotion, Price = 100 }];

    public NpcFeatures Features => NpcFeatures.Merchant | NpcFeatures.Dialog;

    public string Dialog => "Hello there, welcome to this village. Feel free to browse my merchandise. If you have any items, I'll gladly buy it from you";
}
