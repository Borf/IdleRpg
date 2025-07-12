using IdleRpg.Game.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Runtime.Loader;

namespace IdleRpg.Game;

public class CoreLoader : IDisposable
{
    private AssemblyLoadContext? assemblyContext;
    private FileSystemWatcher CoreWatcher;

    private readonly string CoreName;
    private readonly ILogger<CoreLoader> Logger;
    private readonly Action<IGameCore> ReloadCallback;

    public CoreLoader(string coreName, ILogger<CoreLoader> logger, Action<IGameCore> reloadCallback)
    {
        CoreName = coreName;
        Logger = logger;
        CoreWatcher = new FileSystemWatcher(Path.Combine("Resources", "Games", "Rom", "Core"));
        CoreWatcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
        CoreWatcher.Changed += (s, e) => LoadCore();
        CoreWatcher.Deleted += (s, e) => LoadCore();
        //CoreWatcher.Renamed += (s, e) => LoadCore(e);
        CoreWatcher.Created += (s, e) => LoadCore();
        CoreWatcher.IncludeSubdirectories = true;
        CoreWatcher.Filter = "*.cs";
        CoreWatcher.EnableRaisingEvents = true;

        CoreName = coreName;
        ReloadCallback = reloadCallback;
        LoadCore();
    }

    public void Dispose()
    {
        CoreWatcher.EnableRaisingEvents = false;
    }

    private void LoadCore()
    {
        var oldAssemblyContext = assemblyContext;
        assemblyContext = new AssemblyLoadContext("Rom.dll", true);

        var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine("Resources", "Games", "Rom", "Core", "GameCore.cs")));

        var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location) ?? string.Empty;

        CSharpCompilation compilation = CSharpCompilation.Create("Rom.dll")
            .AddSyntaxTrees(syntaxTree)
            .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")))
            .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")))
            .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")))
            .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")))
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(Type).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(Game.Core.IGameCore).Assembly.Location))
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


        using (var dllStream = new MemoryStream())
        using (var pdbStream = new MemoryStream())
        {
            var emitResult = compilation.Emit(dllStream, pdbStream);
            if (!emitResult.Success)
            {
                assemblyContext.Unload();
                Logger.LogError(string.Join("\n", emitResult.Diagnostics.Select(d => d.GetMessage())));
            }
            else
            {
                Logger.LogInformation("Core loaded successfully.");
                dllStream.Seek(0, SeekOrigin.Begin);
                pdbStream.Seek(0, SeekOrigin.Begin);
                var assembly = assemblyContext.LoadFromStream(dllStream, pdbStream);

                var core = (IGameCore?)Activator.CreateInstance(assembly.GetTypes().First(t => typeof(Game.Core.IGameCore).IsAssignableFrom(t) && !t.IsAbstract));
                if (core != null)
                {
                    ReloadCallback.Invoke(core);
                    var statsEnum = core.GetStats();
                    Console.WriteLine(string.Join(", ", Enum.GetNames(statsEnum)));
                    oldAssemblyContext?.Unload();
                }
                else
                {
                    assemblyContext.Unload();
                    Logger.LogError("Couldn't instantiate gamecore");
                }
            }
        }


    }
}
