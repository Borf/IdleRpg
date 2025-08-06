using MemoryPack;
using MemoryPack.Compression;
using System;

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
    public CellType[,] CellType { get; set; }


}

[Flags]
public enum CellType
{
    None = 0,
    Walkable = 1 << 0,
    NotWalkable = 1 << 2,
    Water = 1 << 3,
}

public class Map : Map_v1_1
{
    public static Map Load(string name)
    {
        var filename = Path.Combine("Resources", "Games", GameService.CoreName, "Maps", $"{name}.map");
        var data = File.ReadAllBytes(filename);
        using var decompressor = new BrotliDecompressor();
        var decompressedBuffer = decompressor.Decompress(data[3..]);
        return MemoryPackSerializer.Deserialize<Map>(decompressedBuffer);
    }
}

