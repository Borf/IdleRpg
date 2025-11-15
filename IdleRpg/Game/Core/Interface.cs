using IdleRpg.Util;

namespace IdleRpg.Game.Core;

public class MonsterSpawnTemplate
{
    public required Point Position { get; init; }
    public required Enum MobId { get; init; }
    public int Amount { get; init; } = 1;
    public int RangeX { get; init; } = 0;
    public int RangeY { get; init; } = 0;
    public int Range => Math.Max(RangeX, RangeY);
    public SpawnType SpawnType { get; init; } = SpawnType.Rect;
    public TimeSpan RespawnTime { get; init; } = TimeSpan.FromSeconds(0);
}

public enum SpawnType
{
    Sphere,
    Rect,
    FixedPoint,
    Area //TODO
}
