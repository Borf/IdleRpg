namespace IdleRpg.Game;

public static class StatModifierOrderer
{
    public static PriorityQueue<StatModifier, int> OrderByPriority(IEnumerable<StatModifier> modifiers)
    {
        var priorityQueue = new PriorityQueue<StatModifier, int>();
        foreach (var modifier in modifiers)
        {
            
        }
        return priorityQueue;
    }


}
