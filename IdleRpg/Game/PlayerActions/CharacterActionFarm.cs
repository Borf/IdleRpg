using IdleRpg.Game.Core;
using L1PathFinder;
using System.Threading.Tasks;

namespace IdleRpg.Game.PlayerActions;

public class CharacterActionFarm : ICharacterAction
{
    public Character Character { get; }
    public BgTask BgTask { get; set; }
    public bool Started { get; set; } = false;
    public string Status => $"Farming monsters";
    private ILogger<CharacterActionFarm> Logger;
    
    public List<Enum> MobIds { get; set; } = new();
    public TimeSpan TimeSpan { get; set; } = TimeSpan.FromHours(1);
    public DateTimeOffset TimeStart { get; private set; } = DateTimeOffset.MinValue;

    public int Kills { get; set; } = 0;
    //range
    //startlocation

    public CharacterActionFarm(Character character)
    {
        Character = character;
        BgTask = new BgTask("Farming " + character.Name, BackgroundTask);
        Logger = character.ServiceProvider.GetRequiredService<ILogger<CharacterActionFarm>>();
    }

    public void Start(BgTaskManager bgTaskManager)
    {
        TimeStart = DateTimeOffset.Now;
        bgTaskManager.Run(BgTask);
    }

    public async Task Stop()
    {
        await BgTask.Cancel();
    }

    private async Task BackgroundTask(CancellationToken token)
    {
        while (!token.IsCancellationRequested && DateTimeOffset.Now < TimeStart + TimeSpan)
        {
            var charsNear = Character.Location.MapInstance.GetCharactersAround(Character.Location, 20); //TODO: range should be configurable
            var enemies = charsNear
                .Where(c => (c is CharacterNPC npc) && (!MobIds.Any() || MobIds.Contains(npc.NpcTemplate.Id)))
                .Select(c => (CharacterNPC)c)
                .OrderBy(c => c.Location.DistanceTo(Character.Location));
            if(!enemies.Any())
            {
                Logger.LogInformation($"No enemies found for {Character.Name}");
                await Task.Delay(1000, token);
                continue;
            }
            var enemy = enemies.First(); //maybe other priority? Maybe Take(1) method up there?
            var distance = Character.Location.DistanceTo(enemy.Location);
            Logger.LogInformation($"Found enemy {enemy.Name} for {Character.Name}, distance {distance}");
            //eww duplicate code
            if(distance > 2)
            {
                var action = new CharacterActionWalk(Character, enemy.Location);
                Character.ActionQueue.QueueActionFront(action);
                await action.BgTask.Await();
            }
            else
            {
                var action = new CharacterActionAttack(Character, enemy);
                Character.ActionQueue.QueueActionFront(action);
                await action.BgTask.Await();
            }


            await Task.Delay(1000, token);
        }
        Logger.LogInformation($"Done farming {Character.Name}");
    }

    public bool IsDone => BgTask.Finished;

}
