namespace DataStructuresAlgorithms.Playground;

internal static class Ui
{
    public static void Heading(string text)
    {
        Console.WriteLine();
        Write($"=== {text} ===", ConsoleColor.Cyan);
    }

    public static void State(string text) => Write(text, ConsoleColor.White);

    public static void Result(string text) => Write(text, ConsoleColor.Green);

    public static void Error(string text) => Write($"! {text}", ConsoleColor.Red);

    public static void Muted(string text) => Write(text, ConsoleColor.DarkGray);

    public static string List<T>(IEnumerable<T> items) => $"[{string.Join(", ", items)}]";

    /// <summary>Writes one line made of differently coloured pieces.</summary>
    public static void Line(IEnumerable<(string Text, ConsoleColor Color)> segments)
    {
        foreach ((string text, ConsoleColor color) in segments)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
        }

        Console.ResetColor();
        Console.WriteLine();
    }

    private static void Write(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}

/// <summary>Wraps a comparer and counts calls, to make an algorithm's work visible.</summary>
internal sealed class CountingComparer<T>(IComparer<T>? inner = null) : IComparer<T>
{
    private readonly IComparer<T> _inner = inner ?? Comparer<T>.Default;

    public int Comparisons { get; private set; }

    public int Compare(T? x, T? y)
    {
        Comparisons++;
        return _inner.Compare(x!, y!);
    }
}
