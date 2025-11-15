using IdleRpg.Game.Core;
using L1PathFinder;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IdleRpg.Game.PlayerActions;

public class CharacterActionWalk : CharacterAction
{
    public Location TargetLocation { get; set; }
    public double Length { get; set; } = 0;
    public int DistanceWalked { get; set; } = 0;
    public override string Status() => $"Walking to {TargetLocation?.X}, {TargetLocation?.Y}";
    public List<Point>? CurrentPath { get; set; }
    private ILogger<CharacterActionWalk> Logger;

    public CharacterActionWalk(Character character, Location targetLocation) : base(character)
    {
        TargetLocation = targetLocation;
        Logger = character.ServiceProvider.GetRequiredService<ILogger<CharacterActionWalk>>();
    }

    protected override async Task BackgroundTask(CancellationToken token)
    {
        if (Character.Location.MapInstance != TargetLocation.MapInstance)
            throw new NotImplementedException();

        List<Point> currentPath;
        Length = Character.Location.MapInstance.Map.Planner.Search(
            new Point(TargetLocation.X, TargetLocation.Y),
            new Point(Character.Location.X, Character.Location.Y), 
            out currentPath );
        CurrentPath = currentPath;
        if (Length <= 0 || CurrentPath.Count == 0)
        {
            Length = -1;
            Logger.LogError("Could not find path. Distance " + Length);
            return;// No path found
        }

        Logger.LogInformation("Found path!");
        foreach(var point in CurrentPath)
        {
            Logger.LogInformation($" - {point.X}, {point.Y}");
        }

        foreach (var p in CurrentPath)
        {
            Logger.LogTrace($"Moving {Character.Name} to {p.X}, {p.Y}");
            while((Character.Location.X != p.X || Character.Location.Y != p.Y) && !token.IsCancellationRequested)
            {
                DistanceWalked++;
                Character.Location.X += Math.Sign(p.X - Character.Location.X);
                Character.Location.Y += Math.Sign(p.Y - Character.Location.Y);
                Logger.LogTrace($"Step {Character.Name} to {Character.Location.X}, {Character.Location.Y}");
                await Task.Delay(1000, token); // Simulate walking time
            }
            if (token.IsCancellationRequested)
                break;
        }
        Logger.LogInformation($"Done Moving {Character.Name}, took {DistanceWalked} steps");
    }

    public override bool IsDone
    { 
        get 
        { 
            if(Character.Location.X == TargetLocation.X && Character.Location.Y == TargetLocation.Y && Character.Location.MapInstance == TargetLocation.MapInstance)
                return true;
            if (Length <= 0)
                return true;
            return base.IsDone;
        } 
    }

    public override string? ToString()
    {
        return "Walking, " + (Length - DistanceWalked) + " meter remaining.";
    }
}
