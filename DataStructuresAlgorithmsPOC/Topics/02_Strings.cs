using System.Text;

namespace DataStructuresAlgorithms.Topics;

/// <summary>Strings are immutable in C#: every "change" creates a new string, so build big ones with StringBuilder.</summary>
public static class Strings
{
    public static string Reverse(string text)
    {
        char[] chars = text.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    public static string ReverseWords(string sentence) =>
        string.Join(" ", sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries).Reverse());

    public static bool IsPalindrome(string text)
    {
        string clean = new(text.Where(char.IsLetterOrDigit).Select(char.ToLower).ToArray());
        return clean == Reverse(clean);
    }

    /// <summary>All substrings: n(n+1)/2 of them, so O(n²).</summary>
    public static List<string> Substrings(string text)
    {
        List<string> result = [];

        for (int start = 0; start < text.Length; start++)
        {
            for (int length = 1; start + length <= text.Length; length++)
            {
                result.Add(text.Substring(start, length));
            }
        }

        return result;
    }

    public static Dictionary<char, int> CountCharacters(string text)
    {
        Dictionary<char, int> counts = [];

        foreach (char c in text)
        {
            counts[c] = counts.GetValueOrDefault(c) + 1;
        }

        return counts;
    }

    public static void Demo()
    {
        Console.WriteLine($"Reverse \"hello\"            -> {Reverse("hello")}");
        Console.WriteLine($"Reverse words \"I love C#\"  -> {ReverseWords("I love C#")}");
        Console.WriteLine($"Upper / Replace            -> {"hello world".ToUpper()} / {"hello world".Replace("world", "C#")}");
        Console.WriteLine($"Substrings of \"abc\"        -> {string.Join(", ", Substrings("abc"))}");
        Console.WriteLine($"Contains \"ell\" in hello    -> {"hello".Contains("ell")}");
        Console.WriteLine($"Palindrome \"racecar\"       -> {IsPalindrome("racecar")}");
        Console.WriteLine($"Palindrome \"A man, a plan, a canal: Panama\" -> {IsPalindrome("A man, a plan, a canal: Panama")}");
        Console.WriteLine($"Palindrome \"hello\"         -> {IsPalindrome("hello")}");
        Console.WriteLine($"Count chars \"banana\"       -> {string.Join(", ", CountCharacters("banana").Select(p => $"{p.Key}:{p.Value}"))}");

        StringBuilder builder = new();
        for (int i = 1; i <= 5; i++) builder.Append(i).Append(' ');
        Console.WriteLine($"StringBuilder 1..5         -> {builder}");
    }
}
