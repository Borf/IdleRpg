using IdleRpg.Game;
using IdleRpg.Game.Core;
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyRpg.Npcs.Mobs;

public abstract class Mob : INpc
{
    public abstract Enum Id { get; }
    public abstract string Name { get; }
    public abstract Dictionary<Enum, int> Stats { get; }
    public abstract List<ItemDrop> ItemDrops { get; }
}
