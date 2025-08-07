using MemoryPack;
using MemoryPack.Compression;
using System;
using System.Diagnostics;

namespace IdleRpg.Game.Core;

[MemoryPackable(GenerateType.VersionTolerant)]
public partial class Map_v1_1
{
    [MemoryPackOrder(0)]
    public short Version { get; set; } = 0x0101;
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

[MemoryPackable(GenerateType.VersionTolerant)]
public partial class Map : Map_v1_1
{
    [MemoryPackIgnore]
    public string Name { get; private set; } = string.Empty;
    public static Map Load(string name)
    {
        var filename = Path.Combine("Resources", "Games", GameService.CoreName, "Maps", $"{name}.map");
        var data = File.ReadAllBytes(filename);
        using var decompressor = new BrotliDecompressor();

        Debug.Assert(data[0] == 'M');
        Debug.Assert(data[1] == 'A');
        Debug.Assert(data[2] == 'P');
        ushort version = BitConverter.ToUInt16(data[3..5]);
        Debug.Assert(version == 0x0101, $"Unsupported map version: {version}");

        var decompressedBuffer = decompressor.Decompress(data[5..]);
        var map = MemoryPackSerializer.Deserialize<Map>(decompressedBuffer)!;
        map.Name = name;
        return map;
    }
}

