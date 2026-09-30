using System.Collections;

namespace DataStructuresAlgorithms.DataStructures;

public sealed class DoublyLinkedListNode<T>
{
    internal DoublyLinkedListNode(T value, DoublyLinkedList<T> list)
    {
        Value = value;
        List = list;
    }

    public T Value { get; set; }

    public DoublyLinkedListNode<T>? Next { get; internal set; }

    public DoublyLinkedListNode<T>? Previous { get; internal set; }

    internal DoublyLinkedList<T>? List { get; set; }
}

/// <summary>
/// Doubly linked list. Handing out nodes lets callers remove or move an item in O(1),
/// which is what makes <see cref="LruCache{TKey,TValue}"/> possible.
/// </summary>
/// <remarks>
/// Add/remove at either end: O(1). Remove or move a known node: O(1). Search by value: O(n).
/// </remarks>
public sealed class DoublyLinkedList<T> : IReadOnlyCollection<T>
{
    public DoublyLinkedListNode<T>? First { get; private set; }

    public DoublyLinkedListNode<T>? Last { get; private set; }

    public int Count { get; private set; }

    public DoublyLinkedListNode<T> AddFirst(T value)
    {
        DoublyLinkedListNode<T> node = new(value, this);
        LinkFirst(node);
        return node;
    }

    public DoublyLinkedListNode<T> AddLast(T value)
    {
        DoublyLinkedListNode<T> node = new(value, this) { Previous = Last };

        if (Last is null)
        {
            First = node;
        }
        else
        {
            Last.Next = node;
        }

        Last = node;
        Count++;
        return node;
    }

    public void Remove(DoublyLinkedListNode<T> node)
    {
        EnsureOwned(node);
        Unlink(node);
        node.List = null;
    }

    public T RemoveFirst()
    {
        DoublyLinkedListNode<T> first = First ?? throw new InvalidOperationException("The list is empty.");
        Remove(first);
        return first.Value;
    }

    public T RemoveLast()
    {
        DoublyLinkedListNode<T> last = Last ?? throw new InvalidOperationException("The list is empty.");
        Remove(last);
        return last.Value;
    }

    public void MoveToFront(DoublyLinkedListNode<T> node)
    {
        EnsureOwned(node);

        if (node == First)
        {
            return;
        }

        Unlink(node);
        LinkFirst(node);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (DoublyLinkedListNode<T>? node = First; node is not null; node = node.Next)
        {
            yield return node.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void LinkFirst(DoublyLinkedListNode<T> node)
    {
        node.Previous = null;
        node.Next = First;

        if (First is null)
        {
            Last = node;
        }
        else
        {
            First.Previous = node;
        }

        First = node;
        Count++;
    }

    private void Unlink(DoublyLinkedListNode<T> node)
    {
        if (node.Previous is null)
        {
            First = node.Next;
        }
        else
        {
            node.Previous.Next = node.Next;
        }

        if (node.Next is null)
        {
            Last = node.Previous;
        }
        else
        {
            node.Next.Previous = node.Previous;
        }

        node.Next = null;
        node.Previous = null;
        Count--;
    }

    private void EnsureOwned(DoublyLinkedListNode<T> node)
    {
        if (node.List != this)
        {
            throw new InvalidOperationException("The node does not belong to this list.");
        }
    }
}
