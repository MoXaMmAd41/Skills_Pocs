namespace DataStructuresAlgorithms.DataStructures;

/// <summary>
/// Fixed-capacity cache that evicts the least recently used entry when full.
/// A hash map gives O(1) lookup of the list node; the linked list keeps recency order
/// (front = most recent, back = next to evict), and moving a node is O(1).
/// </summary>
/// <remarks>Get/Set/Remove: O(1). Not thread-safe; wrap with a lock for concurrent use.</remarks>
public sealed class LruCache<TKey, TValue>
    where TKey : notnull
{
    private readonly HashMap<TKey, DoublyLinkedListNode<KeyValuePair<TKey, TValue>>> _map = new();
    private readonly DoublyLinkedList<KeyValuePair<TKey, TValue>> _recency = new();

    public LruCache(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        Capacity = capacity;
    }

    public int Capacity { get; }

    public int Count => _map.Count;

    public bool TryGet(TKey key, out TValue value)
    {
        if (!_map.TryGetValue(key, out DoublyLinkedListNode<KeyValuePair<TKey, TValue>>? node))
        {
            value = default!;
            return false;
        }

        _recency.MoveToFront(node);
        value = node.Value.Value;
        return true;
    }

    /// <returns>
    /// The entry evicted to make room, or null. (A nullable pair rather than <c>TKey?</c>, which for
    /// value-type keys would be <c>default</c> and indistinguishable from a real key such as 0.)
    /// </returns>
    public KeyValuePair<TKey, TValue>? Set(TKey key, TValue value)
    {
        if (_map.TryGetValue(key, out DoublyLinkedListNode<KeyValuePair<TKey, TValue>>? existing))
        {
            existing.Value = new KeyValuePair<TKey, TValue>(key, value);
            _recency.MoveToFront(existing);
            return null;
        }

        KeyValuePair<TKey, TValue>? evicted = null;

        if (_map.Count == Capacity)
        {
            evicted = _recency.RemoveLast();
            _map.Remove(evicted.Value.Key);
        }

        _map[key] = _recency.AddFirst(new KeyValuePair<TKey, TValue>(key, value));
        return evicted;
    }

    public bool Remove(TKey key)
    {
        if (!_map.TryGetValue(key, out DoublyLinkedListNode<KeyValuePair<TKey, TValue>>? node))
        {
            return false;
        }

        _recency.Remove(node);
        return _map.Remove(key);
    }

    /// <summary>Keys from most to least recently used.</summary>
    public IEnumerable<TKey> KeysByRecency() => _recency.Select(entry => entry.Key);
}
