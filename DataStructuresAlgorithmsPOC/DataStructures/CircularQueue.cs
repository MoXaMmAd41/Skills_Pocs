using System.Collections;

namespace DataStructuresAlgorithms.DataStructures;

/// <summary>
/// FIFO queue on a ring buffer: head and tail wrap around the array, so dequeuing never shifts elements.
/// </summary>
/// <remarks>Enqueue: amortised O(1). Dequeue/Peek: O(1).</remarks>
public sealed class CircularQueue<T> : IReadOnlyCollection<T>
{
    private const int DefaultCapacity = 4;

    private T[] _items = new T[DefaultCapacity];
    private int _head;

    public int Count { get; private set; }

    public bool IsEmpty => Count == 0;

    /// <summary>Size of the ring buffer.</summary>
    public int Capacity => _items.Length;

    /// <summary>Array slot holding the front item; item <c>i</c> lives in slot <c>(HeadIndex + i) % Capacity</c>.</summary>
    public int HeadIndex => _head;

    public void Enqueue(T item)
    {
        if (Count == _items.Length)
        {
            Grow();
        }

        _items[(_head + Count) % _items.Length] = item;
        Count++;
    }

    public T Dequeue() => TryDequeue(out T? item) ? item : throw new InvalidOperationException("The queue is empty.");

    public bool TryDequeue(out T item)
    {
        if (IsEmpty)
        {
            item = default!;
            return false;
        }

        item = _items[_head];
        _items[_head] = default!;
        _head = (_head + 1) % _items.Length;
        Count--;
        return true;
    }

    public T Peek() => IsEmpty ? throw new InvalidOperationException("The queue is empty.") : _items[_head];

    /// <summary>Enumerates from the front (next to dequeue) to the back.</summary>
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return _items[(_head + i) % _items.Length];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // Unwrap into a larger array so the logical order starts at index 0 again.
    private void Grow()
    {
        T[] larger = new T[_items.Length * 2];

        for (int i = 0; i < Count; i++)
        {
            larger[i] = _items[(_head + i) % _items.Length];
        }

        _items = larger;
        _head = 0;
    }
}
