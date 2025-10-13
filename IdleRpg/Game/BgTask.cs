using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace IdleRpg.Game;

public class BgTask
{
    public required string Name { get; set; }
    public required Func<CancellationToken, Task> Action { get; set; }
    public BgTask? ParentTask { get; set; } = null;
    public List<BgTask> ChildTasks { get; set; } = new List<BgTask>();
    public bool Finished { get; private set; } = false;

    private Task Task = null!;
    private CancellationTokenSource TokenSource { get; set; } = new CancellationTokenSource();
    private CancellationToken CancellationToken { get { return TokenSource.Token; } }

    public BgTask() { }
    [SetsRequiredMembers]
    public BgTask(string name, Func<CancellationToken, Task> action)
    {
        Name = name;
        Action = action;
    }


    public void Start()
    {
        Finished = false;
        Task = Task.Run(async () =>
        {
            try
            {
                await Action(CancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Task Exception: " + ex); //TODO: _logger
            }
            finally
            {
                Finished = true;
                if (ParentTask != null)
                    ParentTask.ChildTasks.Remove(this);
            }
        });
    }

    public async Task Cancel()
    {
        TokenSource.Cancel();
        await Task;
    }

    public async Task Await()
    {
        await Task;
    }

}
