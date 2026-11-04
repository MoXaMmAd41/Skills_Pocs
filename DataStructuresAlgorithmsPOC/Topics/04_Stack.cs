namespace DataStructuresAlgorithms.Topics;

/// <summary>
/// Stack = LIFO (Last In, First Out), like a pile of plates.
/// Push / Pop / Peek are all O(1).
/// </summary>
public class MyStack
{
    private readonly List<int> _items = [];

    public int Count => _items.Count;

    public void Push(int value) => _items.Add(value);

    public int Pop()
    {
        int top = Peek();
        _items.RemoveAt(_items.Count - 1);
        return top;
    }

    public int Peek() => _items.Count > 0 ? _items[^1] : throw new InvalidOperationException("Stack is empty.");

    public override string ToString() => "top -> [" + string.Join(", ", Enumerable.Reverse(_items)) + "]";
}

public static class StackTopic
{
    /// <summary>Classic stack problem: every closing bracket must match the most recent opening one.</summary>
    public static bool IsBalanced(string text)
    {
        Stack<char> open = new();

        foreach (char c in text)
        {
            if (c is '(' or '[' or '{')
            {
                open.Push(c);
            }
            else if (c is ')' or ']' or '}')
            {
                if (open.Count == 0) return false;

                char expected = c switch { ')' => '(', ']' => '[', _ => '{' };
                if (open.Pop() != expected) return false;
            }
        }

        return open.Count == 0;
    }

    public static void Demo()
    {
        MyStack stack = new();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        Console.WriteLine($"Push 1, 2, 3  -> {stack}");
        Console.WriteLine($"Peek          -> {stack.Peek()}");
        Console.WriteLine($"Pop           -> {stack.Pop()}   now {stack}");
        Console.WriteLine($"Pop           -> {stack.Pop()}   now {stack}");

        Console.WriteLine($"Balanced \"{{[()]}}\" -> {IsBalanced("{[()]}")}");
        Console.WriteLine($"Balanced \"([)]\"   -> {IsBalanced("([)]")}");
        Console.WriteLine("Built-in: Stack<T> (Push, Pop, Peek)");
    }
}
