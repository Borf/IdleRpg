using IdleRpg.Util;

namespace IdleRpg.Game.Core;

public class SpawnTemplate
{
    public required Point Position { get; init; }
    public required Enum Mob { get; init; }
    public int Amount { get; init; } = 1;
    public int RangeX { get; init; } = 10;
    public int RangeY { get; init; } = 10;
    public int Range => Math.Max(RangeX, RangeY);
    public SpawnType SpawnType { get; init; } = SpawnType.Sphere;
    public TimeSpan RespawnTime { get; init; } = TimeSpan.FromMinutes(1);
}

public enum SpawnType
{
    Sphere,
    Rect,
    FixedPoint,
    Area //TODO
}
