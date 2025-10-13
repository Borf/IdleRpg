namespace IdleRpg.Game;

public interface ICharacterAction
{
    Character Character { get; }
    bool IsDone { get; }
    bool Started { get; set; }
    void Start(BgTaskManager bgTaskManager);
    Task Stop();
    string Status { get; }
}
