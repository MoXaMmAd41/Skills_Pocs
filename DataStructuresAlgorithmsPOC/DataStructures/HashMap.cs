using System.Collections;

namespace DataStructuresAlgorithms.DataStructures;

/// <summary>
/// Hash table using separate chaining: each bucket holds a linked chain of entries whose keys hash to it.
/// </summary>
/// <remarks>
/// Get/Add/Remove: O(1) on average, O(n) worst case (every key in one bucket).
/// The table doubles when the load factor passes 0.75 to keep chains short.
/// </remarks>
public sealed class HashMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private const int DefaultCapacity = 16;
    private const double MaxLoadFactor = 0.75;

    private readonly IEqualityComparer<TKey> _comparer;
    private Entry?[] _buckets;

    public HashMap(IEqualityComparer<TKey>? comparer = null, int capacity = DefaultCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);

        _comparer = comparer ?? EqualityComparer<TKey>.Default;
        _buckets = new Entry?[capacity];
    }

    public int Count { get; private set; }

    /// <summary>Number of buckets; doubles when <see cref="LoadFactor"/> would pass 0.75.</summary>
    public int Capacity => _buckets.Length;

    public double LoadFactor => (double)Count / _buckets.Length;

    public TValue this[TKey key]
    {
        get => TryGetValue(key, out TValue? value) ? value : throw new KeyNotFoundException($"Key '{key}' was not found.");
        set => Insert(key, value, overwrite: true);
    }

    public void Add(TKey key, TValue value) => Insert(key, value, overwrite: false);

    public bool ContainsKey(TKey key) => Find(key) is not null;

    public bool TryGetValue(TKey key, out TValue value)
    {
        Entry? entry = Find(key);
        value = entry is null ? default! : entry.Value;
        return entry is not null;
    }

    public bool Remove(TKey key)
    {
        int index = BucketOf(key, _buckets.Length);
        Entry? previous = null;

        for (Entry? entry = _buckets[index]; entry is not null; previous = entry, entry = entry.Next)
        {
            if (!_comparer.Equals(entry.Key, key))
            {
                continue;
            }

            if (previous is null)
            {
                _buckets[index] = entry.Next;
            }
            else
            {
                previous.Next = entry.Next;
            }

            Count--;
            return true;
        }

        return false;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (Entry? head in _buckets)
        {
            for (Entry? entry = head; entry is not null; entry = entry.Next)
            {
                yield return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void Insert(TKey key, TValue value, bool overwrite)
    {
        Entry? existing = Find(key);

        if (existing is not null)
        {
            if (!overwrite)
            {
                throw new ArgumentException($"An item with the key '{key}' already exists.", nameof(key));
            }

            existing.Value = value;
            return;
        }

        if (Count + 1 > _buckets.Length * MaxLoadFactor)
        {
            Resize(_buckets.Length * 2);
        }

        int index = BucketOf(key, _buckets.Length);
        _buckets[index] = new Entry(key, value, _buckets[index]);
        Count++;
    }

    private Entry? Find(TKey key)
    {
        for (Entry? entry = _buckets[BucketOf(key, _buckets.Length)]; entry is not null; entry = entry.Next)
        {
            if (_comparer.Equals(entry.Key, key))
            {
                return entry;
            }
        }

        return null;
    }

    private void Resize(int capacity)
    {
        Entry?[] buckets = new Entry?[capacity];

        foreach (Entry? head in _buckets)
        {
            Entry? entry = head;

            while (entry is not null)
            {
                Entry? next = entry.Next;
                int index = BucketOf(entry.Key, capacity);
                entry.Next = buckets[index];
                buckets[index] = entry;
                entry = next;
            }
        }

        _buckets = buckets;
    }

    // Mask the sign bit so negative hash codes still map to a valid bucket.
    private int BucketOf(TKey key, int capacity) => (_comparer.GetHashCode(key) & int.MaxValue) % capacity;

    private sealed class Entry(TKey key, TValue value, Entry? next)
    {
        public TKey Key { get; } = key;

        public TValue Value { get; set; } = value;

        public Entry? Next { get; set; } = next;
    }
}
