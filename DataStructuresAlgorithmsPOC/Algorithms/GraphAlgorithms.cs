using DataStructuresAlgorithms.DataStructures;

namespace DataStructuresAlgorithms.Algorithms;

public sealed record WeightedPath<T>(IReadOnlyList<T> Vertices, double Cost);

public static class GraphAlgorithms
{
    /// <summary>Visits vertices level by level from <paramref name="start"/>. O(V + E).</summary>
    public static IEnumerable<T> BreadthFirst<T>(Graph<T> graph, T start)
        where T : notnull
    {
        EnsureVertex(graph, start);

        HashSet<T> visited = [start];
        CircularQueue<T> queue = new();
        queue.Enqueue(start);

        while (queue.TryDequeue(out T? vertex))
        {
            yield return vertex;

            foreach (Edge<T> edge in graph.Neighbors(vertex))
            {
                if (visited.Add(edge.To))
                {
                    queue.Enqueue(edge.To);
                }
            }
        }
    }

    /// <summary>Follows each branch as deep as possible before backtracking. O(V + E), iterative.</summary>
    public static IEnumerable<T> DepthFirst<T>(Graph<T> graph, T start)
        where T : notnull
    {
        EnsureVertex(graph, start);

        HashSet<T> visited = [];
        ArrayStack<T> stack = new();
        stack.Push(start);

        while (stack.TryPop(out T? vertex))
        {
            if (!visited.Add(vertex))
            {
                continue;
            }

            yield return vertex;

            // Push in reverse so neighbours are explored in insertion order.
            IReadOnlyList<Edge<T>> neighbors = graph.Neighbors(vertex);

            for (int i = neighbors.Count - 1; i >= 0; i--)
            {
                if (!visited.Contains(neighbors[i].To))
                {
                    stack.Push(neighbors[i].To);
                }
            }
        }
    }

    /// <summary>
    /// Path with the fewest edges (weights ignored), via BFS. O(V + E).
    /// </summary>
    /// <returns>The vertices from start to goal inclusive, or null if unreachable.</returns>
    public static IReadOnlyList<T>? ShortestPathByEdges<T>(Graph<T> graph, T start, T goal)
        where T : notnull
    {
        EnsureVertex(graph, start);
        EnsureVertex(graph, goal);

        Dictionary<T, T> cameFrom = [];
        HashSet<T> visited = [start];
        CircularQueue<T> queue = new();
        queue.Enqueue(start);

        while (queue.TryDequeue(out T? vertex))
        {
            if (EqualityComparer<T>.Default.Equals(vertex, goal))
            {
                return BuildPath(cameFrom, start, goal);
            }

            foreach (Edge<T> edge in graph.Neighbors(vertex))
            {
                if (visited.Add(edge.To))
                {
                    cameFrom[edge.To] = vertex;
                    queue.Enqueue(edge.To);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Dijkstra's algorithm: cheapest path when all weights are non-negative.
    /// O((V + E) log V) with a binary heap.
    /// </summary>
    /// <returns>The cheapest path and its total cost, or null if unreachable.</returns>
    public static WeightedPath<T>? ShortestPathByWeight<T>(Graph<T> graph, T start, T goal)
        where T : notnull
    {
        EnsureVertex(graph, start);
        EnsureVertex(graph, goal);

        Dictionary<T, double> distance = new() { [start] = 0 };
        Dictionary<T, T> cameFrom = [];
        MinHeap<(double Distance, T Vertex)> frontier = new(Comparer<(double Distance, T Vertex)>.Create(
            (a, b) => a.Distance.CompareTo(b.Distance)));
        frontier.Push((0, start));

        while (frontier.TryPop(out (double Distance, T Vertex) current))
        {
            // Lazy deletion: a vertex may be queued several times; skip entries made stale by a shorter path.
            if (current.Distance > distance[current.Vertex])
            {
                continue;
            }

            if (EqualityComparer<T>.Default.Equals(current.Vertex, goal))
            {
                return new WeightedPath<T>(BuildPath(cameFrom, start, goal), current.Distance);
            }

            foreach (Edge<T> edge in graph.Neighbors(current.Vertex))
            {
                double candidate = current.Distance + edge.Weight;

                if (!distance.TryGetValue(edge.To, out double known) || candidate < known)
                {
                    distance[edge.To] = candidate;
                    cameFrom[edge.To] = current.Vertex;
                    frontier.Push((candidate, edge.To));
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Kahn's algorithm: orders a directed acyclic graph so every edge points forward
    /// (e.g. build/dependency order). O(V + E).
    /// </summary>
    /// <exception cref="InvalidOperationException">The graph contains a cycle.</exception>
    public static IReadOnlyList<T> TopologicalSort<T>(Graph<T> graph)
        where T : notnull
    {
        if (!graph.IsDirected)
        {
            throw new InvalidOperationException("Topological sort requires a directed graph.");
        }

        Dictionary<T, int> inDegree = graph.Vertices.ToDictionary(v => v, _ => 0);

        foreach (T vertex in graph.Vertices)
        {
            foreach (Edge<T> edge in graph.Neighbors(vertex))
            {
                inDegree[edge.To]++;
            }
        }

        CircularQueue<T> ready = new();

        foreach ((T vertex, int degree) in inDegree)
        {
            if (degree == 0)
            {
                ready.Enqueue(vertex);
            }
        }

        List<T> order = new(graph.VertexCount);

        while (ready.TryDequeue(out T? vertex))
        {
            order.Add(vertex);

            foreach (Edge<T> edge in graph.Neighbors(vertex))
            {
                if (--inDegree[edge.To] == 0)
                {
                    ready.Enqueue(edge.To);
                }
            }
        }

        return order.Count == graph.VertexCount
            ? order
            : throw new InvalidOperationException("The graph contains a cycle, so no topological order exists.");
    }

    private static List<T> BuildPath<T>(Dictionary<T, T> cameFrom, T start, T goal)
        where T : notnull
    {
        List<T> path = [goal];

        for (T current = goal; !EqualityComparer<T>.Default.Equals(current, start);)
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }

    private static void EnsureVertex<T>(Graph<T> graph, T vertex)
        where T : notnull
    {
        if (!graph.ContainsVertex(vertex))
        {
            throw new KeyNotFoundException($"Vertex '{vertex}' is not in the graph.");
        }
    }
}
