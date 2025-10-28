using IdleRpg.Util;

namespace IdleRpg.Game;

public class ActionQueue
{
    public LinkedList<CharacterAction> Queue { get; } = new();
    public AsyncAutoResetEvent Signal { get; set; } = new(false);
    public BgTask Task { get; }
    private BgTaskManager bgTaskManager;
    public bool Any => Queue.Any();
    public CharacterAction? First => Queue.FirstOrDefault();
    private ILogger<ActionQueue> Logger;

    public ActionQueue(BgTaskManager bgTaskManager, ILogger<ActionQueue> logger)
    {
        this.bgTaskManager = bgTaskManager;
        Task = new BgTask("Character ActionQueueTask", Handler);
        bgTaskManager.Run(Task);
        Logger = logger;
    }
    private async Task Handler(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await Signal.WaitAsync();
                Monitor.Enter(Queue);
                {
                    var done = Queue.Where(q => q.IsDone && q.Started).ToList();
                    Monitor.Exit(Queue);
                    foreach (var q in done)
                        await q.BgTask.Cancel();
                    Monitor.Enter(Queue);
                    foreach (var q in done)
                        Queue.Remove(q);
                    if (Queue.Any())
                    {

                        var front = Queue.First();
                        if (!front.Started)
                        {
                            Logger.LogInformation("Starting " + front);
                            front.Start(bgTaskManager);
                            front.Started = true;
                            //break;
                        }
                    }
                }
                Monitor.Exit(Queue);
            }
            catch (Exception ex)
            {
                Monitor.Exit(Queue);
                Logger.LogError("ActionQueue exception: {0}", ex);
            }
        }
        Console.WriteLine("Done????");
    }


    public void QueueAction(CharacterAction action)
    {
        lock (Queue)
            Queue.AddLast(action);
        Signal.Set();
    }

    public void QueueActionFront(CharacterAction action)
    {
        lock(Queue)
            Queue.AddFirst(action);
        Signal.Set();
    }
    public async Task ClearActions()
    {
        Monitor.Enter(Queue);
        while (Queue.Any())
        {
            var action = Queue.First();
            Queue.RemoveFirst();
            Monitor.Exit(Queue);
            await action.Stop();
            Monitor.Enter(Queue);
        }
        Monitor.Exit(Queue);
    }

    public string ToDiscordString()
    {
        List<string> strings = new();
        foreach(var action in Queue.Take(20))
        {
            string? str = action.ToString();
            if(str != null)
                strings.Add(str + " " + (action.IsDone ? "done" : "not done") + ", " + (action.Started ? "started" : "not started") + ", " + action.BgTask.Task.Status);
        }

        return string.Join("\n", strings.Select(s => "- " + s));
    }

    public void SignalActionDone()
    {
        Signal.Set();
    }
}
