namespace IdleRpg.Util;

public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
    public Point(Point p)
    {
        X = p.X;
        Y = p.Y;
    }
    public override string ToString()
    {
        return $"({X}, {Y})";
    }
    public override bool Equals(object? obj)
    {
        if (obj is not Point p)
            return false;
        return X == p.X && Y == p.Y;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
    public static bool operator ==(Point a, Point b) => a.Equals(b);
    public static bool operator !=(Point a, Point b) => !a.Equals(b);
    public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
}
