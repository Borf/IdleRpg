using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
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

    public Task? Task { get; private set; } = null;
    private CancellationTokenSource TokenSource { get; set; } = new CancellationTokenSource();
    private CancellationToken CancellationToken { get { return TokenSource.Token; } }

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
            catch (TaskCanceledException)
            { }
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
        while(Task is null)
            await Task.Delay(10);
        await Task;
    }

    public async Task Await()
    {
        while (Task is null)
            await Task.Delay(10);
        await Task;
    }

}
