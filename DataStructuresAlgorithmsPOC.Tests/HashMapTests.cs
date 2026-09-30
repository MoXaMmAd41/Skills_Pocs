using DataStructuresAlgorithms.DataStructures;

namespace DataStructuresAlgorithms.Tests;

public sealed class HashMapTests
{
    [Fact]
    public void BehavesLikeDictionary_UnderRandomOperations()
    {
        HashMap<int, int> map = new();
        Dictionary<int, int> expected = [];
        Random random = new(42);

        for (int i = 0; i < 20_000; i++)
        {
            int key = random.Next(2_000);

            switch (random.Next(3))
            {
                case 0:
                    map[key] = i;
                    expected[key] = i;
                    break;
                case 1:
                    Assert.Equal(expected.Remove(key), map.Remove(key));
                    break;
                default:
                    Assert.Equal(expected.TryGetValue(key, out int value), map.TryGetValue(key, out int actual));
                    Assert.Equal(value, actual);
                    break;
            }
        }

        Assert.Equal(expected.Count, map.Count);
        Assert.Equal(expected.OrderBy(p => p.Key), map.OrderBy(p => p.Key));
    }

    [Fact]
    public void Add_ThrowsOnDuplicateKey_IndexerOverwrites()
    {
        HashMap<string, int> map = new() { { "a", 1 } };

        Assert.Throws<ArgumentException>(() => map.Add("a", 2));

        map["a"] = 3;
        Assert.Equal(3, map["a"]);
    }

    [Fact]
    public void MissingKey_ThrowsKeyNotFound()
    {
        Assert.Throws<KeyNotFoundException>(() => new HashMap<string, int>()["missing"]);
    }

    [Fact]
    public void HandlesCollisions_AndNegativeHashCodes()
    {
        // Every key collides and hashes negative, so all entries share one chain.
        HashMap<int, string> map = new(new ConstantHashComparer());

        for (int i = 0; i < 50; i++)
        {
            map[i] = i.ToString();
        }

        Assert.True(map.Remove(25));
        Assert.False(map.ContainsKey(25));
        Assert.Equal("49", map[49]);
        Assert.Equal(49, map.Count);
    }

    [Fact]
    public void UsesProvidedComparer()
    {
        HashMap<string, int> map = new(StringComparer.OrdinalIgnoreCase) { ["Key"] = 1 };

        Assert.True(map.ContainsKey("KEY"));
    }

    private sealed class ConstantHashComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => x == y;

        public int GetHashCode(int obj) => -7;
    }
}
