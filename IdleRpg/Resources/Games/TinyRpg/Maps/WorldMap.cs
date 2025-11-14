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
        LoadMobs<Npcs.NpcIds>();

        Spawns.Add(new SpawnTemplate()
        {
            Position = new Point(20, 20),
            Amount = 5,
            MobId = Npcs.NpcIds.Slime,
            RangeX = 5,
            RangeY = 5,
            RespawnTime = TimeSpan.FromSeconds(5),
            SpawnType = SpawnType.Sphere
        });
        Spawns.Add(new SpawnTemplate()
        {
            Position = new Point(746, 707),
            Amount = 5,
            MobId = Npcs.NpcIds.Slime,
            RangeX = 3,
            RangeY = 3,
            RespawnTime = TimeSpan.FromSeconds(5),
            SpawnType = SpawnType.Rect
        });
        Spawns.Add(new SpawnTemplate()
        {
            Position = new Point(795, 717),
            Amount = 30,
            MobId = Npcs.NpcIds.Slime,
            RangeX = 20,
            RangeY = 20,
            RespawnTime = TimeSpan.FromSeconds(5),
            SpawnType = SpawnType.Rect
        });


        Spawns.Add(new SpawnTemplate()
        {
            Position = new Point(739, 731),
            MobId = Npcs.NpcIds.NpcStarterVillageMerchant,
        });
    }
}
