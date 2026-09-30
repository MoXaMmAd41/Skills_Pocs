namespace DataStructuresAlgorithms.Algorithms;

/// <summary>Searches over sorted data. Each halves the search range per step: O(log n).</summary>
public static class Searching
{
    /// <returns>The index of <paramref name="value"/>, or -1 if absent.</returns>
    public static int BinarySearch<T>(ReadOnlySpan<T> sorted, T value, IComparer<T>? comparer = null)
    {
        comparer ??= Comparer<T>.Default;
        int index = LowerBound(sorted, value, comparer);

        return index < sorted.Length && comparer.Compare(sorted[index], value) == 0 ? index : -1;
    }

    /// <summary>
    /// First index whose element is ≥ <paramref name="value"/> (or <c>Length</c> if none) — the
    /// insertion point that keeps the span sorted. Also answers "how many elements are &lt; value".
    /// </summary>
    public static int LowerBound<T>(ReadOnlySpan<T> sorted, T value, IComparer<T>? comparer = null)
    {
        comparer ??= Comparer<T>.Default;
        int low = 0, high = sorted.Length;

        while (low < high)
        {
            // low + (high - low) / 2 instead of (low + high) / 2 avoids int overflow on huge spans.
            int middle = low + (high - low) / 2;

            if (comparer.Compare(sorted[middle], value) < 0)
            {
                low = middle + 1;
            }
            else
            {
                high = middle;
            }
        }

        return low;
    }
}
