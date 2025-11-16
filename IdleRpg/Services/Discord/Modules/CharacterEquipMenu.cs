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

public class CharacterEquipMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private MapGeneratorService mapGenerator;
    private ILogger<CharacterInventoryMenu> logger;
    public CharacterEquipMenu(GameService gameService, MapGeneratorService mapGenerator, ILogger<CharacterInventoryMenu> logger)
    {
        this.gameService = gameService;
        this.mapGenerator = mapGenerator;
        this.logger = logger;
    }

    [ComponentInteraction("character:equip")]
    public async Task CharacterEquip()
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);

        var items = character.Inventory.GroupBy(i => i.ItemId).ToList();
        var sortedItems = items.OrderBy(i => gameService.ItemTemplates[i.First().ItemId].Name);

        using var header = ((IDiscordGame)gameService.GameCore).HeaderGenerator.GetImage(DiscordMenu.Main, character);
        using var headerStream = header.AsPngStream();

        await ModifyOriginalResponseAsync(c =>
        {
            c.AddAttachment(new FileAttachment(headerStream, "header.png"));

            c.Components = new ComponentBuilderV2()
                .WithMediaGallery(["attachment://header.png"])
                .WithNavigation("character:equip")
                .WithTextDisplay($"\n" + string.Join("\n", character.EquippedItems.Select(kv => $"- {kv.Key}: {gameService.ItemTemplates[kv.Value.ItemId].Name}")))
                .Build();
        });

    }

}
