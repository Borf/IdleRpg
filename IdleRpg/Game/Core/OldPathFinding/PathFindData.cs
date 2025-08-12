using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IdleRpg.Game.Core.OldPathFinding;

//public class PathFindData
//{
//    public const int GridSize = 16; //the larger the grid, the more the gridcell's Directions array explodes ^4. Maybe add a flag to either use cached directions or not?
//    public GridCell[,] GridCells { get; }
//    public int GridWidth { get; }
//    public int GridHeight { get; }

//    public PathFindData(Map map)
//    {
//        //TODO: maybe store the map?
//        GridWidth = (int)Math.Ceiling(map.Width / (double)PathFindData.GridSize);
//        GridHeight = (int)Math.Ceiling(map.Height / (double)PathFindData.GridSize);
//        GridCells = new GridCell[GridWidth, GridHeight];
//        for (int x = 0; x < GridWidth; x++)
//            for (int y = 0; y < GridHeight; y++)
//                GridCells[x, y] = new GridCell(new Point(x * PathFindData.GridSize, y * PathFindData.GridSize));
//    }


//}

//public class GridCell(Point globalPosition)
//{
//    public Point GlobalPosition { get; set; } = globalPosition; // topleft of the cell
//    public Point ArrayPosition => GlobalPosition / PathFindData.GridSize; // position in the grid array, used for quick access
//    public List<Link> Links = new List<Link>(); // list of directions and then links per direction
//    public Dictionary<Link, int>[,] DistanceToLinks = new Dictionary<Link, int>[PathFindData.GridSize, PathFindData.GridSize]; // gridsize x gridsize with a distance to every link to find the closest link quickly
//    public Direction[,,,] Directions = new Direction[PathFindData.GridSize, PathFindData.GridSize, PathFindData.GridSize, PathFindData.GridSize]; // gridsize x gridsize, then target cell reachability. basically a huge lookup table for directions. This takes up a huge amount of memory, and could be live-computed using A*
//}
//public class Link(Point point)
//{
//    public Point Point { get; set; } = point;
//    public List<Point> Targets { get; set; } = new();
//}

//public enum Direction : byte
//{
//    Up = 0,
//    Right = 1,
//    Down = 2,
//    Left = 3,
//    Unreachable = 4
//}

//public static class DirectionHelper
//{
//    public static readonly Point[] Offsets = new Point[]
//    {
//        new Point(0, -1), // Up
//        new Point(1, 0),  // Right
//        new Point(0, 1),  // Down
//        new Point(-1, 0)  // Left
//    };
//    public static Point Offset(this Direction direction)
//    {
//        return Offsets[(int)direction];
//    }
//}