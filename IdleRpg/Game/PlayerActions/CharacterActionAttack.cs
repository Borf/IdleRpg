using IdleRpg.Game.Core;
using L1PathFinder;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IdleRpg.Game.PlayerActions;

public class CharacterActionAttack : CharacterAction
{
    public override string Status() => $"Attacking {Target?.Name}";
    public Character Target { get; set; }
    private ILogger<CharacterActionAttack> Logger;

    public CharacterActionAttack(Character character, Character target) : base(character)
    {
        Target = target;
        Logger = character.ServiceProvider.GetRequiredService<ILogger<CharacterActionAttack>>();
    }

    protected override async Task BackgroundTask(CancellationToken token)
    {
        if (Character.Location.MapInstance != Target.Location.MapInstance)
            throw new NotImplementedException();

        if (Character.Location.DistanceTo(Target.Location) > 2)
            return; // not in range


        var gameService = Character.ServiceProvider.GetRequiredService<GameService>();
        var skill = gameService.Skills.First(); // TODO: select skill
        var damage = skill.CalculateDamage(Character, Target);

        Logger.LogInformation($"Character {Character.Name} is hitting {Target.Name} with {damage}");


        gameService.GameCore.Damage(Character, Target, damage);

        if (!gameService.GameCore.IsAlive(Target))
        {
            Logger.LogInformation($"{Character.Name} has killed {Target.Name}");
            if (Target is CharacterMonster npc)
            {
                if (Character is CharacterPlayer player)
                {
                    player.Log(LogCategory.Battle, $"You killed a {Target.Name}");
                    gameService.GameCore.GainExp(Character, npc.Template);

                    foreach (var drop in npc.Template.ItemDrops)
                    {
                        if (Random.Shared.NextDouble() <= drop.DropChance)
                        {
                            player.Log(LogCategory.Battle, "You got a " + gameService.ItemTemplates[drop.Item].Name);
                            player.Inventory.Add(new InventoryItem(drop.Item, Guid.NewGuid()));
                        }
                    }
                }
                await npc.Die();
                if(npc.Spawner != null)
                    npc.Spawner.SpawnedMonsters.Remove(npc);
                npc.Location.MapInstance.Characters.Remove(npc);
            }
        }

        await Task.Delay(500);
        Logger.LogInformation($"{Character.Name} done attacking");
    }


    public override string? ToString()
    {
        return "Attacking " + Target.Name;
    }

}
