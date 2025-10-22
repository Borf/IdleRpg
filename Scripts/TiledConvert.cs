#!/usr/bin/dotnet run
#:property PublishAot=false
#:package MemoryPack@1.21.4
#:package SixLabors.ImageSharp@3.1.11
#:package SixLabors.ImageSharp.Drawing@2.1.7

using MemoryPack;
using MemoryPack.Compression;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


string jsonFilePath = "../IdleRpg/Resources/Games/TinyRpg/Maps/Worldmap.tmj";
string fileName = "../IdleRpg/Resources/Games/TinyRpg/Maps/Worldmap.map";
string collisionFileName = "../IdleRpg/Resources/Games/TinyRpg/Maps/Worldmap.collision.png";
string mapImageFile = "../IdleRpg/Resources/Games/TinyRpg/Maps/Worldmap.png";
string mapImagePath = "../IdleRpg/Resources/Games/TinyRpg/Maps/Worldmap/";
int tileSize = 1024;

var json = JsonSerializer.Deserialize<TiledMap>(File.ReadAllText(jsonFilePath))!;

int minX = json.layers.Where(l => l.type == "tilelayer").Min(l => l.chunks.Min(c => c.x));
int maxX = json.layers.Where(l => l.type == "tilelayer").Max(l => l.chunks.Max(c => c.x + c.width));

int minY = json.layers.Where(l => l.type == "tilelayer").Min(l => l.chunks.Min(c => c.y));
int maxY = json.layers.Where(l => l.type == "tilelayer").Max(l => l.chunks.Max(c => c.y + c.height));

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
    for (int x = 0; x < width; x++)
        map.CellType[x, y] = CellType.None;

foreach (var layer in json.layers)
{
    foreach(var chunk in layer.chunks)
    {
        for(int x = chunk.x; x < chunk.x + chunk.width; x++)
        {
            for(int y = chunk.y ; y < chunk.y + chunk.height; y++)
            {
                int localX = x - chunk.x;
                int localY = y - chunk.y;
                int index = localY * chunk.width + localX;
                long tileId = chunk.data[index];
                if(tileId != 0)
                {
                    var tile = json.tilesets.SelectMany(ts => ts.tiles).FirstOrDefault(t => t.id + json.tilesets[0].firstgid == tileId);
                    if(tile != null && tile.properties != null)
                    {
                        foreach(var prop in tile.properties)
                        {
                            if(prop.name == "blocking" && prop.type == "bool" && prop.value)
                            {
                                int mapX = x - minX;
                                int mapY = y - minY;
                                map.CellType[mapX, mapY] |= CellType.NotWalkable;
                            }
                        }
                    }
                }
            }
        }
    }
}

Console.WriteLine("Fixing walkability");
for (int y = 0; y < height; y++)
    for (int x = 0; x < width; x++)
        if (!map.CellType[x, y].HasFlag(CellType.NotWalkable))
            map.CellType[x, y] |= CellType.Walkable;


Console.WriteLine("Writing collisionmap debug image");
Image<Rgba32> collisionMap = new Image<Rgba32>(width, height);
for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        if (map.CellType[x, y].HasFlag(CellType.NotWalkable))
            collisionMap[x, y] = new Rgba32(255, 0, 0, 255);
        else
            collisionMap[x, y] = new Rgba32(0, 255, 255, 255);
    }
}
collisionMap.SaveAsPng(collisionFileName);
Console.WriteLine("Zooming and splitting map");
Configuration.Default.MemoryAllocator = MemoryAllocator.Create(new MemoryAllocatorOptions
{
    AllocationLimitMegabytes = 7000
});
using Image<Rgba32> mapImage = Image.Load<Rgba32>(mapImageFile);


if(Directory.Exists(mapImagePath))
    Directory.Delete(mapImagePath, true);
Directory.CreateDirectory(mapImagePath);

int zoom = 0;
while (mapImage.Width > tileSize || mapImage.Height > tileSize)
{
    if (zoom != 0)
    {
        Console.WriteLine($"Zooming to level {zoom}");
        int newWidth = mapImage.Width / 2;
        int newHeight = mapImage.Height / 2;
        mapImage.Mutate(ctx => ctx.Resize(newWidth, newHeight));
    }
    Console.WriteLine($"Generating zoom level {zoom} with size {mapImage.Width}x{mapImage.Height}");
    Directory.CreateDirectory(Path.Combine(mapImagePath, zoom.ToString()));
    for (int x = 0; x < mapImage.Width; x += tileSize)
    {
        for (int y = 0; y < mapImage.Height; y += tileSize)
        {
            int xx = x / tileSize;
            int yy = y / tileSize;

            using var subImage = mapImage.Clone(ctx => ctx.Crop(new Rectangle(
                x, 
                y, 
                Math.Min(tileSize, mapImage.Width - x), 
                Math.Min(tileSize, mapImage.Height - y)
            )));
            subImage.SaveAsPng(Path.Combine(mapImagePath, zoom.ToString(), $"map_{yy}_{xx}.png"));
        }
    }
    zoom++;
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
    public CellType[,] CellType { get; set; } = new CellType[0, 0];
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
    public string name { get; set; } = string.Empty;
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
    public List<long> data { get; set; } = new();
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
    public long id { get; set; }
    public List<Property1> properties { get; set; } = new();
    public Objectgroup? objectgroup { get; set; }
}

public class Objectgroup
{
    public string draworder { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public List<Object> objects { get; set; } = new();
    public int opacity { get; set; }
    public string type { get; set; } = string.Empty;
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
