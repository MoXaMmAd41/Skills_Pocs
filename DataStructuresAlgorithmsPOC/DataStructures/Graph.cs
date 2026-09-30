namespace DataStructuresAlgorithms.DataStructures;

public readonly record struct Edge<T>(T To, double Weight);

/// <summary>Weighted graph stored as an adjacency list (each vertex keeps its outgoing edges).</summary>
/// <remarks>
/// AddVertex/AddEdge: O(1). Neighbours of v: O(1) to fetch, O(deg v) to walk.
/// Memory O(V + E) — much better than an adjacency matrix for sparse graphs.
/// </remarks>
public sealed class Graph<T>(bool directed = false)
    where T : notnull
{
    private readonly Dictionary<T, List<Edge<T>>> _adjacency = [];

    public bool IsDirected { get; } = directed;

    public IEnumerable<T> Vertices => _adjacency.Keys;

    public int VertexCount => _adjacency.Count;

    public void AddVertex(T vertex) => _adjacency.TryAdd(vertex, []);

    public void AddEdge(T from, T to, double weight = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(weight);

        AddVertex(from);
        AddVertex(to);

        _adjacency[from].Add(new Edge<T>(to, weight));

        if (!IsDirected)
        {
            _adjacency[to].Add(new Edge<T>(from, weight));
        }
    }

    public bool ContainsVertex(T vertex) => _adjacency.ContainsKey(vertex);

    public IReadOnlyList<Edge<T>> Neighbors(T vertex) =>
        _adjacency.TryGetValue(vertex, out List<Edge<T>>? edges)
            ? edges
            : throw new KeyNotFoundException($"Vertex '{vertex}' is not in the graph.");
}
