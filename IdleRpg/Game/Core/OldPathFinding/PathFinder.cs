namespace IdleRpg.Game.Core.OldPathFinding;

public static class PathFinder
{
    //public static PathFindData LoadData(Map map)
    //{
    //    PathFindData data = new PathFindData(map);

    //    int count = 0;
    //    Parallel.For(0, data.GridWidth, cellX =>
    //    {
    //        for (int cellY = 0; cellY < data.GridHeight; cellY++)
    //        {
    //            CalculatePathFindData(map, data, ref count, data.GridCells[cellX, cellY]);
    //        }
    //    });
    //    return data;
    //}

    //private static int CalculatePathFindData(Map map, PathFindData data, ref int count, GridCell cell)
    //{
    //    for (int x = 0; x < PathFindData.GridSize; x++)
    //        for (int y = 0; y < PathFindData.GridSize; y++)
    //            cell.DistanceToLinks[x, y] = new();

    //    foreach (var direction in Enum.GetValues<Direction>().Where(d => d != Direction.Unreachable))
    //    {
    //        //cell.Links[(int)direction] = new List<Link>();
    //        var offset = direction.Offset();
    //        var otherCellPos = cell.ArrayPosition + offset;
    //        if (otherCellPos.X < 0 || otherCellPos.Y < 0 || otherCellPos.X >= data.GridCells.GetLength(0) || otherCellPos.Y >= data.GridCells.GetLength(1))
    //            continue; // out of bounds
    //        var otherCell = data.GridCells[otherCellPos.X, otherCellPos.Y];

    //        for (int i = 0; i < PathFindData.GridSize; i++)
    //        {
    //            Point pos, otherPos;

    //            if (direction == Direction.Up || direction == Direction.Down)
    //            {
    //                pos = new Point(cell.GlobalPosition.X + i, cell.GlobalPosition.Y + (direction == Direction.Up ? 0 : PathFindData.GridSize - 1));
    //                otherPos = new Point(cell.GlobalPosition.X + i, cell.GlobalPosition.Y + (direction == Direction.Up ? -1 : PathFindData.GridSize));
    //            }
    //            else
    //            {
    //                pos = new Point(cell.GlobalPosition.X + (direction == Direction.Left ? 0 : PathFindData.GridSize - 1), cell.GlobalPosition.Y + i);
    //                otherPos = new Point(cell.GlobalPosition.X + (direction == Direction.Left ? -1 : PathFindData.GridSize), cell.GlobalPosition.Y + i);
    //            }

    //            if (map[pos] == CellType.Walkable && map[otherPos] == CellType.Walkable)
    //            {
    //                var link = cell.Links.SingleOrDefault(l => l.Point == pos) ?? new Link(pos);
    //                if (!link.Targets.Contains(otherPos))
    //                    link.Targets.Add(otherPos);
    //                //cell.Links[(int)direction].Add(link);
    //                cell.Links.Add(link);
    //            }
    //        }
    //    }
    //    for (int x = 0; x < PathFindData.GridSize; x++)
    //    {
    //        for (int y = 0; y < PathFindData.GridSize; y++)
    //        {
    //            var pos = new Point(cell.GlobalPosition.X + x, cell.GlobalPosition.Y + y);
    //            int[,] distanceMap = new int[PathFindData.GridSize, PathFindData.GridSize];
    //            for (int xx = 0; xx < PathFindData.GridSize; xx++)
    //                for (int yy = 0; yy < PathFindData.GridSize; yy++)
    //                    distanceMap[xx, yy] = int.MaxValue; // initialize with max value
    //            distanceMap[x, y] = 0; // distance to self is 0
    //            Queue<Point> queue = new Queue<Point>();
    //            queue.Enqueue(pos);
    //            while (queue.Count > 0)
    //            {
    //                var currentGlobal = queue.Dequeue();
    //                var currentCell = map[currentGlobal];
    //                if (currentCell != CellType.Walkable)
    //                    continue; // not walkable, skip
    //                foreach (var direction in Enum.GetValues<Direction>().Where(d => d != Direction.Unreachable))
    //                {
    //                    var offset = direction.Offset();
    //                    var neighborPos = currentGlobal + offset;
    //                    if (neighborPos.X < 0 || neighborPos.Y < 0 || neighborPos.X >= map.Width || neighborPos.Y >= map.Height)
    //                        continue; // out of bounds
    //                    if (neighborPos.X - cell.GlobalPosition.X < 0 || neighborPos.Y - cell.GlobalPosition.Y < 0 || neighborPos.X - cell.GlobalPosition.X >= PathFindData.GridSize || neighborPos.Y - cell.GlobalPosition.Y >= PathFindData.GridSize)
    //                        continue; // out of bounds in the grid cell
    //                    if (map[neighborPos] != CellType.Walkable)
    //                        continue; // not walkable, skip
    //                    int newDistance = distanceMap[currentGlobal.X - cell.GlobalPosition.X, currentGlobal.Y - cell.GlobalPosition.Y] + 1;
    //                    if (newDistance < distanceMap[neighborPos.X - cell.GlobalPosition.X, neighborPos.Y - cell.GlobalPosition.Y])
    //                    {
    //                        distanceMap[neighborPos.X - cell.GlobalPosition.X, neighborPos.Y - cell.GlobalPosition.Y] = newDistance;
    //                        queue.Enqueue(neighborPos);
    //                    }
    //                }
    //            }
    //            //now we have a distancemap for x,y in this grid, so we can fill in the distances and directions
    //            foreach (var link in cell.Links)
    //            {
    //                if (link.Point.X < cell.GlobalPosition.X || link.Point.Y < cell.GlobalPosition.Y || link.Point.X >= cell.GlobalPosition.X + PathFindData.GridSize || link.Point.Y >= cell.GlobalPosition.Y + PathFindData.GridSize)
    //                    continue; // out of bounds in the grid cell
    //                int linkX = link.Point.X - cell.GlobalPosition.X;
    //                int linkY = link.Point.Y - cell.GlobalPosition.Y;
    //                if (distanceMap[linkX, linkY] < int.MaxValue) // if reachable
    //                {
    //                    if (!cell.DistanceToLinks[linkX, linkY].ContainsKey(link))
    //                        cell.DistanceToLinks[linkX, linkY][link] = distanceMap[linkX, linkY];
    //                    else
    //                        cell.DistanceToLinks[linkX, linkY][link] = Math.Min(cell.DistanceToLinks[linkX, linkY][link], distanceMap[linkX, linkY]);
    //                }
    //            }


    //        }
    //    }
    //    Console.WriteLine("Calculating pathfinding: " + Math.Round((count++ / (float)(data.GridWidth * data.GridHeight) * 100), 2));
    //    return count;
    //}
}
