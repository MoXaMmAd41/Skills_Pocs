namespace DataStructuresAlgorithms.Topics;

public static class Searching
{
    /// <summary>Check every element. Works on any array. O(n).</summary>
    public static int LinearSearch(int[] array, int target)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == target) return i;
        }

        return -1;
    }

    /// <summary>Array must be SORTED. Look at the middle and throw away the half that can't contain the target. O(log n).</summary>
    public static int BinarySearch(int[] sorted, int target)
    {
        int left = 0, right = sorted.Length - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;

            if (sorted[mid] == target) return mid;
            if (sorted[mid] < target) left = mid + 1;
            else right = mid - 1;
        }

        return -1;
    }

    public static void Demo()
    {
        int[] numbers = [4, 8, 15, 16, 23, 42];
        Console.WriteLine($"Array: [{string.Join(", ", numbers)}]");
        Console.WriteLine($"Linear search 23 -> index {LinearSearch(numbers, 23)}");
        Console.WriteLine($"Binary search 23 -> index {BinarySearch(numbers, 23)}");
        Console.WriteLine($"Binary search 5  -> index {BinarySearch(numbers, 5)} (not found)");
        Console.WriteLine("For 1,000,000 items: linear needs up to 1,000,000 checks, binary about 20.");
    }
}
