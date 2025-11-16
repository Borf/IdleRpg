using Discord;
using Discord.Interactions;
using IdleRpg.Game;
using IdleRpg.Game.Core;
using IdleRpg.Util;
using Microsoft.VisualBasic;
using SixLabors.ImageSharp.Processing;

namespace IdleRpg.Services.Discord.Modules;

public class AreaMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private DiscordMessageBuilderService dmb;
    private MapGeneratorService mapGenerator;

    public AreaMenu(GameService gameService, MapGeneratorService mapGenerator, DiscordMessageBuilderService dmb, MapGeneratorService mapgenerator)
    {
        this.gameService = gameService;
        this.dmb = dmb;
        this.mapGenerator = mapgenerator;
    }

    [ComponentInteraction("area")]
    public async Task Actions()
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);

        using var mapImage = mapGenerator.GenerateMapImage(character, 32, 0);
        mapImage.Mutate(ip => ip.Resize(mapImage.Width * 2, mapImage.Height * 2, KnownResamplers.NearestNeighbor));
        using var mapStream = mapImage.AsPngStream();

        using var header = ((IDiscordGame)gameService.GameCore).HeaderGenerator.GetImage(DiscordMenu.Main, character);
        using var headerStream = header.AsPngStream();

        var charsAround = character.Location.MapInstance.GetCharactersAround(character.Location, 16);
        string msg = "## You see:\n";

        bool canShop = false;
        bool canQuest = false;
        bool canTalk = false;

        if (charsAround.Count == 0)
            msg = "You see nobody around you";
        else
        {
            var npcs = charsAround.Select(n => n as CharacterNpc).Where(n => n is not null).Select(n => n!).ToList();
            foreach (var npc in npcs)
            {
                msg += $"- {npc.Name} ({npc.Location.DistanceTo(character.Location)}m away)\n";
                if (npc.Template is INpcMerchant)
                    canShop = true;
                if (npc.Template.Features.HasFlag(NpcFeatures.Dialog))
                    canTalk = true;
            }
            var monsters = charsAround
                .Where(n => n is CharacterMonster)
                .Select(n => (CharacterMonster)n)
                .GroupBy(m => m.Template.Id)
                .ToDictionary(m => m.Key, m => m.ToList());

            foreach (var monster in monsters)
            {
                msg += $"- {monster.Value.Count} {monster.Value.First()!.Name}{(monster.Value.Count > 1 ? "s" : "")}\n";
            }
        }

        await ModifyOriginalResponseAsync(c =>
        {
            c
                .AddAttachment(new FileAttachment(mapStream, "map.png"))
                .AddAttachment(new FileAttachment(headerStream, "header.png"))
                .Components = new ComponentBuilderV2()
                .WithMediaGallery(["attachment://header.png"])
                .WithNavigation("area")
                .WithSection(sb => sb
                    .WithTextDisplay(msg)
                    .WithAccessory(new ThumbnailBuilder("attachment://map.png", null, false)))
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder("Shop", "area:shop", ButtonStyle.Primary, emote: Emoji.Parse(":moneybag:"), isDisabled: !canShop),
                    new ButtonBuilder("Quests", "area:quests", ButtonStyle.Primary, emote: Emoji.Parse(":compass:"), isDisabled: !canQuest),
                    new ButtonBuilder("Talk", "area:talk", ButtonStyle.Primary, emote: Emoji.Parse(":speech_balloon:"), isDisabled: !canTalk),
                ])
                .Build();
        });
    }
    [ComponentInteraction("area:talk")]
    public async Task Talk()
    {
        await Talk("");
    }
    [ComponentInteraction("area:talk:*")]
    public async Task Talk(string npcName)
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);

        using var header = ((IDiscordGame)gameService.GameCore).HeaderGenerator.GetImage(DiscordMenu.Main, character);
        using var headerStream = header.AsPngStream();
        SixLabors.ImageSharp.Image? npcImage = null;
        Stream? npcImageStream = null;

        var charsAround = character.Location.MapInstance.GetCharactersAround(character.Location, 16);

        var npcs = charsAround.Where(n => n is CharacterNpc).Select(n => (CharacterNpc)n).ToList();

        var cb = new ComponentBuilderV2()
                    .WithMediaGallery(["attachment://header.png"])
                    .WithNavigation("area:talk");

        if (string.IsNullOrEmpty(npcName))
            cb.WithTextDisplay("# Who would you like to talk to?");
        else
        {
            var npc = npcs.First(n => n.Template.GetType().FullName == npcName);
            npcImage = npc.Template.Image!.Clone(ip => ip.Resize(npc.Template.Image!.Width * 8, npc.Template.Image!.Height * 8, KnownResamplers.NearestNeighbor));
            npcImageStream = npcImage!.AsPngStream();
            var dialog = ((INpcDialog)npc.Template).Dialog;

            cb.WithSection(sb => sb
                .WithTextDisplay($"# {npc.Name}\n{dialog}")
                .WithAccessory(new ThumbnailBuilder("attachment://npc.png", "Npc"))
            );
        }
        var ar = new ActionRowBuilder();
        foreach (var npc in npcs)
        {
            if (npc.Template.Features.HasFlag(NpcFeatures.Dialog))
            {
                ar.WithButton(npc.Name, $"area:talk:{npc.Template.GetType().FullName}", style: npcName == npc.Template.GetType().FullName ? ButtonStyle.Success : ButtonStyle.Primary);
                if (ar.ComponentCount() == 5)
                {
                    cb.WithActionRow(ar);
                    ar = new();
                }
            }
        }
        cb.WithActionRow(ar);

        await ModifyOriginalResponseAsync(c =>
        {
            c.AddAttachment(new FileAttachment(headerStream, "header.png"))
                .Components = cb.Build();
            if (npcImageStream != null)
                c.AddAttachment(new FileAttachment(npcImageStream, "npc.png"));
        });

        if (npcImageStream != null)
            npcImageStream.Dispose();
        if (npcImage != null)
            npcImage.Dispose();
    }
}
