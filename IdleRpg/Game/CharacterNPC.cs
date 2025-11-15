
using IdleRpg.Game.Core;
using IdleRpg.Game.PlayerActions;

namespace IdleRpg.Game;

public class CharacterNpc : Character
{
    public INpcTemplate Template { get; }
    public CharacterNpc(IServiceProvider serviceProvider, Core.INpcTemplate template, MapInstance instance) : base(serviceProvider)
    {
        Id = (ulong)Random.Shared.NextInt64(); //TODO: check for clashes (and check if this is needed)
        Template = template;
        Name = template.Name;
        Location = new Location(template.Position.X, template.Position.Y) { MapInstance = instance };
        ActionQueue.QueueAction(new NpcAi(this));
    }


}
