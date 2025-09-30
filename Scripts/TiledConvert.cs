#!/usr/bin/dotnet run
#:property PublishAot=false
#:package MemoryPack@1.21.4

using MemoryPack;
using MemoryPack.Compression;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;

string jsonFilePath = "../IdleRpg/Resources/Games/TinyRpg/Maps/Worldmap.tmj";
string fileName = "../IdleRpg/Resources/Games/TinyRpg/Maps/Worldmap_.map";

var json = JsonSerializer.Deserialize<TiledMap>(File.ReadAllText(jsonFilePath));

int minX = json.layers.Min(l => l.chunks.Min(c => c.x));
int maxX = json.layers.Max(l => l.chunks.Max(c => c.x + c.width));

int minY = json.layers.Min(l => l.chunks.Min(c => c.y));
int maxY = json.layers.Max(l => l.chunks.Max(c => c.y + c.height));

Console.WriteLine($"Map bounds: ({minX}, {minY}) to ({maxX}, {maxY})");

int width = maxX - minX;
int height = maxY - minY;

var map = new Map_v1_1
{
    Width = width,
    Height = height,
    CellType = new CellType[width, height]
};
for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        map.CellType[x, y] |= CellType.None;
        foreach (var layer in json.layers)
        {
            foreach(var chunk in layer.chunks)
            {
                if(x + minX >= chunk.x && x + minX < chunk.x + chunk.width &&
                   y + minY >= chunk.y && y + minY < chunk.y + chunk.height)
                {
                    int localX = (x + minX) - chunk.x;
                    int localY = (y + minY) - chunk.y;
                    int index = localY * chunk.width + localX;
                    int tileId = chunk.data[index];
                    if(tileId != 0)
                    {
                        var tile = json.tilesets.SelectMany(ts => ts.tiles).FirstOrDefault(t => t.id + json.tilesets[0].firstgid == tileId);
                        if(tile != null && tile.properties != null)
                        {
                            foreach(var prop in tile.properties)
                            {
                                if(prop.name == "blocking" && prop.type == "bool" && prop.value)
                                {
                                    map.CellType[x, y] |= CellType.NotWalkable;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (!map.CellType[x, y].HasFlag(CellType.NotWalkable))
            map.CellType[x, y] |= CellType.Walkable;
    }
}

for (int y = 0; y < 50; y++)
{
    for (int x = 0; x < 180; x++)
    {
        if(map.CellType[x, y].HasFlag(CellType.Walkable))
            Console.Write("·");
        else if (map.CellType[x, y].HasFlag(CellType.NotWalkable))
            Console.Write("█");
        else
            Console.Write(" ");
    }
    Console.WriteLine();
}


        using var compressor = new BrotliCompressor();
MemoryPackSerializer.Serialize(compressor, map);
File.WriteAllText(fileName, "MAP");
File.AppendAllBytes(fileName, [0x01, 0x01]);
File.AppendAllBytes(fileName, compressor.ToArray());


[Flags]
public enum CellType
{
    None = 0,
    Walkable = 1 << 0,
    NotWalkable = 1 << 2,
    Water = 1 << 3,
}

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




public class TiledMap
{
    public int compressionlevel { get; set; }
    public int height { get; set; }
    public bool infinite { get; set; }
    public List<Layer> layers { get; set; } = new();
    public int nextlayerid { get; set; }
    public int nextobjectid { get; set; }
    public string orientation { get; set; } = string.Empty;
    public string renderorder { get; set; } = string.Empty;
    public string tiledversion { get; set; } = string.Empty;
    public int tileheight { get; set; }
    public List<Tileset> tilesets { get; set; } = new();
    public int tilewidth { get; set; }
    public string type { get; set; } = string.Empty;
    public string version { get; set; } = string.Empty;
    public int width { get; set; }
}

public class Layer
{
    public List<Chunk> chunks { get; set; } = new();
    public int height { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public int opacity { get; set; }
    public int startx { get; set; }
    public int starty { get; set; }
    public string type { get; set; } = string.Empty;
    public bool visible { get; set; }
    public int width { get; set; }
    public int x { get; set; }
    public int y { get; set; }
}

public class Chunk
{
    public List<int> data { get; set; }
    public int height { get; set; }
    public int width { get; set; }
    public int x { get; set; }
    public int y { get; set; }
}

public class Tileset
{
    public int columns { get; set; }
    public int firstgid { get; set; }
    public string image { get; set; } = string.Empty;
    public int imageheight { get; set; }
    public int imagewidth { get; set; }
    public int margin { get; set; }
    public string name { get; set; } = string.Empty;
    public int spacing { get; set; }
    public int tilecount { get; set; }
    public int tileheight { get; set; }
    public List<Tile> tiles { get; set; } = new();
    public int tilewidth { get; set; }
}

public class Tile
{
    public int id { get; set; }
    public List<Property1> properties { get; set; } = new();
    public Objectgroup? objectgroup { get; set; }
}

public class Objectgroup
{
    public string draworder { get; set; }
    public string name { get; set; }
    public List<Object> objects { get; set; } = new();
    public int opacity { get; set; }
    public string type { get; set; }
    public bool visible { get; set; }
    public int x { get; set; }
    public int y { get; set; }
}

public class Object
{
    public int height { get; set; }
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public int rotation { get; set; }
    public string type { get; set; } = string.Empty;
    public bool visible { get; set; }
    public int width { get; set; }
    public int x { get; set; }
    public int y { get; set; }
}

public class Property1
{
    public string name { get; set; } = string.Empty;
    public string type { get; set; } = string.Empty;
    public bool value { get; set; }
}
