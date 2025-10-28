using IdleRpg.Data;
using IdleRpg.Data.Db;
using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using IdleRpg.Game.PlayerActions;
using Microsoft.EntityFrameworkCore;

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
        serviceProvider.GetRequiredService<BgTaskManager>().Run(
        new BgTask("Auto save " + this.Name, async token =>
        {
            while (!token.IsCancellationRequested)
            {
                Task.Delay(TimeSpan.FromMinutes(1), token).Wait(token);
                if (token.IsCancellationRequested)
                    break;
                await Save();
            }
        }));
    }


    public async Task Save()
    {
        var gameService = ServiceProvider.GetRequiredService<GameService>();
        using var scope = ServiceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var character = context.Characters.Include(c => c.Stats).FirstOrDefault(c => c.Id == Id);
        if(character == null)
        {
            character = new Data.Db.Character()
            {
                Id = Id,
                Name = Name,
            };
            context.Characters.Add(character);
        }

        character.Map = Location.MapInstance.Map.Name;
        character.X = Location.X;
        character.Y = Location.Y;



        foreach(var stat in Stats)
        {
            var attributes = stat.Key.GetType().GetMember(stat.Key.ToString()).First(m => m.DeclaringType == gameService.statsEnum).GetCustomAttributes(false); //yuck
            if (!attributes.Any(a => a is NotCalculatedAttribute))
                continue;

            var dbStat = character.Stats.FirstOrDefault(st => st.Stat == stat.Key.ToString());
            if(dbStat == null)
                character.Stats.Add(dbStat = new CharacterStat() { Character = character, Stat = stat.Key.ToString(), Value = stat.Value });
            dbStat.Value = stat.Value;
        }

        await context.SaveChangesAsync();
    }

}
