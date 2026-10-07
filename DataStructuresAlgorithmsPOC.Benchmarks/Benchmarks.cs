using BenchmarkDotNet.Attributes;
using DataStructuresAlgorithms.Algorithms;
using DataStructuresAlgorithms.DataStructures;

namespace DataStructuresAlgorithms.Benchmarks;

/// <summary>Our sorts against the runtime's introsort. Each run sorts a fresh copy of the same data.</summary>
[MemoryDiagnoser]
public class SortingBenchmarks
{
    private int[] _source = [];

    [Params(1_000, 100_000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        Random random = new(42);
        _source = Enumerable.Range(0, Size).Select(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] ArraySort() => Run(items => Array.Sort(items));

    [Benchmark]
    public int[] QuickSort() => Run(items => Sorting.QuickSort<int>(items));

    [Benchmark]
    public int[] MergeSort() => Run(items => Sorting.MergeSort<int>(items));

    [Benchmark]
    public int[] HeapSort() => Run(items => Sorting.HeapSort<int>(items));

    private int[] Run(Action<int[]> sort)
    {
        int[] items = (int[])_source.Clone();
        sort(items);
        return items;
    }
}

/// <summary>Insert then look up every key: separate chaining vs the runtime's Dictionary.</summary>
[MemoryDiagnoser]
public class HashMapBenchmarks
{
    private const int Size = 100_000;

    [Benchmark(Baseline = true)]
    public int Dictionary()
    {
        Dictionary<int, int> map = [];

        for (int i = 0; i < Size; i++) map[i] = i;

        int sum = 0;
        for (int i = 0; i < Size; i++) sum += map[i];
        return sum;
    }

    [Benchmark]
    public int HashMap()
    {
        HashMap<int, int> map = new();

        for (int i = 0; i < Size; i++) map[i] = i;

        int sum = 0;
        for (int i = 0; i < Size; i++) sum += map[i];
        return sum;
    }
}

/// <summary>
/// The case behind the employee autocomplete: find names by prefix among many.
/// A linear scan checks every name per query; the trie only walks the matching branch.
/// </summary>
[MemoryDiagnoser]
public class PrefixSearchBenchmarks
{
    private const int Limit = 10;

    private readonly string[] _prefixes = ["am", "jo", "sa", "moh", "kh"];
    private List<string> _names = [];
    private Trie<string> _trie = new();

    [Params(10_000, 200_000)]
    public int NameCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        string[] firstNames = ["Amjad", "John", "Sara", "Mohammad", "Khaled", "Lina", "Omar", "Rana", "Yousef", "Hala"];
        Random random = new(42);

        _names = Enumerable.Range(0, NameCount)
            .Select(i => $"{firstNames[random.Next(firstNames.Length)]}{i}")
            .ToList();

        _trie = new Trie<string>();
        _names.ForEach(name => _trie.Insert(name, name));
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() =>
        _prefixes.Sum(prefix => _names
            .Where(name => name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Order(StringComparer.OrdinalIgnoreCase)
            .Take(Limit)
            .Count());

    [Benchmark]
    public int Trie() => _prefixes.Sum(prefix => _trie.FindByPrefix(prefix, Limit).Count);
}
