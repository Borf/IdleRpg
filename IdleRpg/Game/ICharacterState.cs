using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace IdleRpg.Game;

public interface ICharacterState<T> where T : Character
{
    protected T Character { get; }
}
