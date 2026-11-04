namespace DataStructuresAlgorithms.Topics;

/// <summary>
/// Circular queue = FIFO (First In, First Out) on a fixed-size array.
/// Head and tail wrap around with % capacity, so Dequeue never shifts elements: O(1).
/// </summary>
public class CircularQueue(int capacity)
{
    private readonly int[] _items = new int[capacity];
    private int _head; // index of the front item
    private int _tail; // index where the next item goes

    public int Count { get; private set; }

    public bool IsFull => Count == _items.Length;

    public void Enqueue(int value)
    {
        if (IsFull) throw new InvalidOperationException("Queue is full.");

        _items[_tail] = value;
        _tail = (_tail + 1) % _items.Length;
        Count++;
    }

    public int Dequeue()
    {
        int front = Peek();
        _head = (_head + 1) % _items.Length;
        Count--;
        return front;
    }

    public int Peek() => Count > 0 ? _items[_head] : throw new InvalidOperationException("Queue is empty.");

    public override string ToString() =>
        "front -> [" + string.Join(", ", Enumerable.Range(0, Count).Select(i => _items[(_head + i) % _items.Length])) + "]";
}

/// <summary>Deque (double-ended queue): add and remove at BOTH ends in O(1). Built on a doubly linked list.</summary>
public class Deque
{
    private readonly DoublyLinkedList _list = new();

    public void AddFront(int value) => _list.AddFirst(value);
    public void AddBack(int value) => _list.AddLast(value);
    public int RemoveFront() => _list.RemoveFirst();
    public int RemoveBack() => _list.RemoveLast();

    public override string ToString() => "[" + string.Join(", ", _list.ToList()) + "]";
}

public static class QueueTopic
{
    public static void Demo()
    {
        Console.WriteLine("Simple queue (built-in Queue<T>):");
        Queue<string> line = new();
        line.Enqueue("Ali");
        line.Enqueue("Sara");
        line.Enqueue("Omar");
        Console.WriteLine($"  Enqueue Ali, Sara, Omar -> served first: {line.Dequeue()}, then {line.Dequeue()}");

        Console.WriteLine("Circular queue (capacity 3):");
        CircularQueue queue = new(3);
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);
        Console.WriteLine($"  Enqueue 1, 2, 3 -> {queue}  full = {queue.IsFull}");
        Console.WriteLine($"  Dequeue -> {queue.Dequeue()}  now {queue}");
        queue.Enqueue(4);
        Console.WriteLine($"  Enqueue 4 (reuses the freed slot) -> {queue}");

        Console.WriteLine("Deque:");
        Deque deque = new();
        deque.AddBack(2);
        deque.AddBack(3);
        deque.AddFront(1);
        Console.WriteLine($"  AddBack 2, 3 + AddFront 1 -> {deque}");
        Console.WriteLine($"  RemoveFront = {deque.RemoveFront()}, RemoveBack = {deque.RemoveBack()} -> {deque}");
    }
}
