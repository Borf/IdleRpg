using Discord;
using Discord.Interactions;
using IdleRpg.Game;
using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using IdleRpg.Util;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using Point = SixLabors.ImageSharp.Point;

namespace IdleRpg.Services.Discord.Modules;

public class CharacterInventoryMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private MapGeneratorService mapGenerator;
    private ILogger<CharacterInventoryMenu> logger;
    public CharacterInventoryMenu(GameService gameService, MapGeneratorService mapGenerator, ILogger<CharacterInventoryMenu> logger)
    {
        this.gameService = gameService;
        this.mapGenerator = mapGenerator;
        this.logger = logger;
    }

    [ComponentInteraction("character:inventory")]
    public async Task CharacterInventory()
    {
        await CharacterInventoryFilter(0, "all", "name", "");
    }

    [ComponentInteraction("character:inventory:*:*:*:*")]
    public async Task CharacterInventoryFilter(int page, string type, string order, string filter)
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);

        var items = character.Inventory.GroupBy(i => i.ItemId).ToList();
        var sortedItems = items.OrderBy(i => gameService.ItemTemplates[i.First().ItemId].Name);

        var font = SystemFonts.CreateFont("Tahoma", 12);
        var font2 = SystemFonts.CreateFont("Tahoma", 13, FontStyle.Bold);
        var font3 = SystemFonts.CreateFont("Tahoma", 12, FontStyle.Italic);


        using var header = ((IDiscordGame)gameService.GameCore).HeaderGenerator.GetImage(DiscordMenu.Main, character);
        using var headerStream = header.AsPngStream();


        using var image = Image.Load<Rgba32>(Path.Join("Resources", "Games", "TinyRpg", "InventoryBack.png"));

        var smb = new SelectMenuBuilder();

        int i = 0;
        foreach(var item in items)
        {
            var template = gameService.ItemTemplates[item.First().ItemId];
            if(template.InventoryImage != null)
                image.Mutate(ip => ip.DrawImage(template.InventoryImage, new Point(11 + (i % 3) * 256, 10 + (i / 3) * 85), 1.0f));

            for(int x = -1; x <= 1; x++)
                for(int y = -1; y <= 1; y++)
                    image.Mutate(ip => ip.DrawText(new RichTextOptions(font)
                    {
                        Origin = new PointF((i % 3) * 256+x, 55 + (i / 3) * 85+y),
                        TextAlignment = TextAlignment.End,
                        WrappingLength = 75,
                    }, item.Count() + "x", SixLabors.ImageSharp.Color.White));
            image.Mutate(ip => ip.DrawText(new RichTextOptions(font) { 
                    Origin = new PointF((i % 3) * 256, 55 + (i / 3) * 85), 
                    TextAlignment = TextAlignment.End,
                    WrappingLength = 75,
                }, item.Count() + "x", SixLabors.ImageSharp.Color.Black));

            image.Mutate(ip => ip.DrawText(new RichTextOptions(font2)
            {
                Origin = new PointF(100 + (i % 3) * 256, 12 + (i / 3) * 85),
                WrappingLength = 145,
            }, template.Name, SixLabors.ImageSharp.Color.Black));
            
            image.Mutate(ip => ip.DrawText(new RichTextOptions(font3)
            {
                Origin = new PointF(100 + (i % 3) * 256, 12 + (i / 3) * 85 + 16),
                WrappingLength = 145,
            },
            template.Description, SixLabors.ImageSharp.Color.Black));
            smb.AddOption($"{template.Name} ({item.Count()}x)", item.First().Guid.ToString(), template.Description);
            i++;
        }
        using var imageStream = image.AsPngStream();


        await ModifyOriginalResponseAsync(c =>
        {
            c.AddAttachment(new FileAttachment(headerStream, "header.png"));
            c.AddAttachment(new FileAttachment(imageStream, "item1.png"));

            c.Components = new ComponentBuilderV2()
                .WithMediaGallery(["attachment://header.png"])
                .WithNavigation("character:inventory")
                .WithActionRow([
                    new ButtonBuilder("All", "character:inventory:setcat:all",    ButtonStyle.Primary),
                    new ButtonBuilder("Usable", "character:battle:setcat:usable", ButtonStyle.Secondary, isDisabled: true),
                    new ButtonBuilder("Equip", "character:battle:setcat:equip",   ButtonStyle.Secondary, isDisabled: true),
                    new ButtonBuilder("Other", "character:battle:setcat:other",   ButtonStyle.Secondary, isDisabled: true),
                    new ButtonBuilder("Sort by Name", "character:battle:sort",    ButtonStyle.Success, isDisabled: true),
                ])
                .WithMediaGallery(["attachment://item1.png"])
                .WithActionRow(ar => ar.WithSelectMenu(smb
                    .WithCustomId("character:inventory:item")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .WithPlaceholder("Select the item"))
                )//options are made above
                .WithActionRow([
                    new ButtonBuilder("Prev", "character:inventory:page:0",     ButtonStyle.Primary),
                    new ButtonBuilder("Page 1/2", "blablabla",                  ButtonStyle.Secondary, isDisabled: true),
                    new ButtonBuilder("Next", "character:inventory:page:2",     ButtonStyle.Primary),
                    new ButtonBuilder("Filter", "blablablaa",                   ButtonStyle.Success, isDisabled: true),
                ])
                .Build();
        });

    }

    [ComponentInteraction("character:inventory:item")]
    public async Task CharacterInventoryItem(string[] itemGuids)
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        var items = character.Inventory.GroupBy(i => i.ItemId).ToDictionary(kv => kv.First().ItemId, kv => kv.ToList());
        var item = character.Inventory.First(i => i.Guid.ToString() == itemGuids[0]);
        var itemTemplate = gameService.ItemTemplates[item.ItemId];

        using var header = ((IDiscordGame)gameService.GameCore).HeaderGenerator.GetImage(DiscordMenu.Main, character);
        using var headerStream = header.AsPngStream();

        var image = gameService.ItemTemplates[item.ItemId].InventoryImage;
        using var imageStream = image?.AsPngStream();



        var sb = new SectionBuilder()
            .WithTextDisplay(
            $"# {itemTemplate.Name}\n" +
            $"## {itemTemplate.Description}\n" +
            $"- `Amount        ` {items[item.ItemId].Count} {itemTemplate.Name}{(items[item.ItemId].Count > 1 ? "s" : "")}\n" +
            $"- `Price         ` {string.Join(", ", itemTemplate.Value.Select(kv => $"{kv.Value} {kv.Key}"))} each, {string.Join(", ", itemTemplate.Value.Select(kv => $"{items[item.ItemId].Count * kv.Value} {kv.Key} total"))}\n")
            .WithAccessory(new ThumbnailBuilder("attachment://item.png", null, false));

        bool canBeEquipped = false;

        if(itemTemplate is IEquipable equipable)
        {
            canBeEquipped = true;

            string equipStr = $"- `Equipment type` {equipable.EquipSlot}\n" +
                $"- `When equipped ` {equipable.EquipDescription}\n";
            if(character.EquippedItems.ContainsKey(equipable.EquipSlot))
            {
                var equip = character.EquippedItems[equipable.EquipSlot];
                if (equip == item)
                    canBeEquipped = false;
                equipStr += $"- `Current Equip ` {gameService.ItemTemplates[equip.ItemId].Name} ({((IEquipable)gameService.ItemTemplates[equip.ItemId]).EquipDescription})\n";
            }
            sb.WithTextDisplay(equipStr);

        }

        var ar = new ActionRowBuilder()
            .WithButton("Sell All", $"character:inventory:sellall:{item.Guid}")
            .WithButton("Equip", $"character:inventory:equip:{item.Guid}", disabled: !canBeEquipped);


        await ModifyOriginalResponseAsync(c =>
        {
            c.AddAttachment(new FileAttachment(headerStream, "header.png"));
            c.AddAttachment(new FileAttachment(imageStream, "item.png"));

            c.Components = new ComponentBuilderV2()
                .WithMediaGallery(["attachment://header.png"])
                .WithNavigation($"character:inventory:item|{itemGuids[0]}#{itemTemplate.Name}")
                .WithSection(sb)
                .WithActionRow(ar)
                .Build();
        });
    }
    [ComponentInteraction("character:inventory:item:*")]
    public async Task CharacterInventoryItem(string itemGuid)
    {
        await CharacterInventoryItem([itemGuid]);
    }

    [ComponentInteraction("character:inventory:equip:*")]
    public async Task CharacterInventoryEquip(string itemGuid)
    {
        var character = gameService.GetCharacter(Context.User.Id);
        var items = character.Inventory.GroupBy(i => i.ItemId).ToDictionary(kv => kv.First().ItemId, kv => kv.ToList());
        var item = character.Inventory.First(i => i.Guid.ToString() == itemGuid);
        var itemTemplate = gameService.ItemTemplates[item.ItemId];
        var equip = (IEquipable)itemTemplate;

        //TODO: move these 2 to a method in character to equip stuff (that also removes old buffs)
        character.EquippedItems[equip.EquipSlot] = item;
        character.Buffs.Add(new Buff()
        {
            AppliedBy = character,
            Source = BuffSource.Equip,
            AppliedFromEquip = equip,
            Modifiers = equip.EquipEffects
        });
        character.CalculateStats();

        await CharacterInventoryItem([itemGuid]);
    }

}
