using IdleRpg.Data.Db;
using IdleRpg.Game.PlayerState;

namespace IdleRpg.Game;

public class CharacterPlayer : Character
{
    public ICharacterState<CharacterPlayer> State
    {
        get
        {
            if (ActionQueue.Any())
            {
                return new PlayerIdleState(this);
            }
            else
                return new PlayerIdleState(this);
        }
    }
    public CharacterPlayer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public void Start(IServiceProvider serviceProvider)
    {
    }

}
