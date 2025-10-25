using IdleRpg.Data.Db;

namespace IdleRpg.Game;

public abstract class CharacterAction
{
    public Character Character { get; protected set; }
    public abstract bool IsDone { get; }
    public bool Started { get; set; } = false;
    public BgTask BgTask { get; private set; } = null!;
    public abstract string Status();

    public CharacterAction(Character character)
    {
        Character = character;
        BgTask = new BgTask("CharacterAction " + Status, async (token) =>
        {
            await BackgroundTask(token);
            Character.ActionQueue.SignalActionDone();
        });
    }


    public virtual void Start(BgTaskManager bgTaskManager)
    {
        bgTaskManager.Run(BgTask);
    }
    public async Task Stop()
    {
        await BgTask.Cancel();
    }

    public async Task Await()
    {
        await BgTask.Await();
    }

    protected abstract Task BackgroundTask(CancellationToken token);
}
