namespace DataStructuresAlgorithms.Topics;

/// <summary>
/// Singly linked list: each node points to the next one.
/// Add at front O(1) | Add at end O(n) (walk to the tail) | Search O(n) | Reverse O(n)
/// </summary>
public class SinglyLinkedList
{
    public class Node(int value)
    {
        public int Value = value;
        public Node? Next;
    }

    public Node? Head { get; private set; }

    public void AddFirst(int value) => Head = new Node(value) { Next = Head };

    public void AddLast(int value)
    {
        Node node = new(value);

        if (Head is null)
        {
            Head = node;
            return;
        }

        Node current = Head;
        while (current.Next is not null) current = current.Next;
        current.Next = node;
    }

    public bool Remove(int value)
    {
        if (Head is null) return false;

        if (Head.Value == value)
        {
            Head = Head.Next;
            return true;
        }

        Node current = Head;
        while (current.Next is not null && current.Next.Value != value) current = current.Next;

        if (current.Next is null) return false;

        current.Next = current.Next.Next; // skip over the removed node
        return true;
    }

    /// <summary>Flip every arrow: prev &lt;- current, then move both one step forward.</summary>
    public void Reverse()
    {
        Node? previous = null;
        Node? current = Head;

        while (current is not null)
        {
            Node? next = current.Next;
            current.Next = previous;
            previous = current;
            current = next;
        }

        Head = previous;
    }

    public List<int> ToList()
    {
        List<int> values = [];
        for (Node? n = Head; n is not null; n = n.Next) values.Add(n.Value);
        return values;
    }

    public override string ToString() => string.Join(" -> ", ToList()) + " -> null";
}

/// <summary>
/// Doubly linked list: each node points to next AND previous, and we keep a Tail.
/// Add / remove at both ends O(1) | Can walk backwards.
/// </summary>
public class DoublyLinkedList
{
    public class Node(int value)
    {
        public int Value = value;
        public Node? Next;
        public Node? Previous;
    }

    public Node? Head { get; private set; }
    public Node? Tail { get; private set; }

    public void AddFirst(int value)
    {
        Node node = new(value) { Next = Head };
        if (Head is null) Tail = node;
        else Head.Previous = node;
        Head = node;
    }

    public void AddLast(int value)
    {
        Node node = new(value) { Previous = Tail };
        if (Tail is null) Head = node;
        else Tail.Next = node;
        Tail = node;
    }

    public int RemoveFirst()
    {
        Node node = Head ?? throw new InvalidOperationException("List is empty.");
        Head = node.Next;
        if (Head is null) Tail = null;
        else Head.Previous = null;
        return node.Value;
    }

    public int RemoveLast()
    {
        Node node = Tail ?? throw new InvalidOperationException("List is empty.");
        Tail = node.Previous;
        if (Tail is null) Head = null;
        else Tail.Next = null;
        return node.Value;
    }

    public List<int> ToList()
    {
        List<int> values = [];
        for (Node? n = Head; n is not null; n = n.Next) values.Add(n.Value);
        return values;
    }

    public List<int> ToListBackward()
    {
        List<int> values = [];
        for (Node? n = Tail; n is not null; n = n.Previous) values.Add(n.Value);
        return values;
    }

    public override string ToString() => "null <- " + string.Join(" <-> ", ToList()) + " -> null";
}

public static class LinkedLists
{
    public static void Demo()
    {
        Console.WriteLine("Singly:");
        SinglyLinkedList singly = new();
        singly.AddLast(1);
        singly.AddLast(2);
        singly.AddLast(3);
        singly.AddFirst(0);
        Console.WriteLine($"  AddLast 1,2,3 + AddFirst 0 -> {singly}");
        singly.Remove(2);
        Console.WriteLine($"  Remove 2                   -> {singly}");
        singly.Reverse();
        Console.WriteLine($"  Reverse                    -> {singly}");

        Console.WriteLine("Doubly:");
        DoublyLinkedList doubly = new();
        doubly.AddLast(1);
        doubly.AddLast(2);
        doubly.AddLast(3);
        doubly.AddFirst(0);
        Console.WriteLine($"  AddLast 1,2,3 + AddFirst 0 -> {doubly}");
        Console.WriteLine($"  Backward                   -> {string.Join(" -> ", doubly.ToListBackward())}");
        Console.WriteLine($"  RemoveFirst = {doubly.RemoveFirst()}, RemoveLast = {doubly.RemoveLast()} -> {doubly}");
    }
}
