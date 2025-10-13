using IdleRpg.Data.Db;
using IdleRpg.Game.PlayerActions;

namespace IdleRpg.Game;

public class CharacterPlayer : Character
{
    public string State => ActionQueue.First?.Status ?? "Idle";

    public CharacterActionFarm NextFarmAction { get; set; }

    public CharacterPlayer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        NextFarmAction = new(this);
    }

    public void Start(IServiceProvider serviceProvider)
    {

    }

}
