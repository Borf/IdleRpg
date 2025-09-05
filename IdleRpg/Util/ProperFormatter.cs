using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace IdleRpg.Util;

public class ProperFormatter : ConsoleFormatter, IDisposable
{
    private readonly IDisposable? _optionsReloadToken;
    private ProperFormatterOptions _formatterOptions;
    private int MaxCatLen = 0;
    private Dictionary<string, int> catColors = new();

    public ProperFormatter(IOptionsMonitor<ProperFormatterOptions> options)
        // Case insensitive
        : base("proper") =>
        (_optionsReloadToken, _formatterOptions) =
            (options.OnChange(ReloadLoggerOptions), options.CurrentValue);

    private void ReloadLoggerOptions(ProperFormatterOptions options) =>
        _formatterOptions = options;

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        string? message =
            logEntry.Formatter?.Invoke(
                logEntry.State, logEntry.Exception);

        if (message is null)
        {
            return;
        }

        string cat = logEntry.Category;
        if (cat.Contains("."))
            cat = cat.Substring(cat.LastIndexOf(".") + 1);

        MaxCatLen = Math.Max(MaxCatLen, cat.Length);
        if (!catColors.ContainsKey(cat))
        {
            var color = (catColors.Count % 6) + 31;
            catColors[cat] = color;
        }

        textWriter.WriteLine($"\x1b[36m{DateTime.Now.ToString("MM-dd HH:mm:ss")} \u001b[{catColors[cat]}m{cat.PadRight(MaxCatLen)}\u001b[0m {message}");
    }


    public void Dispose() => _optionsReloadToken?.Dispose();
}


public class ProperFormatterOptions : ConsoleFormatterOptions
{

}