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
        lock (Queue)
        {
            List<string> strings = new();
            foreach (var action in Queue.Where(q => q.ParentAction == null).Take(4).ToList())
            {
                string? str = action.ToString();
                if (str != null)
                {
                    if (action.Started)
                        str = $"[<t:{action.StartTime.ToUnixTimeSeconds()}:T>] {str}";
                    else
                        str = $"[queued] *{str}*";
                    strings.Add($"- {str}");
                    foreach(var subtask in Queue.Where(q => q.ParentAction == action))
                    {
                        strings.Add("  - " + subtask.ToString());
                    }

                }
            }

            return string.Join("\n", strings);
        }
    }

    public void SignalActionDone()
    {
        Signal.Set();
    }
}
