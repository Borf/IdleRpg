using IdleRpg.Util;

namespace IdleRpg.Game;

public class ActionQueue
{
    public LinkedList<ICharacterAction> Queue { get; } = new();
    public AsyncAutoResetEvent Signal { get; set; } = new(false);
    public BgTask Task { get; }
    private BgTaskManager bgTaskManager;
    public bool Any => Queue.Any();
    public ICharacterAction? First => Queue.FirstOrDefault();

    public ActionQueue(BgTaskManager bgTaskManager)
    {
        this.bgTaskManager = bgTaskManager;

        Task = new BgTask("Character ActionQueueTask", Handler);
        bgTaskManager.Run(Task);
    }
    private async Task Handler(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Signal.WaitAsync();
            Console.WriteLine("Actionqueue semaphore!");
            lock (Queue)
            {
                if (!Queue.Any())
                {
                    Console.WriteLine("no actions in queue?");
                }

                while (Queue.Any())
                {
                    var front = Queue.First();
                    if (!front.Started)
                    {
                        Console.WriteLine("Starting " + front);
                        front.Start(bgTaskManager);
                        front.Started = true;
                        break;
                    }
                    if (front.IsDone)
                    {
                        Console.WriteLine("Action done " + front);
                        Queue.RemoveFirst();
                    }
                }
            }
        }
    }


    public void QueueAction(ICharacterAction action)
    {
        lock (Queue)
            Queue.AddLast(action);
        Signal.Set();
    }

    public void QueueActionFront(ICharacterAction action)
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

}
