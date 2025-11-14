using IdleRpg.Game.Core;
using L1PathFinder;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IdleRpg.Game.PlayerActions;

public class CharacterActionFarm : CharacterAction
{
    public override string Status() => $"Farming monsters";
    public override bool IsDone => BgTask.Finished;
    private ILogger<CharacterActionFarm> Logger;
    
    public List<Enum> MobIds { get; set; } = new();
    public TimeSpan TimeSpan { get; set; } = TimeSpan.FromHours(1);
    public DateTimeOffset TimeStart { get; private set; } = DateTimeOffset.MinValue;

    public int Kills { get; set; } = 0;
    //range
    //startlocation

    public CharacterActionFarm(Character character) : base(character)
    {
        Logger = character.ServiceProvider.GetRequiredService<ILogger<CharacterActionFarm>>();
    }

    public override void Start(BgTaskManager bgTaskManager)
    {
        base.Start(bgTaskManager);
        TimeStart = DateTimeOffset.Now;
    }
    protected override async Task BackgroundTask(CancellationToken token)
    {
        int noMobCount = 0;
        while (!token.IsCancellationRequested && DateTimeOffset.Now < TimeStart + TimeSpan)
        {
            var charsNear = Character.Location.MapInstance.GetCharactersAround(Character.Location, 20); //TODO: range should be configurable
            var enemies = charsNear
                .Where(c => (c is CharacterMonster npc) && (!MobIds.Any() || MobIds.Contains(npc.NpcTemplate.Id)))
                .Select(c => (CharacterMonster)c)
                .OrderBy(c => c.Location.DistanceTo(Character.Location));
            if(!enemies.Any())
            {
                noMobCount++;
                Logger.LogInformation($"No enemies found for {Character.Name}");
                if (noMobCount > 60)
                {
                    Logger.LogInformation($"Giving up on farming");
                    break;
                }
                await Task.Delay(1000, token);
                continue;
            }
            noMobCount = 0;
            var enemy = enemies.First(); //maybe other priority? Maybe Take(1) method up there?
            var distance = Character.Location.DistanceTo(enemy.Location);
            Logger.LogInformation($"Found enemy {enemy.Name} for {Character.Name}, distance {distance}");
            //eww duplicate code
            if(distance > 2)
            {
                var action = new CharacterActionWalk(Character, enemy.Location) { ParentAction = this };
                Character.ActionQueue.QueueActionFront(action);
                await action.Await();
                distance = Character.Location.DistanceTo(enemy.Location);
                if(distance > 2)
                {
                    await Task.Delay(1000); // you didn't reach. Maybe no route
                }
            }
            else
            {
                var action = new CharacterActionAttack(Character, enemy) { ParentAction = this };
                Character.ActionQueue.QueueActionFront(action);
                await action.Await();
            }
        }
        Logger.LogInformation($"Done farming {Character.Name}");
    }


    public override string? ToString()
    {
        if(Started)
            return "Battling monsters for " + (TimeSpan - (DateTimeOffset.Now - TimeStart)).ToString(@"hh\:mm\:ss");
        else
            return "Battling monsters for " + TimeSpan.ToString(@"hh\:mm\:ss");
    }
}
