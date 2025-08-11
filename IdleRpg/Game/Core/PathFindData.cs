namespace IdleRpg.Game.Core;

public class PathFindData
{
    public const int GridSize = 32;
    public GridCell[,] GridCells;
}

public class GridCell
{
    public List<Link>[] Links = new List<Link>[4]; // list of directions and then links per direction
    public List<Link>[,] DistanceToLinks; // gridsize x gridsize with a distance to every link to find the closest link quickly
    public Direction[,][,] Directions; // gridsize x gridsize, then target cell reachability. is Dictionary of point good? or should use linear list?
}

public class Link
{
    public Point Point { get; set; }
    public List<Point> Targets { get; set; } = new();
}
public enum Direction
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3,
    Unreachable = 4
}