using IdleRpg.Game.Core;
using L1PathFinder;
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
                .Where(c => (c is CharacterNPC npc) && (!MobIds.Any() || MobIds.Contains(npc.NpcTemplate.Id)))
                .Select(c => (CharacterNPC)c)
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
                var action = new CharacterActionWalk(Character, enemy.Location);
                Character.ActionQueue.QueueActionFront(action);
                Logger.LogInformation($"awaiting walk");
                await action.Await();
                Logger.LogInformation($"Done awaiting walk");
            }
            else
            {
                var action = new CharacterActionAttack(Character, enemy);
                Character.ActionQueue.QueueActionFront(action);
                Logger.LogInformation($"awaiting attack");
                await action.Await();
                Logger.LogInformation($"Done awaiting attack");
            }


            await Task.Delay(1000, token);
        }
        Logger.LogInformation($"Done farming {Character.Name}");
    }


    public override string? ToString()
    {
        return "Killing monsters... ";
    }
}
