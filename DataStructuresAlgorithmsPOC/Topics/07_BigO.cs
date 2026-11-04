namespace DataStructuresAlgorithms.Topics;

/// <summary>
/// Big O describes how work (time) or memory (space) grows as the input size n grows.
/// Each method below counts its steps so you can see the growth.
/// </summary>
public static class BigO
{
    // O(1): same work no matter how big n is.
    public static int Constant(int n) => 1;

    // O(log n): halve the problem each step (binary search).
    public static int Logarithmic(int n)
    {
        int steps = 0;
        for (int i = n; i > 1; i /= 2) steps++;
        return steps;
    }

    // O(n): one pass over the input.
    public static int Linear(int n)
    {
        int steps = 0;
        for (int i = 0; i < n; i++) steps++;
        return steps;
    }

    // O(n log n): good sorting algorithms (merge sort).
    public static int Linearithmic(int n) => n * Logarithmic(n);

    // O(n²): a loop inside a loop (bubble sort, comparing every pair).
    public static int Quadratic(int n)
    {
        int steps = 0;
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                steps++;
        return steps;
    }

    // Space O(1): only a few variables.
    public static int SumInPlace(int[] numbers)
    {
        int sum = 0;
        foreach (int x in numbers) sum += x;
        return sum;
    }

    // Space O(n): builds a new array as big as the input.
    public static int[] Doubled(int[] numbers) => numbers.Select(x => x * 2).ToArray();

    public static void Demo()
    {
        Console.WriteLine($"{"n",8} {"O(1)",6} {"O(log n)",9} {"O(n)",8} {"O(n log n)",11} {"O(n²)",12}");

        foreach (int n in new[] { 10, 100, 1_000, 10_000 })
        {
            Console.WriteLine($"{n,8} {Constant(n),6} {Logarithmic(n),9} {Linear(n),8} {Linearithmic(n),11} {Quadratic(n),12:N0}");
        }

        Console.WriteLine("\nRules of thumb:");
        Console.WriteLine("  - Drop constants: O(2n) is O(n).");
        Console.WriteLine("  - Keep the biggest term: O(n² + n) is O(n²).");
        Console.WriteLine("  - Loops one after another ADD, nested loops MULTIPLY.");
        Console.WriteLine("  - Space: SumInPlace is O(1) extra memory, Doubled is O(n).");
    }
}
