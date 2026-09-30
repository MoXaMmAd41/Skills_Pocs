using DataStructuresAlgorithms.Algorithms;
using DataStructuresAlgorithms.DataStructures;

namespace DataStructuresAlgorithms.Playground.Demos;

internal sealed class GraphDemo : CommandDemo
{
    private const int MaxMatrixVertices = 10;

    // Drawn by hand: arbitrary graphs don't lay out well in text, but the sample is worth seeing.
    private static readonly string[] SamplePicture =
    [
        "        1         1",
        "    A ─────── B ─────── D",
        "    │                   │",
        "  4 │                   │ 1",
        "    │         1         │",
        "    C ───────────────── E"
    ];

    private Graph<string> _graph = CreateSample();
    private bool _isSample = true;

    // Edges of the last path found, drawn in green until the graph changes.
    private HashSet<(string From, string To)> _highlighted = [];

    public override string Title => "Graph — BFS, DFS, shortest paths, topological sort";

    protected override IReadOnlyList<string> Commands =>
    [
        "edge <a> <b> [w]      add an edge (weight defaults to 1)",
        "bfs <start>           breadth-first order",
        "dfs <start>           depth-first order",
        "path <from> <to>      fewest edges (BFS)",
        "cheapest <from> <to>  lowest total weight (Dijkstra)",
        "topo                  topological order (directed graphs only)",
        "new <type>            empty graph; type = directed or undirected",
        "sample                reload the sample graph",
        "show                  draw the graph again"
    ];

    protected override bool ChangesState(string command) =>
        command is "edge" or "new" or "sample" or "show";

    protected override void Execute(string command, string[] args)
    {
        switch (command)
        {
            case "edge":
                _graph.AddEdge(Word(args), Word(args, 1), args.Length > 2 ? Number(args, 2) : 1);
                _isSample = false;
                _highlighted = [];
                break;
            case "bfs":
                Ui.Result($"BFS: {string.Join(" → ", GraphAlgorithms.BreadthFirst(_graph, Word(args)))}   (level by level)");
                break;
            case "dfs":
                Ui.Result($"DFS: {string.Join(" → ", GraphAlgorithms.DepthFirst(_graph, Word(args)))}   (deep first, then backtrack)");
                break;
            case "path":
                IReadOnlyList<string>? path = GraphAlgorithms.ShortestPathByEdges(_graph, Word(args), Word(args, 1));
                Highlight(path);
                Ui.Result(path is null ? "Unreachable." : $"Fewest edges: {string.Join(" → ", path)} ({Plural(path.Count - 1, "edge")})");
                RenderAdjacencyList([.. _graph.Vertices]);
                break;
            case "cheapest":
                WeightedPath<string>? cheapest = GraphAlgorithms.ShortestPathByWeight(_graph, Word(args), Word(args, 1));
                Highlight(cheapest?.Vertices);
                Ui.Result(cheapest is null ? "Unreachable." : $"Cheapest: {string.Join(" → ", cheapest.Vertices)} (cost {cheapest.Cost})");
                RenderAdjacencyList([.. _graph.Vertices]);
                break;
            case "topo":
                Ui.Result($"Build order: {string.Join(" → ", GraphAlgorithms.TopologicalSort(_graph))}");
                break;
            case "new":
                string type = Word(args).ToLowerInvariant();
                _graph = type is "directed" or "undirected"
                    ? new Graph<string>(directed: type == "directed")
                    : throw new FormatException("Type must be 'directed' or 'undirected'.");
                _isSample = false;
                _highlighted = [];
                break;
            case "sample":
                _graph = CreateSample();
                _isSample = true;
                _highlighted = [];
                break;
            case "show":
                break;
            default:
                UnknownCommand(command);
                break;
        }
    }

    protected override void Render()
    {
        List<string> vertices = [.. _graph.Vertices];
        int edgeCount = vertices.Sum(v => _graph.Neighbors(v).Count) / (_graph.IsDirected ? 1 : 2);

        Console.WriteLine();
        Ui.State($"{(_graph.IsDirected ? "Directed" : "Undirected")} graph · {Plural(vertices.Count, "vertex", "vertices")} · {Plural(edgeCount, "edge")}");

        if (vertices.Count == 0)
        {
            Ui.Muted("  (empty — add edges with: edge A B 2)");
            return;
        }

        if (_isSample)
        {
            Console.WriteLine();
            SamplePicture.ToList().ForEach(Ui.State);
        }

        RenderAdjacencyList(vertices);

        if (vertices.Count <= MaxMatrixVertices)
        {
            RenderAdjacencyMatrix(vertices);
        }
    }

    private void RenderAdjacencyList(List<string> vertices)
    {
        int width = vertices.Max(v => v.Length);
        string arrow = _graph.IsDirected ? "→" : "─";

        Console.WriteLine();
        Ui.Muted(_highlighted.Count > 0
            ? "Adjacency list (neighbour(weight); green = last path found):"
            : "Adjacency list (neighbour(weight)):");

        foreach (string vertex in vertices)
        {
            List<(string, ConsoleColor)> line = [($"  {vertex.PadRight(width)} {arrow} ", ConsoleColor.White)];
            IReadOnlyList<Edge<string>> neighbors = _graph.Neighbors(vertex);

            if (neighbors.Count == 0)
            {
                line.Add(("(none)", ConsoleColor.DarkGray));
            }

            foreach (Edge<string> edge in neighbors)
            {
                bool onPath = _highlighted.Contains((vertex, edge.To));
                line.Add(($"{edge.To}({edge.Weight})  ", onPath ? ConsoleColor.Green : ConsoleColor.Gray));
            }

            Ui.Line(line);
        }
    }

    private void RenderAdjacencyMatrix(List<string> vertices)
    {
        int cell = Math.Max(4, vertices.Max(v => v.Length) + 2);

        Console.WriteLine();
        Ui.Muted("Adjacency matrix (row → column weight, · = no edge):");
        Ui.Muted(new string(' ', cell + 2) + string.Concat(vertices.Select(v => v.PadLeft(cell))));

        foreach (string from in vertices)
        {
            Dictionary<string, double> weights = _graph.Neighbors(from)
                .GroupBy(e => e.To)
                .ToDictionary(g => g.Key, g => g.Min(e => e.Weight));

            string row = string.Concat(vertices.Select(to =>
                (weights.TryGetValue(to, out double w) ? w.ToString() : "·").PadLeft(cell)));

            Ui.State($"  {from.PadRight(cell)}{row}");
        }
    }

    private void Highlight(IReadOnlyList<string>? path)
    {
        _highlighted = [];

        for (int i = 0; path is not null && i < path.Count - 1; i++)
        {
            _highlighted.Add((path[i], path[i + 1]));

            if (!_graph.IsDirected)
            {
                _highlighted.Add((path[i + 1], path[i]));
            }
        }
    }

    private static string Plural(int count, string singular, string? plural = null) =>
        $"{count} {(count == 1 ? singular : plural ?? singular + "s")}";

    // Fewest edges A→E is A-C-E (2 edges, cost 5); cheapest is A-B-D-E (3 edges, cost 3).
    private static Graph<string> CreateSample()
    {
        Graph<string> graph = new();
        graph.AddEdge("A", "B", 1);
        graph.AddEdge("A", "C", 4);
        graph.AddEdge("B", "D", 1);
        graph.AddEdge("D", "E", 1);
        graph.AddEdge("C", "E", 1);
        return graph;
    }
}
