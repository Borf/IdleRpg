using IdleRpg.Game.Core;
using IdleRpg.Game.PlayerActions;
using System;

namespace IdleRpg.Game;

public class Character
{
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<Enum, long> Stats { get; set; } = new();
    public Location Location { get; set; } = null!;
    public IServiceProvider ServiceProvider { get; }
    
    //TODO: refactor these 2 out into a new class?
    public Queue<ICharacterAction> ActionQueue { get; } = new();
    public SemaphoreSlim ActionQueueSemaphore { get; set; } = new(1);
    public BgTask ActionQueueTask { get; }

    public Character(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        ActionQueueTask = new BgTask("Character ActionQueueTask", ActionQueueHandler);
        ServiceProvider.GetRequiredService<BgTaskManager>().Run(ActionQueueTask);
    }

    private async Task ActionQueueHandler(CancellationToken token)
    {
        while(!token.IsCancellationRequested)
        {
            await ActionQueueSemaphore.WaitAsync();
            Console.WriteLine("Actionqueue semaphore!");
            if(!ActionQueue.Any())
            {
                Console.WriteLine("no actions in queue?");
            }

            while (ActionQueue.Any())
            {
                var front = ActionQueue.Peek();
                if (!front.Started)
                {
                    Console.WriteLine("Starting " + front);
                    front.Start(ServiceProvider.GetRequiredService<BgTaskManager>());
                    front.Started = true;
                }
                if(front.IsDone)
                {
                    Console.WriteLine("Action done " + front);
                    ActionQueue.Dequeue();
                }

            }
        }
    }

    public void CalculateStats()
    {
        var gameService = ServiceProvider.GetRequiredService<GameService>();
        foreach (var modifier in gameService.sortedModifiers)
        {
            if (gameService.NotCalculatedStats.Contains(modifier.Stat))
                continue;
            Stats[modifier.Stat] = modifier.Calculation(Stats);
        }
    }

    public void WalkTo(Location target)
    {
        QueueAction(new CharacterActionWalk(this, target));
    }

    private void QueueAction(ICharacterAction action)
    {
        ActionQueue.Enqueue(action);
        ActionQueueSemaphore.Release();
    }
}