using Discord;
using Discord.WebSocket;
using IdleRpg.Game;
using IdleRpg.Game.Core;
using IdleRpg.Util;
using SixLabors.ImageSharp.Processing;

namespace IdleRpg.Services.Discord;

public class DiscordMessageBuilderService
{
    private ILogger<DiscordMessageBuilderService> logger;
    private MapGeneratorService mapGenerator;
    private GameService gameService;

    public DiscordMessageBuilderService(ILogger<DiscordMessageBuilderService> logger, MapGeneratorService mapGenerator, GameService gameService)
    {
        this.logger = logger;
        this.mapGenerator = mapGenerator;
        this.gameService = gameService;
    }

    public async Task StartMenu(IDiscordInteraction interaction, CharacterPlayer character, string message = "")
    {
        using var mapImage = mapGenerator.GenerateMapImage(character, 8, 0);
        using var mapStream = mapImage.AsPngStream();

        using var header = ((IDiscordGame)gameService.GameCore).HeaderGenerator.GetImage(DiscordMenu.Main, character);
        using var headerStream = header.AsPngStream();

        await interaction.ModifyOriginalResponseAsync(c =>
        {
            c
                .AddAttachment(new FileAttachment(mapStream, "map.png"))
                .AddAttachment(new FileAttachment(headerStream, "header.png"))
                .Components = new ComponentBuilderV2()
                .WithMediaGallery(["attachment://header.png"])
                .WithNavigation("start")
                .WithSection(sb => sb
                    .WithTextDisplay($"Your character:\n" +
                        $"- Your character is {character.State}\n" +
                        $"- Your character is on {character.Location.MapInstance.Map.Name}, at {character.Location.X}, {character.Location.Y}\n" +
                        string.Join("\n", character.Logs.Reverse<LogEntry>().Take(5).Select(log => $"- <t:{log.Timestamp.ToUnixTimeSeconds()}:T>: [{log.Category}] {log.Message}")) + "\n" + message)
                    .WithAccessory(new ThumbnailBuilder("attachment://map.png", null, false)))
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder("Character", "character", ButtonStyle.Primary, emote: Emoji.Parse(":man_mage:")),
                    new ButtonBuilder("Actions", "actions", ButtonStyle.Primary, emote: Emoji.Parse(":exclamation:")),
                    //new ButtonBuilder("Inventory", "inventory", ButtonStyle.Primary, emote: Emoji.Parse(":handbag:")),
                    new ButtonBuilder("Handbook", "handbook", ButtonStyle.Primary, emote: Emoji.Parse(":notebook_with_decorative_cover:"), isDisabled: true),
                    new ButtonBuilder("Quests", "quests", ButtonStyle.Primary, emote: Emoji.Parse(":compass:"), isDisabled: true),
                ])
                .Build();
        });
    }


    public async Task ActionsMenu(IDiscordInteraction interaction, Character character, string message = "")
    {
        using var mapImage = mapGenerator.GenerateMapImage(character, 16, 0);
        using var mapStream = mapImage.AsPngStream();

        using var header = ((IDiscordGame)gameService.GameCore).HeaderGenerator.GetImage(DiscordMenu.Main, character);
        using var headerStream = header.AsPngStream();

        string queue = character.ActionQueue.ToDiscordString();

        await interaction.ModifyOriginalResponseAsync(c =>
        {
            c.Attachments = new List<FileAttachment>() { new FileAttachment(mapStream, "map.png"), new FileAttachment(headerStream, "header.png") };
            c.Components = new ComponentBuilderV2()
                .WithMediaGallery(["attachment://header.png"])
                .WithNavigation("actions")
                .WithSection(sb => {
                    sb.WithTextDisplay($"{message}\n" +
                                        $"Your character is {character.State}\n");
                    if (!string.IsNullOrEmpty(queue))
                        sb.WithTextDisplay(queue);
                    sb.WithAccessory(new ThumbnailBuilder("attachment://map.png", null, false));
                })
                .WithSeparator()
                .WithActionRow([
                    new ButtonBuilder("Walk", "actions:walk", ButtonStyle.Primary, emote: Emoji.Parse(":compass:")),
                    new ButtonBuilder("Battle", "actions:battle", ButtonStyle.Primary, emote: Emoji.Parse(":exclamation:")),
                    new ButtonBuilder("Quest", "actions:quest", ButtonStyle.Primary, emote: Emoji.Parse(":handbag:")),
                ])
                .Build();
        });
    }


    public async Task CreateCharacter(SocketInteraction interaction, ICharacterCreateCharOptions options, string message = "")
    {
        using var header = ((IDiscordGame)gameService.GameCore).HeaderGenerator.GetImage(DiscordMenu.Main, null!);
        using var headerStream = header.AsPngStream();

        using var characterImage = ((IGameCore2D)gameService.GameCore).CharCreateCharacterGenerator.GetImage(options);
        characterImage.Mutate(ip => ip.Resize(new ResizeOptions()
        {
            Size = new SixLabors.ImageSharp.Size(characterImage.Width*10, characterImage.Height*10),
            Sampler = KnownResamplers.NearestNeighbor
        }));
        using var characterStream = characterImage.AsPngStream();

        var optionsStr = options.BuildString();

        var cb = new ComponentBuilderV2()
                .WithMediaGallery(["attachment://header.png"])
                .WithTextDisplay("# Character Creation\nLet's start off by creating your character. Click the buttons below to customize your character.")
                .WithSection(sb => sb
                    .WithTextDisplay($"Your character:\n{message}")
                    .WithAccessory(new ThumbnailBuilder("attachment://character.png", null, false)))
                .WithSeparator();
        foreach(var option in options.Options)
        {
            var smb = new SelectMenuBuilder()
                .WithMinValues(1)
                .WithMaxValues(1)
                .WithPlaceholder(option.Value.Name)
                .WithCustomId($"charcreate:{optionsStr}:{option.Key}");
            int selectedOption = options.Get(option.Key);
            for(int i = option.Value.Minvalue; i <= option.Value.MaxValue; i++)
            {
                smb.AddOption($"{option.Value.Name} {i}", i + "", isDefault: i == selectedOption);
            }

            cb.WithActionRow(ar => ar.WithSelectMenu(smb));
        }
        cb.WithActionRow(ar => ar.WithButton("Create Character", $"charcreatedo:{optionsStr}", ButtonStyle.Success));


        await interaction.ModifyOriginalResponseAsync(c =>
        {
            c.Attachments = new List<FileAttachment>() { new FileAttachment(headerStream, "header.png"), new FileAttachment(characterStream, "character.png") };
            c.Components = cb
                .Build();
        });
    }
}

public static class MenuHelper
{
    public static ComponentBuilderV2 WithNavigation(this ComponentBuilderV2 cb, IDiscordInteraction interaction)
    {
        if (interaction is SocketCommandBase interact)
            return cb.WithNavigation(interact.CommandName);
        else if (interaction.Data.GetType().Name == "MessageComponentInteractionData")
            return cb.WithNavigation(interaction.Data.GetType().GetProperty("CustomId")?.GetValue(interaction.Data)?.ToString() ?? "");
        else if (interaction.Data.GetType().Name == "SocketMessageComponentData")
            return cb.WithNavigation(interaction.Data.GetType().GetProperty("CustomId")?.GetValue(interaction.Data)?.ToString() ?? "");
        return cb.WithNavigation("");
    }

    public static ComponentBuilderV2 WithNavigation(this ComponentBuilderV2 cb, string name = "")
    {
        if (name.Contains("_"))
            name = name[0..(name.IndexOf("_"))];

        var crumbs = name.Split(":").ToList();

        var arb = new ActionRowBuilder();
        if (name == "start" || crumbs[0] == "start")
            arb.WithButton("Home", "start", ButtonStyle.Success);
        else
        {
            arb.WithButton("Home", "start", ButtonStyle.Secondary);
            string path = "";
            int index = 0;
            foreach (var crumb in crumbs)
            {
                path += (index > 0 ? ":" : "") + crumb;
                arb.WithButton(crumb.FirstCharToUpper(), path, index == crumbs.Count - 1 ? ButtonStyle.Success : ButtonStyle.Secondary);
                index++;
            }
        }
        cb.WithActionRow(arb);
        cb.WithSeparator();
        return cb;
    }

    public static MessageProperties AddAttachment(this MessageProperties messageProperties, FileAttachment attachment)
    {
        if(!messageProperties.Attachments.IsSpecified)
            messageProperties.Attachments = new List<FileAttachment>();
        var attachments = messageProperties.Attachments.Value as List<FileAttachment>;
        if (attachments != null)
            attachments.Add(attachment);
        else
            throw new Exception("Exception not in list");
        return messageProperties;
    }

}