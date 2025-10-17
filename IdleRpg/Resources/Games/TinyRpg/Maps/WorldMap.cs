using IdleRpg.Game;
using IdleRpg.Game.Core;
using IdleRpg.Util;
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyRpg.Npcs.Mobs;

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

        Spawns.Add(new SpawnTemplate()
        {
            Position = new Point(20, 20),
            Amount = 5,
            Mob = Npcs.NpcIds.Slime,
            Range = 5,
            RespawnTime = TimeSpan.FromSeconds(5),
            SpawnType = SpawnType.Sphere
        });
    }
}
