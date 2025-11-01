using IdleRpg.Data.Db;

namespace IdleRpg.Game;

public abstract class CharacterAction
{
    public CharacterAction? ParentAction { get; set; } = null;
    public Character Character { get; protected set; }
    public DateTimeOffset StartTime { get; set; }
    public abstract bool IsDone { get; }
    public bool Started { get; set; } = false;
    public BgTask BgTask { get; private set; } = null!;
    public abstract string Status();

    public CharacterAction(Character character)
    {
        Character = character;
        BgTask = new BgTask("CharacterAction " + Status(), async (token) =>
        {
            await BackgroundTask(token);
            Character.ActionQueue.SignalActionDone();
        });
    }


    public virtual void Start(BgTaskManager bgTaskManager)
    {
        bgTaskManager.Run(BgTask);
        Started = true;
        StartTime = DateTimeOffset.Now;
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
