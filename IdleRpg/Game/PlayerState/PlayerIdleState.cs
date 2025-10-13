namespace IdleRpg.Game.PlayerState;

public class PlayerIdleState(CharacterPlayer character) : ICharacterState<CharacterPlayer>
{

    public CharacterPlayer Character => character;

}
