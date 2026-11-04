namespace DataStructuresAlgorithms.Topics;

public class TreeNode(int value, TreeNode? left = null, TreeNode? right = null)
{
    public int Value = value;
    public TreeNode? Left = left;
    public TreeNode? Right = right;
}

/// <summary>
/// BFS (Breadth-First): visit level by level using a QUEUE. Finds the shortest path in unweighted graphs.
/// DFS (Depth-First): go as deep as possible first using RECURSION (or a stack).
/// Both are O(V + E): every vertex and edge is seen once.
/// </summary>
public static class BfsDfs
{
    // ---------- Tree ----------

    public static List<int> TreeBfs(TreeNode root)
    {
        List<int> order = [];
        Queue<TreeNode> queue = new();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            TreeNode node = queue.Dequeue();
            order.Add(node.Value);
            if (node.Left is not null) queue.Enqueue(node.Left);
            if (node.Right is not null) queue.Enqueue(node.Right);
        }

        return order;
    }

    // Pre-order: node, left, right. (In-order = left, node, right. Post-order = left, right, node.)
    public static void TreeDfs(TreeNode? node, List<int> order)
    {
        if (node is null) return;
        order.Add(node.Value);
        TreeDfs(node.Left, order);
        TreeDfs(node.Right, order);
    }

    public static int Height(TreeNode? node) => node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));

    // ---------- Graph (adjacency list) ----------

    public static List<string> GraphBfs(Dictionary<string, List<string>> graph, string start)
    {
        List<string> order = [];
        HashSet<string> visited = [start];
        Queue<string> queue = new();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            string node = queue.Dequeue();
            order.Add(node);

            foreach (string next in graph[node])
            {
                if (visited.Add(next)) queue.Enqueue(next);
            }
        }

        return order;
    }

    public static List<string> GraphDfs(Dictionary<string, List<string>> graph, string start)
    {
        List<string> order = [];
        Visit(start);
        return order;

        void Visit(string node)
        {
            if (order.Contains(node)) return;
            order.Add(node);
            foreach (string next in graph[node]) Visit(next);
        }
    }

    /// <summary>BFS remembers who discovered each node, so the first time we reach the goal is the shortest path.</summary>
    public static List<string>? ShortestPath(Dictionary<string, List<string>> graph, string start, string goal)
    {
        Dictionary<string, string?> cameFrom = new() { [start] = null };
        Queue<string> queue = new();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            string node = queue.Dequeue();

            if (node == goal)
            {
                List<string> path = [];
                for (string? n = goal; n is not null; n = cameFrom[n]) path.Insert(0, n);
                return path;
            }

            foreach (string next in graph[node])
            {
                if (cameFrom.TryAdd(next, node)) queue.Enqueue(next);
            }
        }

        return null;
    }

    public static void Demo()
    {
        //        1
        //       / \
        //      2   3
        //     / \   \
        //    4   5   6
        TreeNode root = new(1, new(2, new(4), new(5)), new(3, null, new(6)));
        List<int> dfs = [];
        TreeDfs(root, dfs);
        Console.WriteLine("Tree:     1 / (2: 4, 5) (3: _, 6)");
        Console.WriteLine($"  BFS (level order) -> {string.Join(", ", TreeBfs(root))}");
        Console.WriteLine($"  DFS (pre-order)   -> {string.Join(", ", dfs)}");
        Console.WriteLine($"  Height            -> {Height(root)}");

        //  A - B - D
        //  |   |
        //  C - E - F
        Dictionary<string, List<string>> graph = new()
        {
            ["A"] = ["B", "C"],
            ["B"] = ["A", "D", "E"],
            ["C"] = ["A", "E"],
            ["D"] = ["B"],
            ["E"] = ["B", "C", "F"],
            ["F"] = ["E"]
        };
        Console.WriteLine("Graph:    A-B, A-C, B-D, B-E, C-E, E-F");
        Console.WriteLine($"  BFS from A        -> {string.Join(", ", GraphBfs(graph, "A"))}");
        Console.WriteLine($"  DFS from A        -> {string.Join(", ", GraphDfs(graph, "A"))}");
        Console.WriteLine($"  Shortest A -> F   -> {string.Join(" -> ", ShortestPath(graph, "A", "F")!)}");
    }
}
