﻿using IdleRpg.Game;
using IdleRpg.Game.Core;
using MemoryPack;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyRpg.Npcs.Mobs;

public abstract class Mob : INpcTemplate
{
    public abstract Enum Id { get; }
    public abstract string Name { get; }
    public abstract Dictionary<Enum, int> Stats { get; }
    public abstract List<ItemDrop> ItemDrops { get; }
    public abstract int AgroRange { get; }
    public Image<Rgba32>? Image { get; set; }
    public abstract string ImageFile { get; }
}
