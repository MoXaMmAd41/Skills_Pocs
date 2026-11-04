namespace DataStructuresAlgorithms.Topics;

/// <summary>
/// A dynamic array (like List&lt;T&gt;): a fixed array that doubles in size when it is full.
/// Get by index O(1) | Add at end O(1) | Insert / Delete O(n) (elements shift) | Search O(n)
/// </summary>
public class DynamicArray
{
    private int[] _items = new int[4];

    public int Count { get; private set; }

    public int this[int index] => _items[index];

    public void Add(int value) => Insert(Count, value);

    public void Insert(int index, int value)
    {
        if (index < 0 || index > Count) throw new ArgumentOutOfRangeException(nameof(index));

        if (Count == _items.Length)
        {
            Array.Resize(ref _items, _items.Length * 2);
        }

        // Shift everything after index one step right to make room.
        for (int i = Count; i > index; i--)
        {
            _items[i] = _items[i - 1];
        }

        _items[index] = value;
        Count++;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));

        // Shift everything after index one step left to close the gap.
        for (int i = index; i < Count - 1; i++)
        {
            _items[i] = _items[i + 1];
        }

        Count--;
    }

    public int IndexOf(int value)
    {
        for (int i = 0; i < Count; i++)
        {
            if (_items[i] == value) return i;
        }

        return -1;
    }

    public override string ToString() => "[" + string.Join(", ", _items.Take(Count)) + "]";
}

public static class Arrays
{
    public static void Demo()
    {
        DynamicArray array = new();
        array.Add(10);
        array.Add(20);
        array.Add(30);
        Console.WriteLine($"Add 10, 20, 30      -> {array}");

        array.Insert(1, 15);
        Console.WriteLine($"Insert 15 at 1      -> {array}");

        array.RemoveAt(0);
        Console.WriteLine($"Delete at 0         -> {array}");

        Console.WriteLine($"Search 30           -> index {array.IndexOf(30)}");
        Console.WriteLine($"Search 99           -> index {array.IndexOf(99)}");

        Console.Write("Traverse            -> ");
        for (int i = 0; i < array.Count; i++)
        {
            Console.Write(array[i] + " ");
        }

        Console.WriteLine();
    }
}
