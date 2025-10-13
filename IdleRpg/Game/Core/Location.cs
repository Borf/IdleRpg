using System.Diagnostics.CodeAnalysis;

namespace IdleRpg.Game.Core;

public class Location
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public required MapInstance MapInstance { get; set; }

    public Location(int x, int y)
    {
        X = x;
        Y = y;
    }
    [SetsRequiredMembers]
    public Location(int x, int y, Location location)
    {
        X = x;
        Y = y;
        MapInstance = location.MapInstance;
    }

    public void MoveBy(int x, int y)
    {
        X += x;
        Y += y;
    }
    public void MoveTo(int x, int y)
    {
        X = x;
        Y = y;
    }
}
