using Discord;
using Discord.Interactions;
using IdleRpg.Data.Db;
using IdleRpg.Game;
using IdleRpg.Game.Core;
using IdleRpg.Util;
using Microsoft.VisualBasic;

namespace IdleRpg.Services.Discord.Modules;

public class ActionsMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private DiscordMessageBuilderService dmb;

    public ActionsMenu(GameService gameService, MapGeneratorService mapGenerator, DiscordMessageBuilderService dmb)
    {
        this.gameService = gameService;
        this.dmb = dmb;
    }

    [ComponentInteraction("actions")]
    public async Task Actions()
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        await dmb.ActionsMenu(Context.Interaction, character);
    }

    [ComponentInteraction("actions:teleport:*:*")]
    public async Task ActionsTeleportToLocation(string area, string locationname)
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        var locations = character.Location.MapInstance.Map.MapLocations;
        var location = locations.First(l => l.Name == locationname && l.Region == area);
        character.Location.X = location.Position.X;
        character.Location.Y = location.Position.Y;
        await dmb.ActionsMenu(Context.Interaction, character);
    }
    [ComponentInteraction("actions:teleport")]
    public async Task ActionsTeleport() => await ActionsTeleport("");

    [ComponentInteraction("actions:teleport:*")]
    public async Task ActionsTeleport(string cat)
    {
        await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        using var header = ((IDiscordGame)gameService.GameCore).HeaderGenerator.GetImage(DiscordMenu.Main, character);
        using var headerStream = header.AsPngStream();

        var cb = new ComponentBuilderV2()
            .WithMediaGallery(["attachment://header.png"])
            .WithNavigation("actions:teleport");

        var locations = character.Location.MapInstance.Map.MapLocations;
        var cats = locations.GroupBy(l => l.Region).OrderBy(l => l.Min(l => character.Location.DistanceTo(l.Position))).Take(5);
        if (string.IsNullOrEmpty(cat))
            cat = cats.First().Key;

        var ar = new ActionRowBuilder();
        foreach(var c in cats)
            ar.WithButton(c.Key, $"actions:teleport:{c.Key}", c.Key == cat ? ButtonStyle.Success : ButtonStyle.Primary);
        cb.WithActionRow(ar);

        ar = new();
        foreach (var loc in cats.First(c => c.Key == cat))
        {
            ar.WithButton(loc.Name, $"actions:teleport:{loc.Region}:{loc.Name}");
            if(ar.ComponentCount() == 5)
            {
                cb.WithActionRow(ar);
                ar = new();
            }
        }
        if(ar.ComponentCount() > 0)
            cb.WithActionRow(ar);



        await ModifyOriginalResponseAsync(c =>
        {
            c.Attachments = new List<FileAttachment>() { new FileAttachment(headerStream, "header.png") };
            c.Components = cb.Build();
        });

    }






}
