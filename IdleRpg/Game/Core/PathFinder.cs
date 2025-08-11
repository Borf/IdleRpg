
namespace IdleRpg.Game.Core;

public static class PathFinder
{
    public static PathFindData LoadData(Map map)
    {
        PathFindData data = new PathFindData();

        data.GridCells = new int[10,10];



        return data;
    }
}
