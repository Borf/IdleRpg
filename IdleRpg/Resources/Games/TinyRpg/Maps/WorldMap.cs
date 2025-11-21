using IdleRpg.Game;
using IdleRpg.Game.Core;
using IdleRpg.Util;
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

    
    public override void Load(GameService gameService)
    {
        base.Load(gameService);
        // start up maptasks

        // register how to instances work on this map

        // warp portals 'n stuff
        LoadMobs<Npcs.NpcIds>();
        LoadNpcs(gameService);

        MapLocations.Add(new()
        {
            Name = "Village",
            Position = new(740, 738),
            Region = "Starter",
            HasMerchant = true
        });
    }
}
