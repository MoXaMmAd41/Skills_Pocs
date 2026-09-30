namespace DataStructuresAlgorithms.DataStructures;

/// <summary>
/// Binary min-heap stored in an array: the children of index i live at 2i+1 and 2i+2,
/// and every parent is ≤ its children, so the minimum is always at index 0.
/// Pass a reversed comparer for a max-heap.
/// </summary>
/// <remarks>Push/Pop: O(log n). Peek: O(1). Building from a collection: O(n).</remarks>
public sealed class MinHeap<T>
{
    private readonly IComparer<T> _comparer;
    private T[] _items;

    public MinHeap(IComparer<T>? comparer = null)
    {
        _comparer = comparer ?? Comparer<T>.Default;
        _items = new T[4];
    }

    /// <summary>Bottom-up heapify: sifting down from the last parent is O(n), cheaper than n pushes.</summary>
    public MinHeap(IEnumerable<T> items, IComparer<T>? comparer = null)
    {
        _comparer = comparer ?? Comparer<T>.Default;
        _items = items.ToArray();
        Count = _items.Length;

        if (_items.Length == 0)
        {
            _items = new T[4];
        }

        for (int i = Count / 2 - 1; i >= 0; i--)
        {
            SiftDown(i);
        }
    }

    public int Count { get; private set; }

    public bool IsEmpty => Count == 0;

    /// <summary>
    /// Items in backing-array order (the heap layout, not sorted): index 0 is the minimum and the
    /// children of index i are at 2i+1 and 2i+2. Mirrors <c>PriorityQueue.UnorderedItems</c>.
    /// </summary>
    public IEnumerable<T> UnorderedItems => _items.Take(Count);

    public void Push(T item)
    {
        if (Count == _items.Length)
        {
            Array.Resize(ref _items, _items.Length * 2);
        }

        _items[Count] = item;
        SiftUp(Count++);
    }

    public T Peek() => IsEmpty ? throw new InvalidOperationException("The heap is empty.") : _items[0];

    public T Pop() => TryPop(out T? item) ? item : throw new InvalidOperationException("The heap is empty.");

    public bool TryPop(out T item)
    {
        if (IsEmpty)
        {
            item = default!;
            return false;
        }

        item = _items[0];
        _items[0] = _items[--Count];
        _items[Count] = default!;
        SiftDown(0);
        return true;
    }

    private void SiftUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;

            if (_comparer.Compare(_items[index], _items[parent]) >= 0)
            {
                return;
            }

            (_items[index], _items[parent]) = (_items[parent], _items[index]);
            index = parent;
        }
    }

    private void SiftDown(int index)
    {
        while (true)
        {
            int left = 2 * index + 1;
            int right = left + 1;
            int smallest = index;

            if (left < Count && _comparer.Compare(_items[left], _items[smallest]) < 0)
            {
                smallest = left;
            }

            if (right < Count && _comparer.Compare(_items[right], _items[smallest]) < 0)
            {
                smallest = right;
            }

            if (smallest == index)
            {
                return;
            }

            (_items[index], _items[smallest]) = (_items[smallest], _items[index]);
            index = smallest;
        }
    }
}
