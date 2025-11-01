using IdleRpg.Game.Core;
using System.Runtime.Loader;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace IdleRpg.Game;

public class CoreLoader : IDisposable
{
    private AssemblyLoadContext? assemblyContext;
    private Assembly? assembly;
    private FileSystemWatcher CoreWatcher;

    private readonly string CoreName;
    private readonly ILogger<CoreLoader> Logger;
    private readonly ICoreHolder coreHolder;

    public CoreLoader(string coreName, ILogger<CoreLoader> logger, ICoreHolder coreHolder)
    {
        CoreName = coreName;
        Logger = logger;
        this.coreHolder = coreHolder;
        CoreWatcher = new FileSystemWatcher(Path.Combine("Resources", "Games", CoreName, "Core"));
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
        LoadCore();
    }

    public void Dispose()
    {
        CoreWatcher.EnableRaisingEvents = false;
    }

    private void LoadCore()
    {
        var oldAssemblyContext = assemblyContext;
        assemblyContext = new AssemblyLoadContext($"{CoreName}.dll", true);


        var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location) ?? string.Empty;
        CSharpCompilation compilation = CSharpCompilation.Create($"{CoreName}.dll")
            .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.CSharp")).Location))
            .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")))
            .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")))
            .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")))
            .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")))
            .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Collections.dll")))
            .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Linq.dll")))
            .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Linq.Queryable.dll")))
            .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Linq.Expressions.dll")))
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(Type).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(Game.Core.IGameCore).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(MemoryPack.MemoryPackSerializer).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(Image).Assembly.Location))
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        var files = Directory
            .GetFiles(Path.Combine("Resources", "Games", CoreName), "*.cs", SearchOption.AllDirectories)
            .Where(f => !Path.GetRelativePath(Path.Combine("Resources", "Games", CoreName), f).StartsWith("obj\\"));
        Logger.LogInformation($"Compiling files {string.Join(", ", files.Select(f => Path.GetRelativePath(Path.Combine("Resources", "Games", CoreName), f)))}");
        foreach (var file in files)
        {
            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(File.ReadAllText(file)));
        }

        using (var dllStream = new MemoryStream())
        using (var pdbStream = new MemoryStream())
        {
            var emitResult = compilation.Emit(dllStream, pdbStream);
            if (!emitResult.Success)
            {
                Logger.LogError(string.Join("\n", emitResult.Diagnostics.Select(d => $"{d.Location}\t{d.GetMessage()}")));
                assemblyContext.Unload();
                assemblyContext = oldAssemblyContext;
            }
            else
            {
                Logger.LogInformation("Core Compiled successfully.");
                dllStream.Seek(0, SeekOrigin.Begin);
                pdbStream.Seek(0, SeekOrigin.Begin);
                assembly = assemblyContext.LoadFromStream(dllStream, pdbStream);
                Logger.LogInformation($"Core Loaded successfully. Found {assembly.GetTypes().Length} types");

                var core = (IGameCore?)Activator.CreateInstance(assembly.GetTypes().First(t => typeof(Game.Core.IGameCore).IsAssignableFrom(t) && !t.IsAbstract));
                if (core != null)
                {
                    coreHolder.GameCore = core;
                    coreHolder.statsEnum = core.GetStats();
                    LoadItems();
                    LoadNpcs();
                    LoadSkills();
                    oldAssemblyContext?.Unload();
                }
                else
                {
                    assemblyContext.Unload();
                    assemblyContext = oldAssemblyContext;
                    Logger.LogError("Couldn't instantiate gamecore");
                }
            }
        }


    }

    private void LoadNpcs()
    {
        Logger.LogInformation("Loading NPCs");
        coreHolder.NpcTemplates.Clear(); //TODO: make sure the old npcs are not referenced, or move them to the new npc references somehow?
        var types = assembly!.GetTypes().Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(INpcTemplate))).ToList();
        Logger.LogInformation($"Found {types.Count} npcs");
        foreach (var t in types)
        {
            var npcTemplate = ((INpcTemplate)Activator.CreateInstance(t)!);
            if(!string.IsNullOrEmpty(npcTemplate.ImageFile))
            {
                npcTemplate.Image = Image.Load<Rgba32>(Path.Combine("Resources", "Games", t.Namespace!.Replace('.', Path.DirectorySeparatorChar), npcTemplate.ImageFile));
            }
            coreHolder.NpcTemplates[npcTemplate.Id] = npcTemplate;
        }
    }
    private void LoadItems()
    {
        Logger.LogInformation("Loading Items");
        coreHolder.ItemTemplates.Clear(); //TODO: make sure the old items are not referenced, or move them to the new item references somehow?

        var types = assembly!.GetTypes().Where(t => t.IsAssignableTo(typeof(IItemTemplate))).ToList();
        Logger.LogInformation($"Found {types.Count} items");
        foreach (var t in types)
        {
            var template = ((IItemTemplate)Activator.CreateInstance(t)!);
            if (!string.IsNullOrEmpty(template.ImageFile))
            {
                template.InventoryImage = Image.Load<Rgba32>(Path.Combine("Resources", "Games", t.Namespace!.Replace('.', Path.DirectorySeparatorChar), template.ImageFile));
            }
            coreHolder.ItemTemplates[template.Id] = template;
        }
    }

    private void LoadSkills()
    {
        Logger.LogInformation("Loading Skills");
        coreHolder.Skills.Clear(); //TODO: make sure the old items are not referenced, or move them to the new item references somehow?

        var types = assembly!.GetTypes().Where(t => t.IsAssignableTo(typeof(ISkill))).ToList();
        Logger.LogInformation($"Found {types.Count} skills");
        foreach (var t in types)
            coreHolder.Skills.Add((ISkill)Activator.CreateInstance(t)!);
    }
}

public interface ICoreHolder
{
    Dictionary<Enum, IItemTemplate> ItemTemplates { get; set; }
    List<ISkill> Skills { get; set; }
    Dictionary<Enum, INpcTemplate> NpcTemplates { get; set; }
    IGameCore GameCore { get; set; }
    Type statsEnum { get; set; }
}
