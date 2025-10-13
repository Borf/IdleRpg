using IdleRpg.Game.Core;
using L1PathFinder;
using System.Threading.Tasks;

namespace IdleRpg.Game.PlayerActions;

public class CharacterActionAttack : ICharacterAction
{
    public Character Character { get; }
    public Character Target { get; set; }
    public BgTask BgTask { get; set; }
    public bool Started { get; set; } = false;
    public string Status => $"Attacking {Target.Name}";

    public CharacterActionAttack(Character character, Character target)
    {
        Character = character;
        Target = target;
        BgTask = new BgTask("Attacking " + character.Name, BackgroundTask);
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
        if (Character.Location.MapInstance != Target.Location.MapInstance)
            throw new NotImplementedException();

        if (Character.Location.DistanceTo(Target.Location) > 2)
            return; // not in range

        Console.WriteLine($"Character {Character.Name} is hitting {Target.Name}");
        await Task.Delay(500);
    }

    public bool IsDone => BgTask.Finished;

}
