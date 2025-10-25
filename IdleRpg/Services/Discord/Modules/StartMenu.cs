using Discord;
using Discord.Interactions;
using IdleRpg.Data.Db;
using IdleRpg.Game;
using IdleRpg.Game.Core;
using IdleRpg.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;

namespace IdleRpg.Services.Discord.Modules;

public class StartMenu : InteractionModuleBase<SocketInteractionContext>
{
    private GameService gameService;
    private DiscordMessageBuilderService dmb;

    public StartMenu(GameService gameService, MapGeneratorService mapGenerator, DiscordMessageBuilderService dmb)
    {
        this.gameService = gameService;
        this.dmb = dmb;
    }

    [ComponentInteraction("start:new")]
    public async Task Start()
    {
        await RespondAsync(null, ephemeral: true, components: new ComponentBuilderV2().WithTextDisplay("Loading...").Build());
        try
        {
            var character = gameService.GetCharacter(Context.User.Id);
            await dmb.StartMenu(Context.Interaction, character);
        }catch(Exception)
        {
            await dmb.CreateCharacter(Context.Interaction, gameService.GameCore.GetNewCharacterOptions());
        }

    }

    [ComponentInteraction("start")]
    public async Task StartReplace()
    {
        if(!Context.Interaction.HasResponded)
            await DeferAsync(ephemeral: true);
        var character = gameService.GetCharacter(Context.User.Id);
        await dmb.StartMenu(Context.Interaction, character);
    }


    [ComponentInteraction("charcreate:*:*")]
    public async Task CharCreate(string options, string newoption, string[] values)
    {
        await DeferAsync(ephemeral: true);
        int value = int.Parse(values[0]);
        var charOptions = gameService.GameCore.GetNewCharacterOptions();
        charOptions.FromString(options);

        charOptions.Set(newoption, value);

        await dmb.CreateCharacter(Context.Interaction, charOptions);
    }

    [ComponentInteraction("charcreatedo:*")]
    public async Task CharCreateDo(string options)
    {
        await RespondWithModalAsync<CharCreateModal>($"charcreatedo:{options}");
    }


    [ModalInteraction("charcreatedo:*")]
    public async Task CharCreateDo2(string options, CharCreateModal modal)
    {
        await DeferAsync(ephemeral: true);
        var charOptions = gameService.GameCore.GetNewCharacterOptions();
        charOptions.FromString(options);

        try
        {
            var character = gameService.CreateCharacter(Context.User.Id, modal.Name, charOptions);

            await dmb.StartMenu(Context.Interaction, character);
        }
        catch(Exception)
        {
            await dmb.CreateCharacter(Context.Interaction, charOptions, $"```diff\n- The name {modal.Name} is already taken\n```");
        }

    }

}

public class CharCreateModal : IModal
{
    public string Title => "Character Name";

    [InputLabel("Name")]
    [ModalTextInput("name", TextInputStyle.Short, "Your name", 3, 20)]
    public string Name { get; set; } = string.Empty;
}