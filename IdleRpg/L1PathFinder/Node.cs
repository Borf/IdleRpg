using MemoryPack;

namespace L1PathFinder;

[MemoryPackable]
public partial class Node
{
    [MemoryPackInclude]
    public int X { get; set; }
    [MemoryPackInclude]
    public List<Bucket>? Buckets { get; set; }
    [MemoryPackInclude]
    public Node? Left { get; set; }
    [MemoryPackInclude]
    public Node? Right { get; set; }
    [MemoryPackInclude]
    public List<Vertex>? Verts { get; set; } = null;
    [MemoryPackInclude]
    public bool Leaf { get; set; }

    public Node(int x, List<Bucket>? buckets, Node? left, Node? right)
    {
        this.Buckets = buckets;
        this.X = x;
        this.Left = left;
        this.Right = right;
        this.Leaf = false;
    }
}