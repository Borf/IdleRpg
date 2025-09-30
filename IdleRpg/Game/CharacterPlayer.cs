using IdleRpg.Data.Db;
using IdleRpg.Game.PlayerState;

namespace IdleRpg.Game;

public class CharacterPlayer : Character
{
    public ICharacterState<CharacterPlayer> State { get; set; }
    public BgTask BehaviourTask;
    public CharacterPlayer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        State = new PlayerIdleState(serviceProvider, this);
    }

    public void Start(IServiceProvider serviceProvider)
    {
        var bgTaskManager = serviceProvider.GetRequiredService<BgTaskManager>();
        BehaviourTask = new BgTask("Behaviour " + Name, async (token) => await BehaviourRunner(token, serviceProvider));
        bgTaskManager.Run(BehaviourTask);
    }

    private async Task BehaviourRunner(CancellationToken token, IServiceProvider serviceProvider)
    {
        while(true)
        {

        }
    }

}
