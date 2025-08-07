#!/usr/bin/dotnet run
#:package SimplexNoise@2.0.0
#:package MemoryPack@1.21.4

using SimplexNoise;
using MemoryPack;
using MemoryPack.Compression;
using System;

int width = 1000;
int height = 1000;
Noise.Seed = 0;
float threshold = 0.45f;
string name = "WorldMap";

string fileName = $"{name}.map";



Console.WriteLine("Generating noise");
float[,] values = Noise.Calc2D(width, height, 0.1f);
float[,] values2 = Noise.Calc2D(width, height, 5.0f);
for (int y = 0; y < height; y++)
    for (int x = 0; x < width; x++)
        values[x,y] = 0.7f * (values[x, y] / 255) + 0.3f * (values2[x, y] / 255);
bool[,] walkable = new bool[width, height];
for (int y = 0; y < height; y++)
    for (int x = 0; x < width; x++)
        walkable[x, y] = values[x, y] > threshold;

if (width <= 200 && height <= 200)
{
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
            Console.Write(walkable[x, y] ? "#" : " ");
        Console.WriteLine();
    }
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();
}

Console.WriteLine("Processing noise");

(int x, int y)[] offsets = new (int, int)[]{(-1, 0), (1,0), (0, -1), (0, 1)};
for (int i = 0; i < 2; i++)
{
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            if(!walkable[x, y])
                continue;
            List<(int x, int y)> connectedPoints = new List<(int x, int y)>();
            List<(int x, int y)> donePoints = new List<(int x, int y)>();
            connectedPoints.Add((x, y));
            while(connectedPoints.Count < 100)
            {
                if (!connectedPoints.Any(p => !donePoints.Contains(p)))
                    break;
                var point = connectedPoints.FirstOrDefault(p => !donePoints.Contains(p));
                for (int o = 0; o < offsets.Length; o++)
                {
                    var offset = offsets[o];
                    if (point.x + offset.x >= 0 && point.x + offset.x < width && point.y + offset.y >= 0 && point.y + offset.y < height)
                    {
                        if (walkable[point.x + offset.x, point.y + offset.y] && !connectedPoints.Contains((point.x + offset.x, point.y + offset.y)))
                        {
                            connectedPoints.Add((point.x + offset.x, point.y + offset.y));
                        }
                    }
                }
                donePoints.Add(point);
            }
            if (connectedPoints.Count < 20)
            {
             //   Console.WriteLine($"Removing at ({x}, {y}) ({connectedPoints.Count})");
                walkable[x, y] = false;
            }
            if (connectedPoints.Count >= 20 && connectedPoints.Count < 100)
            {
                for(int o = 0; o < offsets.Length; o++)
                {
                    var offset = offsets[o];
                    if (x + offset.x >= 0 && x + offset.x < width && y + offset.y >= 0 && y + offset.y < height)
                    {
                        if (!walkable[x + offset.x, y + offset.y])
                        {
                            walkable[x + offset.x, y + offset.y] = true;
                        }
                    }
                }
            }
        }
    }
}
Console.WriteLine("Inflating walkable area");

for (int i = 0; i < 2; i++)
{
    for(int x = 0; x < width; x++)
    {
        for(int y = 0; y < height; y++)
        {
            if (!walkable[x, y])
                continue;
            for (int o = 0; o < offsets.Length; o++)
            {
                var offset = offsets[o];
                if (x + offset.x >= 0 && x + offset.x < width && y + offset.y >= 0 && y + offset.y < height)
                {
                    walkable[x+offset.x, y+offset.y] = true;
                    break;
                }
            }
        }
    }
}
Console.WriteLine("Done");


for (int y = 0; y < height; y +=2)
{
    for (int x = 0; x < width; x++)
    {
        if(walkable[x, y] && walkable[x, y+1])
            Console.Write("█");
        else if (walkable[x, y])
            Console.Write("▀");
        else if (walkable[x, y + 1])
            Console.Write("▄");
        else
            Console.Write(" ");
    }
    Console.WriteLine();
}


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
        if (walkable[x, y])
            map.CellType[x, y] = CellType.Walkable;
        else
            map.CellType[x, y] = CellType.NotWalkable;
    }
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
    Walkable = 1<<0,
    NotWalkable = 1<<2,
    Water = 1<<3,
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