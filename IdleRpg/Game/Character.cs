using IdleRpg.Game.Core;
using IdleRpg.Game.PlayerActions;
using IdleRpg.Util;
using System;
using System.Collections;

namespace IdleRpg.Game;

public class Character
{
    public ulong Id { get; set; } // is the playerID for players, but dunno what it is for NPCs :swt:
    public string Name { get; set; } = string.Empty;
    public Dictionary<Enum, long> Stats { get; set; } = new();
    public Location Location { get; set; } = null!;
    public IServiceProvider ServiceProvider { get; }
    
    public ActionQueue ActionQueue { get; private set; }
    public string State => ActionQueue.First?.Status() ?? "Idle";
    public Character(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        ActionQueue = serviceProvider.GetRequiredService<ActionQueue>();// new(ServiceProvider.GetRequiredService<BgTaskManager>());
    }

    public void CalculateStats()
    {
        var gameService = ServiceProvider.GetRequiredService<GameService>();
        foreach (var modifier in gameService.PrimaryModifiers)
        {
            if (gameService.NotCalculatedStats.Contains(modifier.Stat))
                continue;
            Stats[modifier.Stat] = modifier.Calculation(Stats);
        }
    }

    public void WalkTo(Location target)
    {
        ActionQueue.QueueAction(new CharacterActionWalk(this, target));
    }


}