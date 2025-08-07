using IdleRpg.Game.Core;
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyRpg.Maps;

public class WorldMap : Map
{
    public WorldMap()
    {
        Load("WorldMap");
    }
}
