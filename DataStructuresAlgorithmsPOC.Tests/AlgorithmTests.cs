using DataStructuresAlgorithms.Algorithms;
using DataStructuresAlgorithms.DataStructures;

namespace DataStructuresAlgorithms.Tests;

public sealed class SortingTests
{
    public delegate void SortMethod(Span<int> items, IComparer<int>? comparer);

    public static TheoryData<string> Algorithms => ["Insertion", "Quick", "Merge", "Heap"];

    private static readonly Dictionary<string, SortMethod> Sorts = new()
    {
        ["Insertion"] = Sorting.InsertionSort,
        ["Quick"] = Sorting.QuickSort,
        ["Merge"] = Sorting.MergeSort,
        ["Heap"] = Sorting.HeapSort
    };

    [Theory]
    [MemberData(nameof(Algorithms))]
    public void MatchesArraySort_OnRandomInput(string algorithm)
    {
        Random random = new(123);

        foreach (int length in new[] { 0, 1, 2, 15, 16, 17, 100, 1_000 })
        {
            int[] actual = Enumerable.Range(0, length).Select(_ => random.Next(-500, 500)).ToArray();
            int[] expected = [.. actual];
            Array.Sort(expected);

            Sorts[algorithm](actual, null);

            Assert.Equal(expected, actual);
        }
    }

    [Theory]
    [MemberData(nameof(Algorithms))]
    public void HandlesSortedReversedAndConstantInput(string algorithm)
    {
        int[][] inputs =
        [
            Enumerable.Range(0, 500).ToArray(),
            Enumerable.Range(0, 500).Reverse().ToArray(),
            Enumerable.Repeat(7, 500).ToArray()
        ];

        foreach (int[] input in inputs)
        {
            int[] expected = [.. input];
            Array.Sort(expected);

            Sorts[algorithm](input, null);

            Assert.Equal(expected, input);
        }
    }

    [Theory]
    [MemberData(nameof(Algorithms))]
    public void RespectsCustomComparer(string algorithm)
    {
        int[] items = [3, 1, 2];

        Sorts[algorithm](items, Comparer<int>.Create((a, b) => b.CompareTo(a)));

        Assert.Equal([3, 2, 1], items);
    }

    [Fact]
    public void MergeSort_IsStable()
    {
        (int Key, char Tag)[] items = [.. Enumerable.Range(0, 200).Select(i => (i % 5, (char)('a' + i % 26)))];
        (int Key, char Tag)[] expected = [.. items.OrderBy(i => i.Key)]; // LINQ OrderBy is stable

        Sorting.MergeSort<(int Key, char Tag)>(items, Comparer<(int Key, char Tag)>.Create((a, b) => a.Key.CompareTo(b.Key)));

        Assert.Equal(expected, items);
    }
}

public sealed class SearchingTests
{
    private static readonly int[] Sorted = [1, 3, 3, 3, 7, 9];

    [Theory]
    [InlineData(1, 0)]
    [InlineData(9, 5)]
    [InlineData(7, 4)]
    [InlineData(4, -1)]
    [InlineData(0, -1)]
    [InlineData(10, -1)]
    public void BinarySearch_FindsIndexOrMinusOne(int value, int expected)
    {
        Assert.Equal(expected, Searching.BinarySearch<int>(Sorted, value));
    }

    [Fact]
    public void BinarySearch_ReturnsFirstOccurrenceOfDuplicates()
    {
        Assert.Equal(1, Searching.BinarySearch<int>(Sorted, 3));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(3, 1)]
    [InlineData(4, 4)]
    [InlineData(100, 6)]
    public void LowerBound_ReturnsInsertionPoint(int value, int expected)
    {
        Assert.Equal(expected, Searching.LowerBound<int>(Sorted, value));
    }

    [Fact]
    public void BinarySearch_OnEmptySpan_ReturnsMinusOne()
    {
        Assert.Equal(-1, Searching.BinarySearch<int>([], 1));
    }
}

public sealed class GraphAlgorithmTests
{
    //   A --1-- B --1-- D
    //   |               |
    //   4               1
    //   |               |
    //   C ------1------ E        F (isolated)
    private static Graph<string> CreateGraph()
    {
        Graph<string> graph = new();
        graph.AddEdge("A", "B", 1);
        graph.AddEdge("A", "C", 4);
        graph.AddEdge("B", "D", 1);
        graph.AddEdge("D", "E", 1);
        graph.AddEdge("C", "E", 1);
        graph.AddVertex("F");
        return graph;
    }

    [Fact]
    public void BreadthFirst_VisitsLevelByLevel()
    {
        Assert.Equal(["A", "B", "C", "D", "E"], GraphAlgorithms.BreadthFirst(CreateGraph(), "A"));
    }

    [Fact]
    public void DepthFirst_FollowsEachBranchToTheEnd()
    {
        Assert.Equal(["A", "B", "D", "E", "C"], GraphAlgorithms.DepthFirst(CreateGraph(), "A"));
    }

    [Fact]
    public void ShortestPathByEdges_MinimisesHops()
    {
        Assert.Equal(["A", "C", "E"], GraphAlgorithms.ShortestPathByEdges(CreateGraph(), "A", "E"));
    }

    [Fact]
    public void ShortestPathByWeight_MinimisesCost()
    {
        WeightedPath<string>? path = GraphAlgorithms.ShortestPathByWeight(CreateGraph(), "A", "E");

        Assert.NotNull(path);
        Assert.Equal(["A", "B", "D", "E"], path.Vertices);
        Assert.Equal(3, path.Cost);
    }

    [Fact]
    public void Paths_ToUnreachableVertex_AreNull()
    {
        Graph<string> graph = CreateGraph();

        Assert.Null(GraphAlgorithms.ShortestPathByEdges(graph, "A", "F"));
        Assert.Null(GraphAlgorithms.ShortestPathByWeight(graph, "A", "F"));
    }

    [Fact]
    public void Path_ToSelf_IsSingleVertexWithZeroCost()
    {
        WeightedPath<string>? path = GraphAlgorithms.ShortestPathByWeight(CreateGraph(), "A", "A");

        Assert.Equal(["A"], path!.Vertices);
        Assert.Equal(0, path.Cost);
    }

    [Fact]
    public void UnknownVertex_Throws()
    {
        Assert.Throws<KeyNotFoundException>(() => GraphAlgorithms.BreadthFirst(CreateGraph(), "Z").ToList());
    }

    [Fact]
    public void TopologicalSort_OrdersDependenciesFirst()
    {
        Graph<string> build = new(directed: true);
        build.AddEdge("Domain", "Application");
        build.AddEdge("Application", "Infrastructure");
        build.AddEdge("Infrastructure", "Api");
        build.AddEdge("Application", "Api");

        Assert.Equal(["Domain", "Application", "Infrastructure", "Api"], GraphAlgorithms.TopologicalSort(build));
    }

    [Fact]
    public void TopologicalSort_DetectsCycles()
    {
        Graph<int> graph = new(directed: true);
        graph.AddEdge(1, 2);
        graph.AddEdge(2, 3);
        graph.AddEdge(3, 1);

        Assert.Throws<InvalidOperationException>(() => GraphAlgorithms.TopologicalSort(graph));
    }
}
