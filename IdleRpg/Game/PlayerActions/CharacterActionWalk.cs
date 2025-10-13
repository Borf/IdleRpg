using IdleRpg.Game.Core;
using L1PathFinder;
using System.Threading.Tasks;

namespace IdleRpg.Game.PlayerActions;

public class CharacterActionWalk : ICharacterAction
{
    public Character Character { get; }
    public Location TargetLocation { get; set; }
    public BgTask BgTask { get; set; }
    public bool Started { get; set; } = false;
    public string Status => $"Walking to {TargetLocation.X}, {TargetLocation.Y}";
    public List<L1PathFinder.Point>? CurrentPath { get; set; }

    public CharacterActionWalk(Character character, Location targetLocation)
    {
        Character = character;
        TargetLocation = targetLocation;
        BgTask = new BgTask("Walking " + character.Name, BackgroundTask);
    }

    public void Start(BgTaskManager bgTaskManager)
    {
        bgTaskManager.Run(BgTask);
    }

    public async Task Stop()
    {
        await BgTask.Cancel();
    }

    private async Task BackgroundTask(CancellationToken token)
    {
        if (Character.Location.MapInstance != TargetLocation.MapInstance)
            throw new NotImplementedException();

        List<Point> currentPath;
        var length = Character.Location.MapInstance.Map.Planner.Search(
            new Point(TargetLocation.X, TargetLocation.Y),
            new Point(Character.Location.X, Character.Location.Y), 
            out currentPath );
        CurrentPath = currentPath;
        if (length <= 0 || CurrentPath.Count == 0)
        {
            Console.WriteLine("Could not find path. Distance " + length);
            return;// No path found
        }

        Console.WriteLine("Found path!");
        foreach(var point in CurrentPath)
        {
            Console.WriteLine($" - {point.X}, {point.Y}");
        }

        foreach (var p in CurrentPath)
        {
            Console.WriteLine($"Moving {Character.Name} to {p.X}, {p.Y}");
            while((Character.Location.X != p.X || Character.Location.Y != p.Y) && !token.IsCancellationRequested)
            {
                Character.Location.X += Math.Sign(p.X - Character.Location.X);
                Character.Location.Y += Math.Sign(p.Y - Character.Location.Y);
                Console.WriteLine($"Step {Character.Name} to {Character.Location.X}, {Character.Location.Y}");
                await Task.Delay(1000, token); // Simulate walking time
            }
            if (token.IsCancellationRequested)
                break;
        }
        Console.WriteLine($"Done Moving {Character.Name}");
    }

    public bool IsDone
    { 
        get 
        { 
            if(Character.Location.X == TargetLocation.X && Character.Location.Y == TargetLocation.Y && Character.Location.MapInstance == TargetLocation.MapInstance)
                return true;
            if (BgTask.Finished)
                return true;
            return false;
        } 
    }

}
