using DataStructuresAlgorithms.DataStructures;

namespace DataStructuresAlgorithms.Tests;

public sealed class BinarySearchTreeTests
{
    [Fact]
    public void InOrder_ReturnsSortedDistinctValues()
    {
        BinarySearchTree<int> tree = new();

        foreach (int value in new[] { 50, 30, 70, 20, 40, 60, 80, 30 })
        {
            tree.Add(value);
        }

        Assert.Equal([20, 30, 40, 50, 60, 70, 80], tree.InOrder());
        Assert.Equal(7, tree.Count);
        Assert.Equal(20, tree.Min());
        Assert.Equal(80, tree.Max());
    }

    [Theory]
    [InlineData(20)] // leaf
    [InlineData(30)] // two children (20, 40)
    [InlineData(50)] // root with two children
    public void Remove_KeepsOrdering(int value)
    {
        BinarySearchTree<int> tree = new();

        foreach (int v in new[] { 50, 30, 70, 20, 40, 60, 80 })
        {
            tree.Add(v);
        }

        Assert.True(tree.Remove(value));
        Assert.False(tree.Contains(value));
        Assert.Equal(new[] { 20, 30, 40, 50, 60, 70, 80 }.Where(v => v != value), tree.InOrder());
        Assert.False(tree.Remove(value));
    }

    [Fact]
    public void SortedInput_DegeneratesIntoAList()
    {
        BinarySearchTree<int> tree = new();

        for (int i = 0; i < 100; i++)
        {
            tree.Add(i);
        }

        // Documents the known weakness of an unbalanced BST: height == n, not log n.
        Assert.Equal(100, tree.Height);
    }
}

public sealed class MinHeapTests
{
    [Fact]
    public void PopsInAscendingOrder()
    {
        Random random = new(7);
        int[] values = Enumerable.Range(0, 1_000).Select(_ => random.Next(10_000)).ToArray();
        MinHeap<int> heap = new();

        foreach (int value in values)
        {
            heap.Push(value);
        }

        Assert.Equal(values.Order(), Enumerable.Range(0, values.Length).Select(_ => heap.Pop()));
    }

    [Fact]
    public void Heapify_FromCollection_ProducesAValidHeap()
    {
        MinHeap<int> heap = new([5, 3, 9, 1, 7]);

        Assert.Equal(1, heap.Peek());
        Assert.Equal([1, 3, 5, 7, 9], Enumerable.Range(0, 5).Select(_ => heap.Pop()));
    }

    [Fact]
    public void ReversedComparer_MakesAMaxHeap()
    {
        MinHeap<int> heap = new([5, 3, 9, 1, 7], Comparer<int>.Create((a, b) => b.CompareTo(a)));

        Assert.Equal(9, heap.Pop());
        Assert.Equal(7, heap.Pop());
    }

    [Fact]
    public void EmptyHeap_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => new MinHeap<int>().Pop());
        Assert.Throws<InvalidOperationException>(() => new MinHeap<int>([]).Peek());
    }
}
