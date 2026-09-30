namespace DataStructuresAlgorithms.DataStructures;

public enum TreeSide
{
    Root,
    Left,
    Right
}

/// <summary>
/// Unbalanced binary search tree: every left descendant is smaller than its node, every right one larger.
/// Duplicates are ignored.
/// </summary>
/// <remarks>
/// Add/Contains/Remove: O(h), where h is the height — O(log n) for random input, but O(n) for sorted
/// input, where the tree degenerates into a list. Balanced trees (AVL, red-black, e.g. SortedSet) fix that.
/// </remarks>
public sealed class BinarySearchTree<T>(IComparer<T>? comparer = null)
{
    private readonly IComparer<T> _comparer = comparer ?? Comparer<T>.Default;
    private Node? _root;

    public int Count { get; private set; }

    public int Height => HeightOf(_root);

    public bool Add(T value)
    {
        if (_root is null)
        {
            _root = new Node(value);
            Count++;
            return true;
        }

        Node current = _root;

        while (true)
        {
            int comparison = _comparer.Compare(value, current.Value);

            if (comparison == 0)
            {
                return false;
            }

            ref Node? child = ref comparison < 0 ? ref current.Left : ref current.Right;

            if (child is null)
            {
                child = new Node(value);
                Count++;
                return true;
            }

            current = child;
        }
    }

    public bool Contains(T value)
    {
        Node? current = _root;

        while (current is not null)
        {
            int comparison = _comparer.Compare(value, current.Value);

            if (comparison == 0)
            {
                return true;
            }

            current = comparison < 0 ? current.Left : current.Right;
        }

        return false;
    }

    public bool Remove(T value)
    {
        int before = Count;
        _root = Remove(_root, value);
        return Count < before;
    }

    public T Min() => Leftmost(_root ?? throw new InvalidOperationException("The tree is empty.")).Value;

    public T Max()
    {
        Node node = _root ?? throw new InvalidOperationException("The tree is empty.");

        while (node.Right is not null)
        {
            node = node.Right;
        }

        return node.Value;
    }

    /// <summary>Yields values in ascending order (iterative, so deep trees can't overflow the stack).</summary>
    public IEnumerable<T> InOrder()
    {
        ArrayStack<Node> stack = new();
        Node? current = _root;

        while (current is not null || !stack.IsEmpty)
        {
            while (current is not null)
            {
                stack.Push(current);
                current = current.Left;
            }

            Node node = stack.Pop();
            yield return node.Value;
            current = node.Right;
        }
    }

    /// <summary>
    /// Root, then left subtree, then right subtree, with each node's depth and side — enough to draw the tree.
    /// </summary>
    public IEnumerable<(T Value, int Depth, TreeSide Side)> PreOrder()
    {
        if (_root is null)
        {
            yield break;
        }

        ArrayStack<(Node Node, int Depth, TreeSide Side)> stack = new();
        stack.Push((_root, 0, TreeSide.Root));

        while (stack.TryPop(out (Node Node, int Depth, TreeSide Side) current))
        {
            yield return (current.Node.Value, current.Depth, current.Side);

            // Push right first so the left subtree is visited first.
            if (current.Node.Right is not null) stack.Push((current.Node.Right, current.Depth + 1, TreeSide.Right));
            if (current.Node.Left is not null) stack.Push((current.Node.Left, current.Depth + 1, TreeSide.Left));
        }
    }

    private Node? Remove(Node? node, T value)
    {
        if (node is null)
        {
            return null;
        }

        int comparison = _comparer.Compare(value, node.Value);

        if (comparison < 0)
        {
            node.Left = Remove(node.Left, value);
            return node;
        }

        if (comparison > 0)
        {
            node.Right = Remove(node.Right, value);
            return node;
        }

        // Zero or one child: splice the node out.
        if (node.Left is null || node.Right is null)
        {
            Count--;
            return node.Left ?? node.Right;
        }

        // Two children: replace with the in-order successor, then delete the successor from the right subtree.
        T successor = Leftmost(node.Right).Value;
        node.Value = successor;
        node.Right = Remove(node.Right, successor);
        return node;
    }

    private static Node Leftmost(Node node)
    {
        while (node.Left is not null)
        {
            node = node.Left;
        }

        return node;
    }

    private static int HeightOf(Node? node) =>
        node is null ? 0 : 1 + Math.Max(HeightOf(node.Left), HeightOf(node.Right));

    private sealed class Node(T value)
    {
        public T Value = value;
        public Node? Left;
        public Node? Right;
    }
}
