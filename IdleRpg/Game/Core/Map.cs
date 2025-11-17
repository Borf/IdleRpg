using L1PathFinder;
using MemoryPack;
using MemoryPack.Compression;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.Text.Json;

namespace IdleRpg.Game.Core;

[MemoryPackable(GenerateType.VersionTolerant)]
public partial class Map_Base
{
    [MemoryPackOrder(0)]
    public short Version { get; set; }
}

[MemoryPackable(GenerateType.VersionTolerant)]
public partial class Map_v1_1 : Map_Base
{
    Map_v1_1() => Version = 0x0101;
    [MemoryPackOrder(1)]
    public int Width { get; set; }
    [MemoryPackOrder(2)]
    public int Height { get; set; }
    [MemoryPackOrder(10)]
    public CellType[,] CellType { get; set; } = new CellType[0,0];
}

[Flags]
public enum CellType
{
    None = 0,
    Walkable = 1 << 0,
    NotWalkable = 1 << 2,
    Water = 1 << 3,
}
public class Mobinfo
{
    public required string MobName { get; set; } = string.Empty;
    public required string Shape { get; set; } = string.Empty;
    public required int Amount { get; set; }
    public required int RespawnTime { get; set; }
    public required int X { get; set; }
    public required int Y { get; set; }
    public required int Width { get; set; }
    public required int Height { get; set; }
}

public class Map(string name)
{
    public bool Loaded = false;
    public string Name { get; private set; } = name;
    private Map_Base MapData = null!;
    private Map_v1_1? MapData11 => MapData as Map_v1_1;
    public int MapImageTileSize { get; init; } = 16;
    public int MapImageBigTileSize { get; init; } = 1024;
    public int MapImageZoomLevels { get; private set; }
    public L1PathPlanner Planner { get; private set; } = null!;
    //PathFindData PathFindData { get; set; } = null!;
    public InstanceType InstanceType { get; set; } = InstanceType.NoInstance;
    public List<MonsterSpawnTemplate> Spawns { get; set; } = [];
    public List<INpcTemplate> NpcTemplates { get; set; } = [];
    public string MapImagePath { get; private set; } = string.Empty;
    public virtual void Load(GameService gameService) //TODO: should this be gameService parameter?
    {
        var filename = Path.Combine("Resources", "Games", GameService.CoreName, "Maps", $"{Name}.map");
        MapImagePath = Path.Combine("Resources", "Games", GameService.CoreName, "Maps", Name.ToLower());

        var data = File.ReadAllBytes(filename);
        using var decompressor = new BrotliDecompressor();

        Debug.Assert(data[0] == 'M');
        Debug.Assert(data[1] == 'A');
        Debug.Assert(data[2] == 'P');
        ushort version = BitConverter.ToUInt16(data[3..5]);
//        Debug.Assert(version == 0x0101, $"Unsupported map version: {version}");

        var decompressedBuffer = decompressor.Decompress(data[5..]);
        MapData = MemoryPackSerializer.Deserialize<Map_v1_1>(decompressedBuffer)!;
        Debug.Assert(version == MapData.Version, $"Unsupported map version: {version}");

        var grid = new int[Width, Height];
        Image<Rgba32> debugImage = new Image<Rgba32>(Width, Height);
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                grid[x, y] = MapData11!.CellType[x, y].HasFlag(CellType.Walkable) ? 0 : 1;
                debugImage[x, y] = grid[x, y] == 0 ? Color.White : Color.Red;
            }
        }
        debugImage.SaveAsPng(filename + ".collision.png");
        Planner = L1PathPlanner.CreatePlanner(grid);
        MapImageZoomLevels = (int)Math.Ceiling(Math.Log2(Math.Max(Width, Height) * MapImageTileSize / MapImageBigTileSize));

    }


    public void LoadMobs<T>() where T : struct, Enum
    {
        var mobJsonPath = Path.Combine("Resources", "Games", GameService.CoreName, "Maps", $"{Name}.mobs.json");
        if (File.Exists(mobJsonPath))
        {
            var mobData = JsonSerializer.Deserialize<List<Mobinfo>>(File.ReadAllText(mobJsonPath)) ?? [];

            foreach (var mob in mobData)
            {
                Spawns.Add(new MonsterSpawnTemplate()
                {
                    Position = new Util.Point(mob.X, mob.Y),
                    Amount = mob.Amount,
                    MobId = Enum.Parse<T>(mob.MobName, true),
                    RangeX = mob.Width,
                    RangeY = mob.Height,
                    RespawnTime = TimeSpan.FromSeconds(mob.RespawnTime),
                    SpawnType = SpawnType.Rect
                });
            }
        }

    }

    public void LoadNpcs(GameService gameService)
    {
        NpcTemplates = gameService.NpcTemplates.Where(npc => npc.Map.IsEquivalentTo(this.GetType())).ToList(); // should this be isEquivalentTo as that relates to COM?

    }

    public CellType this[int X, int Y]
    {
        get
        {
            if (MapData11 == null)
                throw new InvalidOperationException("Map data is not loaded or is of an unsupported version.");
            if (X < 0 || Y < 0 || X >= MapData11.Width || Y >= MapData11.Height)
                return CellType.NotWalkable; // out of bounds
            return MapData11.CellType[X, Y];
        }
        set
        {
            if (MapData11 == null)
                throw new InvalidOperationException("Map data is not loaded or is of an unsupported version.");
            if (X < 0 || Y < 0 || X >= MapData11.Width || Y >= MapData11.Height)
                throw new ArgumentOutOfRangeException(X + ", " + Y, "Position is out of bounds.");
            MapData11.CellType[X, Y] = value;
        }
    }

    public CellType this[Util.Point pos]
    {
        get
        {
            if (MapData11 == null)
                throw new InvalidOperationException("Map data is not loaded or is of an unsupported version.");
            if (pos.X < 0 || pos.Y < 0 || pos.X >= MapData11.Width || pos.Y >= MapData11.Height)
                return CellType.NotWalkable; // out of bounds
            return MapData11.CellType[pos.X, pos.Y];
        }
        set
        {
            if (MapData11 == null)
                throw new InvalidOperationException("Map data is not loaded or is of an unsupported version.");
            if (pos.X < 0 || pos.Y < 0 || pos.X >= MapData11.Width || pos.Y >= MapData11.Height)
                throw new ArgumentOutOfRangeException(nameof(pos), "Position is out of bounds.");
            MapData11.CellType[pos.X, pos.Y] = value;
        }
    }

    public int Width => MapData11?.Width ?? throw new InvalidOperationException("Map data is not loaded or is of an unsupported version.");
    public int Height => MapData11?.Height ?? throw new InvalidOperationException("Map data is not loaded or is of an unsupported version.");

    public List<MapInstance> MapInstances { get; private set; } = new();
    public MapInstance MapInstance(IGameCore gameCore, IServiceProvider serviceProvider) 
    { 
        while(!Loaded)
            Thread.Sleep(100);
        if (InstanceType == InstanceType.NoInstance)
        {
            if (MapInstances.Count == 0)
                MapInstances.Add(NewInstance(gameCore, serviceProvider));
            return MapInstances.First();
        }
        throw new NotImplementedException();
    }

    public virtual MapInstance NewInstance(IGameCore gameCore, IServiceProvider serviceProvider)
    { 
        var instance = new MapInstance()
        {
            Map = this,
        };
        //TODO: this might take longer later, so put it in a b ackground thread
        instance.LoadMonsterSpawners(gameCore, serviceProvider);
        foreach(var template in NpcTemplates)
        {
            instance.Characters.Add(new CharacterNpc(serviceProvider, template, instance)); //TODO: passing instance here feels a bit dirty
        }

        return instance;
    }

}

public enum InstanceType
{
    NoInstance,
    SinglePersonInstance,
    ChannelInstance,
    PartyInstance,
    GuildInstance,
    FixedSizeInstance,
}