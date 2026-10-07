namespace DataStructuresAlgorithms.Algorithms;

/// <summary>Classic comparison sorts, all operating in place on a span.</summary>
public static class Sorting
{
    // Below this size insertion sort beats the recursive sorts (fewer comparisons of overhead).
    private const int InsertionSortThreshold = 16;

    /// <summary>O(n²) time, O(1) space, stable. Fast for tiny or nearly-sorted input.</summary>
    public static void InsertionSort<T>(Span<T> items, IComparer<T>? comparer = null)
    {
        comparer ??= Comparer<T>.Default;

        for (int i = 1; i < items.Length; i++)
        {
            T current = items[i];
            int j = i - 1;

            while (j >= 0 && comparer.Compare(items[j], current) > 0)
            {
                items[j + 1] = items[j];
                j--;
            }

            items[j + 1] = current;
        }
    }

    /// <summary>
    /// O(n log n) average, O(n²) worst case, O(log n) stack, not stable.
    /// Median-of-three pivot choice avoids the worst case on already-sorted input.
    /// </summary>
    public static void QuickSort<T>(Span<T> items, IComparer<T>? comparer = null) =>
        QuickSortCore(items, comparer ?? Comparer<T>.Default);

    /// <summary>O(n log n) always, O(n) extra space, stable.</summary>
    public static void MergeSort<T>(Span<T> items, IComparer<T>? comparer = null)
    {
        if (items.Length < 2)
        {
            return;
        }

        T[] buffer = new T[items.Length];
        MergeSortCore(items, buffer, comparer ?? Comparer<T>.Default);
    }

    /// <summary>O(n log n) always, O(1) extra space, not stable.</summary>
    public static void HeapSort<T>(Span<T> items, IComparer<T>? comparer = null)
    {
        comparer ??= Comparer<T>.Default;

        // Build a max-heap, then repeatedly move the max to the end and shrink the heap.
        for (int i = items.Length / 2 - 1; i >= 0; i--)
        {
            SiftDown(items, i, items.Length, comparer);
        }

        for (int end = items.Length - 1; end > 0; end--)
        {
            (items[0], items[end]) = (items[end], items[0]);
            SiftDown(items, 0, end, comparer);
        }
    }

    private static void QuickSortCore<T>(Span<T> items, IComparer<T> comparer)
    {
        // Recurse into the smaller side and loop on the larger, capping stack depth at O(log n).
        while (items.Length > InsertionSortThreshold)
        {
            int pivotIndex = Partition(items, comparer);
            Span<T> left = items[..pivotIndex];
            Span<T> right = items[(pivotIndex + 1)..];

            if (left.Length < right.Length)
            {
                QuickSortCore(left, comparer);
                items = right;
            }
            else
            {
                QuickSortCore(right, comparer);
                items = left;
            }
        }

        InsertionSort(items, comparer);
    }

    // Lomuto partition around the median of first/middle/last, parked at the end.
    private static int Partition<T>(Span<T> items, IComparer<T> comparer)
    {
        int last = items.Length - 1;
        int middle = last / 2;

        if (comparer.Compare(items[middle], items[0]) < 0) (items[middle], items[0]) = (items[0], items[middle]);
        if (comparer.Compare(items[last], items[0]) < 0) (items[last], items[0]) = (items[0], items[last]);
        if (comparer.Compare(items[middle], items[last]) < 0) (items[middle], items[last]) = (items[last], items[middle]);

        T pivot = items[last];
        int store = 0;

        for (int i = 0; i < last; i++)
        {
            if (comparer.Compare(items[i], pivot) < 0)
            {
                (items[i], items[store]) = (items[store], items[i]);
                store++;
            }
        }

        (items[store], items[last]) = (items[last], items[store]);
        return store;
    }

    private static void MergeSortCore<T>(Span<T> items, Span<T> buffer, IComparer<T> comparer)
    {
        if (items.Length <= InsertionSortThreshold)
        {
            InsertionSort(items, comparer);
            return;
        }

        int middle = items.Length / 2;
        MergeSortCore(items[..middle], buffer[..middle], comparer);
        MergeSortCore(items[middle..], buffer[middle..], comparer);

        // Already in order: skip the merge (makes sorted input O(n)).
        if (comparer.Compare(items[middle - 1], items[middle]) <= 0)
        {
            return;
        }

        items.CopyTo(buffer);
        int left = 0, right = middle, target = 0;

        while (left < middle && right < items.Length)
        {
            // "<=" takes from the left run on ties, which is what keeps the sort stable.
            items[target++] = comparer.Compare(buffer[left], buffer[right]) <= 0 ? buffer[left++] : buffer[right++];
        }

        // Leftovers from the right run are already in their final place; only the left run needs copying.
        buffer[left..middle].CopyTo(items[target..]);
    }

    private static void SiftDown<T>(Span<T> items, int index, int length, IComparer<T> comparer)
    {
        while (true)
        {
            int left = 2 * index + 1;
            int right = left + 1;
            int largest = index;

            if (left < length && comparer.Compare(items[left], items[largest]) > 0) largest = left;
            if (right < length && comparer.Compare(items[right], items[largest]) > 0) largest = right;

            if (largest == index)
            {
                return;
            }

            (items[index], items[largest]) = (items[largest], items[index]);
            index = largest;
        }
    }
}
