namespace IdleRpg.Game;

public class BgTaskManager
{
    private List<BgTask> BackgroundTasks = new List<BgTask>();



    public void Run(BgTask task)
    {
        task.ParentTask?.ChildTasks.Add(task);
        BackgroundTasks.Add(task);
        task.Start();
    }

}
