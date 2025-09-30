namespace IdleRpg.Game.PlayerState;

public class PlayerIdleState(IServiceProvider serviceProvider, CharacterPlayer character) : ICharacterState<CharacterPlayer>
{
    public IServiceProvider ServiceProvider => serviceProvider;

    public CharacterPlayer Character => character;

    ICharacterState<CharacterPlayer> ICharacterState<CharacterPlayer>.Tick()
    {
        return this;
    }

}
