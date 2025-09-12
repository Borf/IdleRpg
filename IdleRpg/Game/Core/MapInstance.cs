namespace IdleRpg.Game.Core;

public class MapInstance
{
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public required Map Map { get; set; }
    public List<CharacterPlayer> Characters { get; set; } = new();

    public IEnumerable<CharacterNPC> Npcs => Characters.Where(c => c is CharacterNPC).Select(c => (CharacterNPC)c);
    public IEnumerable<CharacterPlayer> Players => Characters.Where(c => c is CharacterPlayer).Select(c => (CharacterPlayer)c);

}
