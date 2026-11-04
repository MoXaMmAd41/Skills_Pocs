namespace DataStructuresAlgorithms.Topics;

/// <summary>
/// Binary search is not only "find x". Whenever the answer space is sorted / monotonic
/// (false false false TRUE TRUE), you can binary search for the BOUNDARY in O(log n).
/// </summary>
public static class BinarySearchPatterns
{
    /// <summary>Lower bound: first index where a[i] >= target (also the insert position).</summary>
    public static int LowerBound(int[] sorted, int target)
    {
        int left = 0, right = sorted.Length; // search space is [left, right)

        while (left < right)
        {
            int mid = left + (right - left) / 2;
            if (sorted[mid] < target) left = mid + 1;
            else right = mid;
        }

        return left;
    }

    public static int FirstOccurrence(int[] sorted, int target)
    {
        int i = LowerBound(sorted, target);
        return i < sorted.Length && sorted[i] == target ? i : -1;
    }

    public static int LastOccurrence(int[] sorted, int target)
    {
        int i = LowerBound(sorted, target + 1) - 1; // just before the first bigger item
        return i >= 0 && sorted[i] == target ? i : -1;
    }

    /// <summary>Search on the answer: biggest x with x*x &lt;= n.</summary>
    public static int IntegerSqrt(int n)
    {
        long left = 0, right = n;

        while (left < right)
        {
            long mid = (left + right + 1) / 2;
            if (mid * mid <= n) left = mid;
            else right = mid - 1;
        }

        return (int)left;
    }

    /// <summary>Sorted array rotated at some pivot, e.g. [4, 5, 6, 7, 0, 1, 2]: find the minimum.</summary>
    public static int MinInRotated(int[] a)
    {
        int left = 0, right = a.Length - 1;

        while (left < right)
        {
            int mid = (left + right) / 2;
            if (a[mid] > a[right]) left = mid + 1; // the drop is to the right of mid
            else right = mid;
        }

        return a[left];
    }

    public static void Demo()
    {
        int[] a = [1, 2, 2, 2, 3, 5, 8];
        Console.WriteLine($"Array: [{string.Join(", ", a)}]");
        Console.WriteLine($"First occurrence of 2     -> {FirstOccurrence(a, 2)}");
        Console.WriteLine($"Last occurrence of 2      -> {LastOccurrence(a, 2)}");
        Console.WriteLine($"Insert position for 4     -> {LowerBound(a, 4)}");
        Console.WriteLine($"Integer sqrt(50)          -> {IntegerSqrt(50)}");
        Console.WriteLine($"Min in [4, 5, 6, 7, 0, 1, 2] -> {MinInRotated([4, 5, 6, 7, 0, 1, 2])}");
    }
}
