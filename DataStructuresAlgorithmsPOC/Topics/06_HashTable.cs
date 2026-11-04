namespace DataStructuresAlgorithms.Topics;

/// <summary>
/// Hash map: hash the key to pick a bucket; keys that land in the same bucket share a list (chaining).
/// Put / Get / Remove: O(1) on average, O(n) worst case if everything collides.
/// </summary>
public class MyHashMap
{
    private readonly List<(string Key, int Value)>[] _buckets = new List<(string, int)>[16];

    private List<(string Key, int Value)> BucketFor(string key)
    {
        int index = (key.GetHashCode() & int.MaxValue) % _buckets.Length;
        return _buckets[index] ??= [];
    }

    public void Put(string key, int value)
    {
        List<(string Key, int Value)> bucket = BucketFor(key);
        int i = bucket.FindIndex(e => e.Key == key);

        if (i >= 0) bucket[i] = (key, value); // update
        else bucket.Add((key, value));        // insert
    }

    public bool TryGet(string key, out int value)
    {
        foreach ((string k, int v) in BucketFor(key))
        {
            if (k == key)
            {
                value = v;
                return true;
            }
        }

        value = 0;
        return false;
    }

    public bool Remove(string key) => BucketFor(key).RemoveAll(e => e.Key == key) > 0;
}

/// <summary>Hash set: a hash map that only stores keys. Great for "have I seen this before?".</summary>
public class MyHashSet
{
    private readonly MyHashMap _map = new();

    public void Add(string item) => _map.Put(item, 0);
    public bool Contains(string item) => _map.TryGet(item, out _);
    public bool Remove(string item) => _map.Remove(item);
}

public static class HashTable
{
    /// <summary>Two Sum with a dictionary: O(n) instead of the O(n²) double loop.</summary>
    public static (int, int)? TwoSum(int[] numbers, int target)
    {
        Dictionary<int, int> indexOf = [];

        for (int i = 0; i < numbers.Length; i++)
        {
            if (indexOf.TryGetValue(target - numbers[i], out int j)) return (j, i);
            indexOf[numbers[i]] = i;
        }

        return null;
    }

    public static void Demo()
    {
        MyHashMap ages = new();
        ages.Put("Ali", 25);
        ages.Put("Sara", 30);
        ages.Put("Ali", 26);
        ages.TryGet("Ali", out int ali);
        Console.WriteLine($"HashMap: Put Ali=25, Sara=30, Ali=26 -> Get Ali = {ali}");
        Console.WriteLine($"HashMap: Get Omar found? {ages.TryGet("Omar", out _)}");

        MyHashSet seen = new();
        seen.Add("apple");
        seen.Add("apple");
        Console.WriteLine($"HashSet: Add apple twice -> Contains apple = {seen.Contains("apple")}, Contains kiwi = {seen.Contains("kiwi")}");

        Dictionary<string, int> wordCount = [];
        foreach (string word in "the cat and the hat".Split(' '))
        {
            wordCount[word] = wordCount.GetValueOrDefault(word) + 1;
        }

        Console.WriteLine($"Dictionary word count -> {string.Join(", ", wordCount.Select(p => $"{p.Key}:{p.Value}"))}");
        Console.WriteLine($"Two Sum [2, 7, 11, 15], target 9 -> indexes {TwoSum([2, 7, 11, 15], 9)}");
    }
}
