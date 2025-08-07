using MemoryPack;
using MemoryPack.Compression;
using System;
using System.Diagnostics;

namespace IdleRpg.Game.Core;

[MemoryPackable(GenerateType.VersionTolerant)]
public partial class Map_Base
{
    [MemoryPackOrder(0)]
    public short Version { get; set; } = 0x0101;
}

[MemoryPackable(GenerateType.VersionTolerant)]
public partial class Map_v1_1 : Map_Base
{
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


public class Map
{
    public string Name { get; protected set; } = string.Empty;
    public Map_Base MapData = null!;
    public Map_v1_1? MapData11 => MapData as Map_v1_1;

    protected void Load(string name)
    {
        var filename = Path.Combine("Resources", "Games", GameService.CoreName, "Maps", $"{name}.map");
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
    }
}

