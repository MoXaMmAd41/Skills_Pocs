namespace DataStructuresAlgorithms.DataStructures;

/// <summary>
/// Prefix tree mapping string keys to values. Each edge is one character, so every key sharing a
/// prefix shares the path to it — ideal for autocomplete.
/// </summary>
/// <remarks>
/// Insert: O(k) and prefix lookup: O(p + m), where k is key length, p prefix length and m the size of
/// the matching subtree walked — independent of the total number of keys.
/// Children are kept sorted, so results come back in alphabetical key order.
/// </remarks>
public sealed class Trie<TValue>
{
    private readonly Node _root = new();
    private readonly bool _ignoreCase;

    public Trie(bool ignoreCase = true) => _ignoreCase = ignoreCase;

    /// <summary>Number of distinct keys stored.</summary>
    public int Count { get; private set; }

    public void Insert(string key, TValue value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        Node node = _root;

        foreach (char c in Normalize(key))
        {
            if (!node.Children.TryGetValue(c, out Node? child))
            {
                child = new Node();
                node.Children.Add(c, child);
            }

            node = child;
        }

        if (node.Values.Count == 0)
        {
            Count++;
        }

        node.Values.Add(value);
    }

    public bool ContainsPrefix(string prefix) => FindNode(prefix) is not null;

    /// <summary>
    /// Returns up to <paramref name="limit"/> distinct values whose key starts with <paramref name="prefix"/>.
    /// A value stored under several matching keys is returned once.
    /// </summary>
    public IReadOnlyList<TValue> FindByPrefix(string prefix, int limit = int.MaxValue)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(limit, 1);

        Node? start = FindNode(prefix);

        if (start is null)
        {
            return [];
        }

        List<TValue> results = [];
        HashSet<TValue> seen = [];
        ArrayStack<Node> pending = new();
        pending.Push(start);

        // Iterative DFS; children are pushed in reverse so they are visited alphabetically.
        while (pending.TryPop(out Node? node))
        {
            foreach (TValue value in node.Values)
            {
                if (seen.Add(value))
                {
                    results.Add(value);

                    if (results.Count == limit)
                    {
                        return results;
                    }
                }
            }

            for (int i = node.Children.Count - 1; i >= 0; i--)
            {
                pending.Push(node.Children.GetValueAtIndex(i));
            }
        }

        return results;
    }

    private Node? FindNode(string prefix)
    {
        Node node = _root;

        foreach (char c in Normalize(prefix))
        {
            if (!node.Children.TryGetValue(c, out Node? child))
            {
                return null;
            }

            node = child;
        }

        return node;
    }

    private string Normalize(string text) => _ignoreCase ? text.ToLowerInvariant() : text;

    private sealed class Node
    {
        public SortedList<char, Node> Children { get; } = [];

        public List<TValue> Values { get; } = [];
    }
}
