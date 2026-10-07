using System.Diagnostics;
using DataStructuresAlgorithms.Algorithms;

namespace DataStructuresAlgorithms.Playground.Demos;

internal sealed class SortingDemo : CommandDemo
{
    private delegate void Sort(Span<int> items, IComparer<int>? comparer);

    private static readonly (string Name, Sort Run)[] Algorithms =
    [
        ("Insertion sort", Sorting.InsertionSort),
        ("Quick sort", Sorting.QuickSort),
        ("Merge sort", Sorting.MergeSort),
        ("Heap sort", Sorting.HeapSort)
    ];

    public override string Title => "Sorting — same input, four algorithms";

    protected override IReadOnlyList<string> Commands =>
    [
        "sort <n> [n...]       sort your numbers with every algorithm",
        "random <count>        sort <count> random numbers and compare the work done",
        "sorted <count>        already-sorted input (best/worst cases differ!)"
    ];

    protected override void Execute(string command, string[] args)
    {
        switch (command)
        {
            case "sort":
                Compare(Numbers(args), showResult: true);
                break;
            case "random":
                Random random = new();
                Compare(Enumerable.Range(0, Count(args)).Select(_ => random.Next(1_000)).ToArray(), showResult: false);
                break;
            case "sorted":
                Compare(Enumerable.Range(0, Count(args)).ToArray(), showResult: false);
                break;
            default:
                UnknownCommand(command);
                break;
        }
    }

    protected override void Render()
    {
    }

    private static int Count(string[] args)
    {
        int count = Number(args);
        return count is >= 1 and <= 200_000 ? count : throw new FormatException("Count must be between 1 and 200,000.");
    }

    private static void Compare(int[] input, bool showResult)
    {
        if (showResult)
        {
            Ui.State($"input:  {Ui.List(input)}");
        }

        foreach ((string name, Sort run) in Algorithms)
        {
            // Insertion sort is O(n²): skip it on big inputs instead of freezing the console.
            if (name.StartsWith("Insertion") && input.Length > 20_000)
            {
                Ui.Muted($"{name,-15} skipped (O(n²) would take too long for {input.Length:N0} items)");
                continue;
            }

            int[] copy = [.. input];
            CountingComparer<int> comparer = new();
            Stopwatch timer = Stopwatch.StartNew();

            run(copy, comparer);

            timer.Stop();
            string result = showResult ? $"  → {Ui.List(copy)}" : string.Empty;
            Ui.Result($"{name,-15} {comparer.Comparisons,12:N0} comparisons {timer.Elapsed.TotalMilliseconds,9:0.000} ms{result}");
        }

        Ui.Muted($"n = {input.Length:N0}; n·log₂n ≈ {input.Length * Math.Log2(Math.Max(2, input.Length)):N0}, n² = {(long)input.Length * input.Length:N0}");
    }
}

internal sealed class BinarySearchDemo : CommandDemo
{
    private int[] _data = [2, 5, 8, 12, 16, 23, 38, 56, 72, 91];

    public override string Title => "Binary search";

    protected override IReadOnlyList<string> Commands =>
    [
        "data <n> [n...]       replace the data (it is sorted for you)",
        "find <n>              binary search, counting the comparisons",
        "range <count>         use 1..count as data (try 1000000 and find a number)"
    ];

    protected override void Execute(string command, string[] args)
    {
        switch (command)
        {
            case "data":
                _data = Numbers(args).Order().ToArray();
                break;
            case "range":
                int count = Number(args);
                _data = count is >= 1 and <= 10_000_000
                    ? Enumerable.Range(1, count).ToArray()
                    : throw new FormatException("Count must be between 1 and 10,000,000.");
                break;
            case "find":
                int value = Number(args);
                CountingComparer<int> comparer = new();
                int index = Searching.BinarySearch<int>(_data, value, comparer);
                int insertAt = Searching.LowerBound<int>(_data, value);

                Ui.Result(index >= 0
                    ? $"Found {value} at index {index} after {comparer.Comparisons} comparisons."
                    : $"{value} not found ({comparer.Comparisons} comparisons); it would be inserted at index {insertAt}.");
                Ui.Muted($"A linear scan could need up to {_data.Length:N0}; log₂({_data.Length:N0}) ≈ {Math.Log2(_data.Length):0.0}");
                break;
            default:
                UnknownCommand(command);
                break;
        }
    }

    protected override void Render() =>
        Ui.State(_data.Length <= 30 ? $"data: {Ui.List(_data)}" : $"data: 1 .. {_data[^1]:N0} ({_data.Length:N0} sorted numbers)");
}

