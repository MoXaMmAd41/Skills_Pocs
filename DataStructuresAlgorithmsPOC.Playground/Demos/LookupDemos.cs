using DataStructuresAlgorithms.DataStructures;

namespace DataStructuresAlgorithms.Playground.Demos;

internal sealed class HashMapDemo : CommandDemo
{
    private readonly HashMap<string, string> _map = new(capacity: 4);

    public override string Title => "Hash map (separate chaining)";

    protected override IReadOnlyList<string> Commands =>
    [
        "set <key> <value>     add or overwrite",
        "get <key>             look up a value",
        "remove <key>          delete a key",
        "fill <n>              add n generated keys (watch it resize)"
    ];

    protected override void Execute(string command, string[] args)
    {
        int capacityBefore = _map.Capacity;

        switch (command)
        {
            case "set":
                _map[Word(args)] = Word(args, 1);
                break;
            case "get":
                Ui.Result($"{args.FirstOrDefault()} = {_map[Word(args)]}");
                break;
            case "remove":
                Ui.Result(_map.Remove(Word(args)) ? "Removed." : "Key not found.");
                break;
            case "fill":
                for (int i = 0, n = Number(args); i < n; i++) _map[$"key{_map.Count}"] = i.ToString();
                break;
            default:
                UnknownCommand(command);
                break;
        }

        if (_map.Capacity != capacityBefore)
        {
            Ui.Result($"Resized {capacityBefore} → {_map.Capacity} buckets (load factor passed 0.75)");
        }
    }

    protected override void Render()
    {
        Ui.State(_map.Count == 0 ? "(empty)" : string.Join(", ", _map.Select(p => $"{p.Key}={p.Value}")));
        Ui.Muted($"count {_map.Count}, buckets {_map.Capacity}, load factor {_map.LoadFactor:0.00}");
    }
}

internal sealed class BinarySearchTreeDemo : CommandDemo
{
    private BinarySearchTree<int> _tree = new();

    public override string Title => "Binary search tree";

    protected override IReadOnlyList<string> Commands =>
    [
        "add <n> [n...]        insert numbers",
        "remove <n>            delete a number",
        "has <n>               search for a number",
        "sorted                in-order traversal (always sorted)",
        "clear                 start over (try adding 1 2 3 4 5 to see it degenerate)"
    ];

    protected override void Execute(string command, string[] args)
    {
        switch (command)
        {
            case "add":
                foreach (int n in Numbers(args)) _tree.Add(n);
                break;
            case "remove":
                Ui.Result(_tree.Remove(Number(args)) ? "Removed." : "Not found.");
                break;
            case "has":
                Ui.Result(_tree.Contains(Number(args)) ? "Found." : "Not found.");
                break;
            case "sorted":
                Ui.Result($"In order: {Ui.List(_tree.InOrder())}");
                break;
            case "clear":
                _tree = new BinarySearchTree<int>();
                break;
            default:
                UnknownCommand(command);
                break;
        }
    }

    protected override void Render()
    {
        if (_tree.Count == 0)
        {
            Ui.State("(empty)");
            return;
        }

        foreach ((int value, int depth, TreeSide side) in _tree.PreOrder())
        {
            string label = side switch { TreeSide.Left => "L: ", TreeSide.Right => "R: ", _ => "" };
            Ui.State($"{new string(' ', depth * 4)}{label}{value}");
        }

        Ui.Muted($"count {_tree.Count}, height {_tree.Height} (a balanced tree would be ~{(int)Math.Ceiling(Math.Log2(_tree.Count + 1))})");
    }
}

internal sealed class HeapDemo : CommandDemo
{
    private readonly MinHeap<int> _heap = new();

    public override string Title => "Min-heap (priority queue)";

    protected override IReadOnlyList<string> Commands =>
    [
        "push <n> [n...]       insert numbers",
        "pop                   remove the smallest",
        "peek                  look at the smallest"
    ];

    protected override void Execute(string command, string[] args)
    {
        switch (command)
        {
            case "push":
                foreach (int n in Numbers(args)) _heap.Push(n);
                break;
            case "pop":
                Ui.Result($"Popped {_heap.Pop()}");
                break;
            case "peek":
                Ui.Result($"Smallest is {_heap.Peek()}");
                break;
            default:
                UnknownCommand(command);
                break;
        }
    }

    protected override void Render()
    {
        int[] items = _heap.UnorderedItems.ToArray();

        Ui.State($"array: {Ui.List(items)}    (not sorted — only the parent ≤ children rule holds)");

        // Draw it level by level: level k holds indices 2^k - 1 .. 2^(k+1) - 2.
        for (int start = 0, width = 1; start < items.Length; start += width, width *= 2)
        {
            Ui.Muted($"  level: {string.Join("  ", items.Skip(start).Take(width))}");
        }
    }
}

internal sealed class TrieDemo : CommandDemo
{
    private readonly Trie<string> _trie = new();

    public override string Title => "Trie (prefix tree) — powers the employee autocomplete";

    protected override IReadOnlyList<string> Commands =>
    [
        "add <word> [word...]  insert words",
        "find <prefix>         words starting with the prefix (alphabetical)",
        "seed                  add sample words: app apple application apply banana band bandana"
    ];

    protected override void Execute(string command, string[] args)
    {
        switch (command)
        {
            case "add":
                foreach (string word in args.Length > 0 ? args : throw new FormatException("Expected one or more words."))
                {
                    _trie.Insert(word, word);
                }

                break;
            case "find":
                IReadOnlyList<string> matches = _trie.FindByPrefix(Word(args));
                Ui.Result(matches.Count == 0 ? "No matches." : $"Matches: {Ui.List(matches)}");
                break;
            case "seed":
                foreach (string word in new[] { "app", "apple", "application", "apply", "banana", "band", "bandana" })
                {
                    _trie.Insert(word, word);
                }

                break;
            default:
                UnknownCommand(command);
                break;
        }
    }

    protected override void Render() => Ui.Muted($"{_trie.Count} word(s) stored");
}

internal sealed class LruCacheDemo : CommandDemo
{
    private readonly LruCache<string, string> _cache = new(capacity: 3);

    public override string Title => "LRU cache (capacity 3)";

    protected override IReadOnlyList<string> Commands =>
    [
        "set <key> <value>     add or update (evicts the least recently used when full)",
        "get <key>             read a value (marks it as recently used)",
        "remove <key>          delete a key"
    ];

    protected override void Execute(string command, string[] args)
    {
        switch (command)
        {
            case "set":
                KeyValuePair<string, string>? evicted = _cache.Set(Word(args), Word(args, 1));

                if (evicted is { } entry)
                {
                    Ui.Result($"Cache full: evicted {entry.Key}={entry.Value}");
                }

                break;
            case "get":
                Ui.Result(_cache.TryGet(Word(args), out string? value) ? $"{args[0]} = {value}" : "Miss (not in cache).");
                break;
            case "remove":
                Ui.Result(_cache.Remove(Word(args)) ? "Removed." : "Key not found.");
                break;
            default:
                UnknownCommand(command);
                break;
        }
    }

    protected override void Render() =>
        Ui.State($"most recent → {Ui.List(_cache.KeysByRecency())} ← evicted next    ({_cache.Count}/{_cache.Capacity})");
}
