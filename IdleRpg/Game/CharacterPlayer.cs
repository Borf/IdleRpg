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
    public CharacterActionFarm NextFarmAction { get; set; }

    public List<Core.InventoryItem> Inventory { get; set; } = [];
    public Dictionary<Enum, Core.InventoryItem> EquippedItems { get; set; } = []; //GameService.equipEnum for key

    public void Log(LogCategory category, string message)
    {
        Logs.Add(new() { Category = category, Message = message });
        while(Logs.Count > 100)
            Logs.RemoveAt(0);
    }


    public CharacterPlayer(string name, IServiceProvider serviceProvider) : base(serviceProvider)
    {
        this.Name = name;
        ActionQueue.Logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger($"ActionQueue:{Name}");
        NextFarmAction = new(this);
        serviceProvider.GetRequiredService<BgTaskManager>().Run(
        new BgTask("Auto save " + this.Name, async token =>
        {
            while (!token.IsCancellationRequested)
            {
                Task.Delay(TimeSpan.FromSeconds(120), token).Wait(token);
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

        var character = context.Characters.Include(c => c.Stats).Include(c => c.Inventory).Include(c => c.Equips).FirstOrDefault(c => c.Id == Id);
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

        await context.SaveChangesAsync();


        foreach (var stat in Stats)
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


        //double sync for inventory
        foreach (var item in Inventory)
        {
            var dbItem = character.Inventory.FirstOrDefault(i => i.Id == item.Guid);
            if(dbItem == null)
            {
                character.Inventory.Add(dbItem = new Data.Db.InventoryItem()
                {
                    Id = item.Guid,
                    Character = character,
                    ItemId = Convert.ToInt32(item.ItemId),
                });
                context.InventoryItems.Add(dbItem);
            }
            //no need to update..itemIds won't change
        }
        //remove deleted items
        foreach (var dbItem in character.Inventory.Where(i => !Inventory.Any(inv => inv.Guid == i.Id)))
            context.InventoryItems.Remove(dbItem);



        foreach(var dbEquip in character.Equips)
        {
            var dbEquipSlot = (Enum)Enum.Parse(gameService.equipEnum, dbEquip.EquipSlot);
            if (EquippedItems.ContainsKey(dbEquipSlot))
            {
                if (EquippedItems[dbEquipSlot].Guid != dbEquip.ItemId) //TODO: dunno if this if is needed
                    dbEquip.ItemId = EquippedItems[dbEquipSlot].Guid;
            }
            else
                context.CharacterEquip.Remove(dbEquip);
        }
        foreach(var equip in EquippedItems)
        {
            if(!character.Equips.Any(e => e.EquipSlot == equip.Key.ToString()))
            {
                var newEquip = new CharacterEquip()
                {
                    Character = character,
                    EquipSlot = equip.Key.ToString(),
                    ItemId = equip.Value.Guid,
                };
                character.Equips.Add(newEquip);
                context.CharacterEquip.Add(newEquip);
            }
        }
        await context.SaveChangesAsync();
    }

}
