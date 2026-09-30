using DataStructuresAlgorithms.DataStructures;

namespace DataStructuresAlgorithms.Tests;

/// <summary>Read-only views used to visualise the structures (e.g. by the Playground).</summary>
public sealed class InspectionTests
{
    [Fact]
    public void Stack_EnumeratesTopToBottom_AndReportsCapacity()
    {
        ArrayStack<int> stack = new();

        foreach (int i in new[] { 1, 2, 3, 4, 5 })
        {
            stack.Push(i);
        }

        Assert.Equal([5, 4, 3, 2, 1], stack);
        Assert.Equal(8, stack.Capacity);
    }

    [Fact]
    public void Queue_EnumeratesFrontToBack_AfterWrapping()
    {
        CircularQueue<int> queue = new();

        foreach (int i in new[] { 1, 2, 3 }) queue.Enqueue(i);
        queue.Dequeue();
        queue.Dequeue();
        foreach (int i in new[] { 4, 5 }) queue.Enqueue(i);

        Assert.Equal([3, 4, 5], queue);
        Assert.Equal(2, queue.HeadIndex);
        Assert.Equal(4, queue.Capacity);
    }

    [Fact]
    public void Heap_UnorderedItems_ExposeHeapLayout()
    {
        MinHeap<int> heap = new();

        foreach (int i in new[] { 7, 3, 9, 1, 5 })
        {
            heap.Push(i);
        }

        int[] items = heap.UnorderedItems.ToArray();

        Assert.Equal(1, items[0]);

        for (int i = 1; i < items.Length; i++)
        {
            Assert.True(items[(i - 1) / 2] <= items[i], "Every parent must be ≤ its children.");
        }
    }

    [Fact]
    public void Tree_PreOrder_ReportsDepthAndSide()
    {
        BinarySearchTree<int> tree = new();

        foreach (int i in new[] { 50, 30, 70, 20 })
        {
            tree.Add(i);
        }

        Assert.Equal(
            [(50, 0, TreeSide.Root), (30, 1, TreeSide.Left), (20, 2, TreeSide.Left), (70, 1, TreeSide.Right)],
            tree.PreOrder());
    }

    [Fact]
    public void HashMap_ReportsCapacityAndLoadFactor()
    {
        HashMap<int, int> map = new(capacity: 4);

        foreach (int i in new[] { 1, 2, 3 })
        {
            map[i] = i;
        }

        Assert.Equal(4, map.Capacity);
        Assert.Equal(0.75, map.LoadFactor);

        map[4] = 4; // passes 0.75 → doubles

        Assert.Equal(8, map.Capacity);
    }
}
