using IdleRpg.Data.Db;
using IdleRpg.Game.PlayerActions;

namespace IdleRpg.Game;

public class CharacterPlayer : Character
{
    public List<LogEntry> Logs { get; } = new();
    public void Log(LogCategory category, string message)
    {
        Logs.Add(new() { Category = category, Message = message });
        while(Logs.Count > 100)
            Logs.RemoveAt(0);
    }

    public CharacterActionFarm NextFarmAction { get; set; }

    public CharacterPlayer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        NextFarmAction = new(this);
    }

    public void Start(IServiceProvider serviceProvider)
    {

    }

}
