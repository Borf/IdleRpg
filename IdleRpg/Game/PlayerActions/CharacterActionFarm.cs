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
    
    public List<Enum> MobIds { get; set; } = new();
    public TimeSpan TimeSpan { get; set; } = TimeSpan.FromHours(1);
    public DateTimeOffset TimeStart { get; private set; } = DateTimeOffset.MinValue;
    //range
    //startlocation

    public CharacterActionFarm(Character character)
    {
        Character = character;
        BgTask = new BgTask("Farming " + character.Name, BackgroundTask);
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
        while (!token.IsCancellationRequested && DateTimeOffset.Now > TimeStart + TimeSpan)
        {
            var charsNear = Character.Location.MapInstance.GetCharactersAround(Character.Location, 20); //TODO: range should be configurable
            var enemies = charsNear
                .Where(c => (c is CharacterNPC npc) && (!MobIds.Any() || MobIds.Contains(npc.NpcTemplate.Id)))
                .Select(c => (CharacterNPC)c)
                .OrderBy(c => c.Location.DistanceTo(Character.Location));
            if(!enemies.Any())
            {
                Console.WriteLine($"No enemies found for {Character.Name}");
                await Task.Delay(1000, token);
                continue;
            }
            var enemy = enemies.First(); //maybe other priority? Maybe Take(1) method up there?
            var distance = Character.Location.DistanceTo(enemy.Location);
            Console.WriteLine($"Found enemy {enemy.Name} for {Character.Name}, distance {distance}");
            if(distance > 2)
            {
                Character.ActionQueue.QueueActionFront(new CharacterActionWalk(Character, enemy.Location));
            }
            else
            {
                Character.ActionQueue.QueueActionFront(new CharacterActionAttack(Character, enemy));
            }
            await Task.Delay(1000, token);
        }
        Console.WriteLine($"Done farming {Character.Name}");
    }

    public bool IsDone => BgTask.Finished;

}
