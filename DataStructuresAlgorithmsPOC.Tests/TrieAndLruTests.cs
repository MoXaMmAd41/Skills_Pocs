using DataStructuresAlgorithms.DataStructures;

namespace DataStructuresAlgorithms.Tests;

public sealed class TrieTests
{
    [Fact]
    public void FindByPrefix_ReturnsMatchesInAlphabeticalKeyOrder()
    {
        Trie<string> trie = new();

        foreach (string word in new[] { "car", "cart", "care", "cat", "dog" })
        {
            trie.Insert(word, word);
        }

        Assert.Equal(["car", "care", "cart"], trie.FindByPrefix("car"));
        Assert.Equal(["car", "care", "cart", "cat"], trie.FindByPrefix("ca"));
        Assert.Empty(trie.FindByPrefix("x"));
    }

    [Fact]
    public void FindByPrefix_IsCaseInsensitiveByDefault()
    {
        Trie<int> trie = new();
        trie.Insert("Amjad", 1);

        Assert.Equal([1], trie.FindByPrefix("AM"));
    }

    [Fact]
    public void FindByPrefix_ReturnsEachValueOnce_AndRespectsLimit()
    {
        Trie<int> trie = new();

        // One employee indexed under several keys, as the employee search does.
        trie.Insert("jane", 1);
        trie.Insert("jane doe", 1);
        trie.Insert("janet", 2);
        trie.Insert("janis", 3);

        Assert.Equal([1, 2, 3], trie.FindByPrefix("jan"));
        Assert.Equal([1, 2], trie.FindByPrefix("jan", limit: 2));
    }

    [Fact]
    public void Count_TracksDistinctKeys()
    {
        Trie<int> trie = new();
        trie.Insert("a", 1);
        trie.Insert("a", 2);
        trie.Insert("ab", 3);

        Assert.Equal(2, trie.Count);
        Assert.True(trie.ContainsPrefix("a"));
        Assert.False(trie.ContainsPrefix("b"));
    }

    [Fact]
    public void EmptyPrefix_MatchesEverything()
    {
        Trie<int> trie = new();
        trie.Insert("b", 2);
        trie.Insert("a", 1);

        Assert.Equal([1, 2], trie.FindByPrefix(""));
    }
}

public sealed class LruCacheTests
{
    [Fact]
    public void EvictsLeastRecentlyUsed_WhenFull()
    {
        LruCache<string, int> cache = new(capacity: 2);
        cache.Set("a", 1);
        cache.Set("b", 2);

        cache.TryGet("a", out _); // "a" is now most recent, so "b" is next to go

        Assert.Equal("b", cache.Set("c", 3)?.Key);
        Assert.False(cache.TryGet("b", out _));
        Assert.True(cache.TryGet("a", out int a));
        Assert.Equal(1, a);
        Assert.Equal(["a", "c"], cache.KeysByRecency());
    }

    [Fact]
    public void UpdatingAKey_RefreshesItWithoutEvicting()
    {
        LruCache<string, int> cache = new(capacity: 2);
        cache.Set("a", 1);
        cache.Set("b", 2);

        Assert.Null(cache.Set("a", 10));
        Assert.Equal("b", cache.Set("c", 3)?.Key);
        Assert.True(cache.TryGet("a", out int a));
        Assert.Equal(10, a);
    }

    [Fact]
    public void Remove_FreesCapacity()
    {
        LruCache<int, int> cache = new(capacity: 1);
        cache.Set(1, 1);

        Assert.True(cache.Remove(1));
        Assert.Null(cache.Set(2, 2));
        Assert.Equal(1, cache.Count);
    }

    [Fact]
    public void RejectsNonPositiveCapacity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LruCache<int, int>(0));
    }
}
