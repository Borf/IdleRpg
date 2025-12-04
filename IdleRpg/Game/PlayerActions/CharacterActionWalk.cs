using IdleRpg.Data.Db;
using IdleRpg.Game.Core;
using L1PathFinder;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System.Drawing;
using System.Threading.Tasks;
using Point = L1PathFinder.Point;

namespace IdleRpg.Game.PlayerActions;

public class CharacterActionWalk : CharacterAction
{
    public Location TargetLocation { get; set; }
    public double Length { get; set; } = 0;
    public int DistanceWalked { get; set; } = 0;
    public override string Status() => $"Walking to {TargetLocation?.X}, {TargetLocation?.Y}";
    public List<Point>? CurrentPath { get; set; }
    private ILogger Logger;

    public CharacterActionWalk(Character character, Location targetLocation) : base(character)
    {
        TargetLocation = targetLocation;
        Logger = character.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger($"CharacterActionWalk:{character.Name}");
    }

    protected override async Task BackgroundTask(CancellationToken token)
    {
        if (Character.Location.MapInstance != TargetLocation.MapInstance)
            throw new NotImplementedException();

        Length = Character.Location.MapInstance.Map.Planner.Search(
            new L1PathFinder.Point(TargetLocation.X, TargetLocation.Y),
            new L1PathFinder.Point(Character.Location.X, Character.Location.Y), 
            out List<Point> currentPath);
        CurrentPath = currentPath;
        if(Length <= 0 || CurrentPath.Count == 0)
        {
            Length = Character.Location.MapInstance.Map.Planner.Search(
                        new L1PathFinder.Point(TargetLocation.X, TargetLocation.Y),
                        new L1PathFinder.Point(Character.Location.X, Character.Location.Y),
                        out currentPath);
            currentPath.Reverse();
            if(Length <= 0 || CurrentPath.Count == 0)
            {
                Console.WriteLine("Still not working after reverse route");
            }
            else
            {
                Console.WriteLine("Working after reverse route");
            }
        }


        if (Length <= 0 || CurrentPath.Count == 0)
        {
            Length = -1;
            Logger.LogError("Could not find path. Distance " + Length);

            string fileName = Path.Combine("walklog", "bad", $"{Character.Location.MapInstance.Map.Name}-{Character.Location.X},{Character.Location.Y}-{TargetLocation.X},{TargetLocation.Y}.png");
            if (!File.Exists(fileName))
            {
                if (!Directory.Exists(Path.Combine("walklog", "bad")))
                    Directory.CreateDirectory(Path.Combine("walklog", "bad"));
                int size = Math.Max(Math.Abs(Character.Location.X - TargetLocation.X), Math.Abs(Character.Location.Y - TargetLocation.Y)) * 2 + 4;
                using var map = Character.ServiceProvider.GetRequiredService<MapGeneratorService>().GenerateMapImage(Character, size, 0);
                SixLabors.ImageSharp.Rectangle rect = new((Character.Location.X - size / 2),(Character.Location.Y - size / 2),size,size);
                //draw player
                map.Mutate(ip => ip.Fill(new DrawingOptions() { GraphicsOptions = new() { BlendPercentage = 0.5f } }, Brushes.Solid(SixLabors.ImageSharp.Color.Red), new SixLabors.ImageSharp.RectangleF(
                    (Character.Location.X - rect.X) * Character.Location.MapInstance.Map.MapImageTileSize + 2.0f,  //TODO: zoom should be factored in here too
                    (Character.Location.Y - rect.Y) * Character.Location.MapInstance.Map.MapImageTileSize + 2.0f, 12.0f, 12.0f)));
                //draw target
                map.Mutate(ip => ip.Fill(new DrawingOptions() { GraphicsOptions = new() { BlendPercentage = 0.5f } }, Brushes.Solid(SixLabors.ImageSharp.Color.Blue), new SixLabors.ImageSharp.RectangleF(
                    (TargetLocation.X - rect.X) * Character.Location.MapInstance.Map.MapImageTileSize + 3.0f,  //TODO: zoom should be factored in here too
                    (TargetLocation.Y - rect.Y) * Character.Location.MapInstance.Map.MapImageTileSize + 3.0f, 10.0f, 10.0f)));

                map.SaveAsPng(fileName);
            }
            return;// No path found
        }
        else
        {
            string fileName = Path.Combine("walklog", "ok", $"{Character.Location.MapInstance.Map.Name}-{Character.Location.X},{Character.Location.Y}-{TargetLocation.X},{TargetLocation.Y}.png");
            if (!File.Exists(fileName))
            {
                if (!Directory.Exists(Path.Combine("walklog", "ok")))
                    Directory.CreateDirectory(Path.Combine("walklog", "ok"));
                int size = Math.Max(Math.Abs(Character.Location.X - TargetLocation.X), Math.Abs(Character.Location.Y - TargetLocation.Y)) * 2;
                foreach(var point in CurrentPath)
                {
                    size = Math.Max(size, Math.Max(Math.Abs(Character.Location.X - point.X), Math.Abs(Character.Location.Y - point.Y)) * 2);
                }

                size += 4;


                using var map = Character.ServiceProvider.GetRequiredService<MapGeneratorService>().GenerateMapImage(Character, size, 0);
                SixLabors.ImageSharp.Rectangle rect = new((Character.Location.X - size / 2), (Character.Location.Y - size / 2), size, size);
                //draw player
                map.Mutate(ip => ip.Fill(new DrawingOptions() { GraphicsOptions = new() { BlendPercentage = 0.5f } }, Brushes.Solid(SixLabors.ImageSharp.Color.Red), new SixLabors.ImageSharp.RectangleF(
                    (Character.Location.X - rect.X) * Character.Location.MapInstance.Map.MapImageTileSize + 2.0f,  //TODO: zoom should be factored in here too
                    (Character.Location.Y - rect.Y) * Character.Location.MapInstance.Map.MapImageTileSize + 2.0f, 12.0f, 12.0f)));
                //draw target
                map.Mutate(ip => ip.Fill(new DrawingOptions() { GraphicsOptions = new() { BlendPercentage = 0.5f } }, Brushes.Solid(SixLabors.ImageSharp.Color.Blue), new SixLabors.ImageSharp.RectangleF(
                    (TargetLocation.X - rect.X) * Character.Location.MapInstance.Map.MapImageTileSize + 3.0f,  //TODO: zoom should be factored in here too
                    (TargetLocation.Y - rect.Y) * Character.Location.MapInstance.Map.MapImageTileSize + 3.0f, 10.0f, 10.0f)));

                for (int i = 0; i < CurrentPath.Count - 1; i++)
                {
                    var current = CurrentPath[i];
                    var next = CurrentPath[i + 1];
                    map.Mutate(ip => ip.DrawLine(Pens.Solid(SixLabors.ImageSharp.Color.Yellow, 4.0f), 
                        new SixLabors.ImageSharp.PointF(
                            (current.X - rect.X) * Character.Location.MapInstance.Map.MapImageTileSize + 6.0f,  //TODO: zoom should be factored in here too
                            (current.Y - rect.Y) * Character.Location.MapInstance.Map.MapImageTileSize + 6.0f),
                        new SixLabors.ImageSharp.PointF(
                            (next.X - rect.X) * Character.Location.MapInstance.Map.MapImageTileSize + 6.0f,  //TODO: zoom should be factored in here too
                            (next.Y - rect.Y) * Character.Location.MapInstance.Map.MapImageTileSize + 6.0f))
                            );

                }


                map.SaveAsPng(fileName);
            }
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
        Length = -1;
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
        if(Started)
            return $"Walking to {TargetLocation.X},{TargetLocation.Y}, {Length - DistanceWalked} meter remaining.";
        else
            return $"Walking to {TargetLocation.X},{TargetLocation.Y}";
    }
}
