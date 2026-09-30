using System.Collections;

namespace DataStructuresAlgorithms.DataStructures;

/// <summary>LIFO stack backed by a growable array.</summary>
/// <remarks>Push: amortised O(1) (doubles on growth). Pop/Peek: O(1).</remarks>
public sealed class ArrayStack<T> : IReadOnlyCollection<T>
{
    private const int DefaultCapacity = 4;

    private T[] _items = new T[DefaultCapacity];

    public int Count { get; private set; }

    public bool IsEmpty => Count == 0;

    /// <summary>Size of the backing array; grows by doubling when full.</summary>
    public int Capacity => _items.Length;

    public void Push(T item)
    {
        if (Count == _items.Length)
        {
            Array.Resize(ref _items, _items.Length * 2);
        }

        _items[Count++] = item;
    }

    public T Pop() => TryPop(out T? item) ? item : throw new InvalidOperationException("The stack is empty.");

    public bool TryPop(out T item)
    {
        if (IsEmpty)
        {
            item = default!;
            return false;
        }

        item = _items[--Count];
        _items[Count] = default!; // release the reference for the GC
        return true;
    }

    public T Peek() => IsEmpty ? throw new InvalidOperationException("The stack is empty.") : _items[Count - 1];

    /// <summary>Enumerates from the top (next to pop) to the bottom.</summary>
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = Count - 1; i >= 0; i--)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
