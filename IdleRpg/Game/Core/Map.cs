namespace IdleRpg.Game.Core;

public class Map
{
    public string Name { get; set; } = string.Empty;
    public int MinX { get; set; } = 0;
    public int MinY { get; set; } = 0;
    public int MaxX { get; set; } = 0;
    public int MaxY { get; set; } = 0;
    
    public TileType[,] Tiles { get; set; } = new TileType[0, 0];

    public void Load() 
    {
        MinX = -64;
        MinY = -64;
        MaxX = 64;
        MaxY = 64;
        Tiles = new TileType[128, 128];
    }
    public TileType this[int x, int y] => Tiles[x - MinX, y - MinY];


    //TODO
    //method for pathfinding (dynamic blocking?)
}

[Flags]
public enum TileType
{
    NotWalkable = 1<<0,
    Walkable = 1<<1,
    //TODO: for more advanced walking paths?
    BlockLeft = 1 << 6,
    BlockRight = 1 << 7,
    BlockUp = 1 << 8,
    BlockDown = 1 << 9,
}