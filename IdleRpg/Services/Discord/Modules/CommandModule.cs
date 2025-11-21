using Discord;
using Discord.Interactions;
using IdleRpg.Data;
using IdleRpg.Game;
using IdleRpg.Game.Core;
using IdleRpg.Util;
using Microsoft.EntityFrameworkCore;

namespace IdleRpg.Services.Discord.Modules;

public class CommandModule : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private InMemoryLogStore logStore;
    private ApplicationDbContext context;

    public CommandModule(GameService gameService, InMemoryLogStore logStore, ApplicationDbContext context)
    {
        this.gameService = gameService;
        this.logStore = logStore;
        this.context = context;
    }

    [SlashCommand("setup", "Adds the control panel")]
    public async Task Setup()
    {
        await RespondAsync("Creating...", ephemeral: true);

        using var header = ((IDiscordGame)gameService.GameCore).HeaderGenerator.GetImage(DiscordMenu.Main, null!);
        using var headerStream = header.AsPngStream();

        //TODO: get the introtext / image from the core
        await Context.Channel.SendFileAsync(new FileAttachment(headerStream, "header.png"), null,
            components: new ComponentBuilderV2()
                .WithTextDisplay("# Idle RPG")
                .WithMediaGallery(["attachment://header.png"])
                .WithSeparator()
                .WithTextDisplay("Play as a character in an immersive world. Fight monsters, get stronger, do quests, personalize your character and share the fun with other players")
                .WithActionRow(ar => ar
                    .WithButton("Start", "start:new", ButtonStyle.Success, emote: Emoji.Parse(":crossed_swords:"))
                    .WithButton("Settings", "settings:new", ButtonStyle.Secondary, emote: Emoji.Parse(":gear:"))
                    .WithButton("Help", "help", ButtonStyle.Secondary, emote: Emoji.Parse(":question:"))
                ).Build()
            );
    }

    [SlashCommand("logs", "Shows the last 50 lines of log")]
    public async Task Logs()
    {
        var logs = string.Join("\n", logStore.GetLogs());
        var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(logs));

        await RespondWithFileAsync(ms, "log.txt", "# Here is your log");

    }

    [SlashCommand("rankings", "Shows the levels of players")]
    public async Task Rankings()
    {
        //TODO: get level enum value somehow?
        var characters = context.Characters.Include(c => c.Stats).ToList();
        characters = characters.OrderBy(c => c.Stats.First(s => s.Stat == "Level").Value).ToList();
        string msg = "# Levels\n";
        msg += string.Join("\n", characters.Take(20).Select(c => $"- <@{c.Id}> {c.Name} ({c.Stats.First(s => s.Stat == "Level").Value})"));
        await RespondAsync(msg);
    }
}
