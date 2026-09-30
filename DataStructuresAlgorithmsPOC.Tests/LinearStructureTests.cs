using DataStructuresAlgorithms.DataStructures;

namespace DataStructuresAlgorithms.Tests;

public sealed class ArrayStackTests
{
    [Fact]
    public void PopsInReverseOrderOfPush_AcrossGrowth()
    {
        ArrayStack<int> stack = new();

        for (int i = 0; i < 100; i++)
        {
            stack.Push(i);
        }

        Assert.Equal(99, stack.Peek());
        Assert.Equal(Enumerable.Range(0, 100).Reverse(), Enumerable.Range(0, 100).Select(_ => stack.Pop()));
        Assert.True(stack.IsEmpty);
    }

    [Fact]
    public void EmptyStack_ThrowsOnPopAndPeek_AndTryPopReturnsFalse()
    {
        ArrayStack<string> stack = new();

        Assert.Throws<InvalidOperationException>(() => stack.Pop());
        Assert.Throws<InvalidOperationException>(() => stack.Peek());
        Assert.False(stack.TryPop(out _));
    }
}

public sealed class CircularQueueTests
{
    [Fact]
    public void DequeuesInFifoOrder_WhenTheBufferWrapsAndGrows()
    {
        CircularQueue<int> queue = new();
        List<int> dequeued = [];

        // Interleave so head moves forward and the tail wraps before the buffer grows.
        for (int i = 0; i < 50; i++)
        {
            queue.Enqueue(i);

            if (i % 3 == 0)
            {
                dequeued.Add(queue.Dequeue());
            }
        }

        while (queue.TryDequeue(out int item))
        {
            dequeued.Add(item);
        }

        Assert.Equal(Enumerable.Range(0, 50), dequeued);
    }

    [Fact]
    public void EmptyQueue_Throws()
    {
        CircularQueue<int> queue = new();

        Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        Assert.Throws<InvalidOperationException>(() => queue.Peek());
    }
}

public sealed class DoublyLinkedListTests
{
    [Fact]
    public void MaintainsOrder_ForAddsAtBothEnds()
    {
        DoublyLinkedList<int> list = new();
        list.AddLast(2);
        list.AddFirst(1);
        list.AddLast(3);

        Assert.Equal([1, 2, 3], list);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void RemoveAndMoveToFront_RelinkNeighbours()
    {
        DoublyLinkedList<string> list = new();
        list.AddLast("a");
        DoublyLinkedListNode<string> b = list.AddLast("b");
        DoublyLinkedListNode<string> c = list.AddLast("c");

        list.MoveToFront(c);
        Assert.Equal(["c", "a", "b"], list);

        list.Remove(b);
        Assert.Equal(["c", "a"], list);
        Assert.Equal("a", list.Last!.Value);
        Assert.Null(list.Last.Next);
    }

    [Fact]
    public void RejectsNodesFromAnotherList()
    {
        DoublyLinkedList<int> first = new();
        DoublyLinkedList<int> second = new();
        DoublyLinkedListNode<int> node = first.AddLast(1);

        Assert.Throws<InvalidOperationException>(() => second.Remove(node));
    }
}
