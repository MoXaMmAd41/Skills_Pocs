namespace DataStructuresAlgorithms.Topics;

/// <summary>Every method sorts the array in place, ascending.</summary>
public static class Sorting
{
    /// <summary>Swap neighbours until nothing moves; the biggest "bubbles" to the end each pass. O(n²).</summary>
    public static void BubbleSort(int[] a)
    {
        for (int end = a.Length - 1; end > 0; end--)
        {
            bool swapped = false;

            for (int i = 0; i < end; i++)
            {
                if (a[i] > a[i + 1])
                {
                    (a[i], a[i + 1]) = (a[i + 1], a[i]);
                    swapped = true;
                }
            }

            if (!swapped) return; // already sorted
        }
    }

    /// <summary>Split in half, sort each half, merge the two sorted halves. O(n log n) always, O(n) extra memory.</summary>
    public static void MergeSort(int[] a)
    {
        if (a.Length < 2) return;

        int[] left = a[..(a.Length / 2)];
        int[] right = a[(a.Length / 2)..];
        MergeSort(left);
        MergeSort(right);

        int i = 0, j = 0, k = 0;
        while (i < left.Length && j < right.Length) a[k++] = left[i] <= right[j] ? left[i++] : right[j++];
        while (i < left.Length) a[k++] = left[i++];
        while (j < right.Length) a[k++] = right[j++];
    }

    /// <summary>Pick a pivot, put smaller items left and bigger right, then sort both sides. O(n log n) average, O(n²) worst.</summary>
    public static void QuickSort(int[] a) => QuickSort(a, 0, a.Length - 1);

    private static void QuickSort(int[] a, int low, int high)
    {
        if (low >= high) return;

        int pivot = a[high];
        int border = low; // everything before border is < pivot

        for (int i = low; i < high; i++)
        {
            if (a[i] < pivot)
            {
                (a[i], a[border]) = (a[border], a[i]);
                border++;
            }
        }

        (a[border], a[high]) = (a[high], a[border]); // pivot goes to its final place
        QuickSort(a, low, border - 1);
        QuickSort(a, border + 1, high);
    }

    /// <summary>Build a max-heap, then repeatedly move the max to the end. O(n log n), O(1) extra memory.</summary>
    public static void HeapSort(int[] a)
    {
        for (int i = a.Length / 2 - 1; i >= 0; i--) SiftDown(a, i, a.Length);

        for (int end = a.Length - 1; end > 0; end--)
        {
            (a[0], a[end]) = (a[end], a[0]);
            SiftDown(a, 0, end);
        }
    }

    // Push a[i] down until both children are smaller. Children of i are 2i+1 and 2i+2.
    private static void SiftDown(int[] a, int i, int size)
    {
        while (true)
        {
            int largest = i, left = 2 * i + 1, right = 2 * i + 2;
            if (left < size && a[left] > a[largest]) largest = left;
            if (right < size && a[right] > a[largest]) largest = right;
            if (largest == i) return;

            (a[i], a[largest]) = (a[largest], a[i]);
            i = largest;
        }
    }

    public static void Demo()
    {
        int[] input = [5, 2, 9, 1, 5, 6, 3];
        Console.WriteLine($"Input: [{string.Join(", ", input)}]");

        foreach ((string name, Action<int[]> sort) in new (string, Action<int[]>)[]
                 {
                     ("Bubble sort  O(n²)      ", BubbleSort),
                     ("Merge sort   O(n log n) ", MergeSort),
                     ("Quick sort   O(n log n) ", QuickSort),
                     ("Heap sort    O(n log n) ", HeapSort)
                 })
        {
            int[] copy = (int[])input.Clone();
            sort(copy);
            Console.WriteLine($"{name} -> [{string.Join(", ", copy)}]");
        }
    }
}
