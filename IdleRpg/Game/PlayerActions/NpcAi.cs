
namespace IdleRpg.Game.PlayerActions;

public class NpcAi(Character character) : CharacterAction(character)
{
    public override bool IsDone => false; //we are never done

    public override string Status() => "Just acting like an NPC";

    protected override async Task BackgroundTask(CancellationToken token)
    {
        while(!token.IsCancellationRequested)
        {
            //beep boop do AI stuff. Can be wandering around, following a path, standing still?
            await Task.Delay(-1, token);
        }
    }
}
