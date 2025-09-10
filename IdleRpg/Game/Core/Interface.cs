using IdleRpg.Util;

namespace IdleRpg.Game.Core;

public class SpawnLocation
{
    public required Point Position { get; init; }
    public required Enum Mob { get; init; }
    public int Amount { get; init; } = 1;
    public int Range { get; init; } = 10;
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
