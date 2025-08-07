namespace IdleRpg.Game.Core;

public class MapInstance
{
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public required Map Map { get; set; }
    public List<Character> Characters { get; set; } = new();

    public IEnumerable<Npc> Npcs => Characters.Where(c => c is Npc).Select(c => (Npc)c);
    public IEnumerable<Player> Players => Characters.Where(c => c is Player).Select(c => (Player)c);

}
