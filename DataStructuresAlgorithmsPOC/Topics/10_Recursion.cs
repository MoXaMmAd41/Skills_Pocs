namespace DataStructuresAlgorithms.Topics;

/// <summary>
/// A recursive function calls itself on a smaller problem.
/// It always needs a BASE CASE that stops the calls, otherwise you get a StackOverflowException.
/// </summary>
public static class Recursion
{
    public static long Factorial(int n)
    {
        if (n <= 1) return 1;          // base case
        return n * Factorial(n - 1);   // recursive call
    }

    public static long Fibonacci(int n)
    {
        if (n < 2) return n;
        return Fibonacci(n - 1) + Fibonacci(n - 2);
    }

    public static int SumDigits(int n)
    {
        if (n < 10) return n;
        return n % 10 + SumDigits(n / 10);
    }

    public static string Reverse(string text)
    {
        if (text.Length <= 1) return text;
        return Reverse(text[1..]) + text[0];
    }

    /// <summary>x^n in O(log n): x^10 = (x^5)², x^5 = x · (x^2)².</summary>
    public static long Power(long x, int n)
    {
        if (n == 0) return 1;
        long half = Power(x, n / 2);
        return n % 2 == 0 ? half * half : half * half * x;
    }

    public static void Demo()
    {
        Console.WriteLine($"Factorial(5)     -> {Factorial(5)}   (5 * 4 * 3 * 2 * 1)");
        Console.WriteLine($"Fibonacci(10)    -> {Fibonacci(10)}");
        Console.WriteLine($"SumDigits(1234)  -> {SumDigits(1234)}");
        Console.WriteLine($"Reverse(\"hello\") -> {Reverse("hello")}");
        Console.WriteLine($"Power(2, 10)     -> {Power(2, 10)}");

        Console.WriteLine("\nHow Factorial(3) runs:");
        Console.WriteLine("  Factorial(3) = 3 * Factorial(2)");
        Console.WriteLine("                     Factorial(2) = 2 * Factorial(1)");
        Console.WriteLine("                                         Factorial(1) = 1   <- base case");
        Console.WriteLine("  then it unwinds: 2 * 1 = 2, 3 * 2 = 6");
    }
}
