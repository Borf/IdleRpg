
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;

namespace IdleRpg.Game;

public class GameService : IHostedService
{
    private readonly ILogger<GameService> _logger;
    private Task bgTask;
    private AssemblyLoadContext? assemblyContext;
    private FileSystemWatcher CoreWatcher;

    public GameService(ILogger<GameService> logger)
    {
        _logger = logger;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Game service is starting.");

        CoreWatcher = new FileSystemWatcher(Path.Combine("Resources", "Games", "Rom", "Core"));
        CoreWatcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
        CoreWatcher.Changed += (s, e) => LoadCore(e);
        CoreWatcher.Deleted += (s, e) => LoadCore(e);
        //CoreWatcher.Renamed += (s, e) => LoadCore(e);
        CoreWatcher.Created += (s, e) => LoadCore(e);
        CoreWatcher.IncludeSubdirectories = true;
        CoreWatcher.Filter = "*.cs";
        CoreWatcher.EnableRaisingEvents = true;

        LoadCore(null);
        bgTask = Task.Run(BackgroundLoop, cancellationToken);
    }

    private void LoadCore(FileSystemEventArgs e)
    {
        _logger.LogInformation(e?.ChangeType.ToString());
        if (assemblyContext != null)
            assemblyContext.Unload();
        assemblyContext = new AssemblyLoadContext("MyAssembly", true);

        var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine("Resources", "Games", "Rom", "Core", "GameCore.cs")));

        CSharpCompilation compilation = CSharpCompilation.Create("assemblyName")
            .AddSyntaxTrees(syntaxTree)
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(Game.Core.IGameCore).Assembly.Location))
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


        using (var dllStream = new MemoryStream())
        using (var pdbStream = new MemoryStream())
        {
            var emitResult = compilation.Emit(dllStream, pdbStream);
            if (!emitResult.Success)
            {
                _logger.LogError(string.Join("\n", emitResult.Diagnostics.Select(d => d.GetMessage())));
            }
            else
            {
                _logger.LogInformation("Core loaded successfully.");
                dllStream.Seek(0, SeekOrigin.Begin);
                pdbStream.Seek(0, SeekOrigin.Begin);
                var assembly = assemblyContext.LoadFromStream(dllStream, pdbStream);
                foreach(var type in assembly.GetTypes())
                {
                    //if(type.IsEnum)
                        _logger.LogInformation($"Found {type.BaseType}: {type.FullName}");
                }
            }
        }

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Game service is stopping.");
        CoreWatcher.EnableRaisingEvents = false;

        return Task.CompletedTask;
    }


    public async Task BackgroundLoop()
    {
        while(true)
        {
            //_logger.LogInformation("Tick");
            await Task.Delay(1000);
        }
    }
}