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
    public WorldMap() : base("WorldMap")
    {
        InstanceType = InstanceType.NoInstance;
    }

    
    public override void Load()
    {
        base.Load();
        // start up maptasks

        // register how to instances work on this map

        // warp portals 'n stuff
    }
}
