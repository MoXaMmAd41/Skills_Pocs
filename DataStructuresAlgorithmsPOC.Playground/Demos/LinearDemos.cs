using DataStructuresAlgorithms.DataStructures;

namespace DataStructuresAlgorithms.Playground.Demos;

internal sealed class StackDemo : CommandDemo
{
    private readonly ArrayStack<int> _stack = new();

    public override string Title => "Stack (LIFO) — last in, first out";

    protected override IReadOnlyList<string> Commands =>
    [
        "push <n> [n...]       put numbers on top",
        "pop                   remove the top number",
        "peek                  look at the top without removing it"
    ];

    protected override void Execute(string command, string[] args)
    {
        switch (command)
        {
            case "push":
                foreach (int n in Numbers(args)) _stack.Push(n);
                break;
            case "pop":
                Ui.Result($"Popped {_stack.Pop()}");
                break;
            case "peek":
                Ui.Result($"Top is {_stack.Peek()}");
                break;
            default:
                UnknownCommand(command);
                break;
        }
    }

    protected override void Render() =>
        Ui.State($"top → {Ui.List(_stack)} ← bottom    (count {_stack.Count}, array capacity {_stack.Capacity})");
}

internal sealed class QueueDemo : CommandDemo
{
    private readonly CircularQueue<int> _queue = new();

    public override string Title => "Queue (FIFO) on a ring buffer — first in, first out";

    protected override IReadOnlyList<string> Commands =>
    [
        "enqueue <n> [n...]    add numbers at the back",
        "dequeue               remove the front number",
        "peek                  look at the front"
    ];

    protected override void Execute(string command, string[] args)
    {
        switch (command)
        {
            case "enqueue" or "add":
                foreach (int n in Numbers(args)) _queue.Enqueue(n);
                break;
            case "dequeue" or "remove":
                Ui.Result($"Dequeued {_queue.Dequeue()}");
                break;
            case "peek":
                Ui.Result($"Front is {_queue.Peek()}");
                break;
            default:
                UnknownCommand(command);
                break;
        }
    }

    protected override void Render()
    {
        Ui.State($"front → {Ui.List(_queue)} ← back    (count {_queue.Count})");

        // Show the physical slots: watch the front wrap around instead of shifting.
        string?[] slots = new string?[_queue.Capacity];
        int i = 0;

        foreach (int item in _queue)
        {
            slots[(_queue.HeadIndex + i++) % _queue.Capacity] = item.ToString();
        }

        string ring = string.Join(" | ", slots.Select((s, index) => (index == _queue.HeadIndex && s is not null ? "*" : "") + (s ?? "_")));
        Ui.Muted($"ring buffer: [ {ring} ]   (* = front slot {_queue.HeadIndex})");
    }
}

internal sealed class LinkedListDemo : CommandDemo
{
    private readonly DoublyLinkedList<string> _list = new();

    public override string Title => "Doubly linked list";

    protected override IReadOnlyList<string> Commands =>
    [
        "first <word>          add at the front",
        "last <word>           add at the back",
        "remove <word>         unlink the first node with that value",
        "front <word>          move that node to the front (what the LRU cache does)"
    ];

    protected override void Execute(string command, string[] args)
    {
        switch (command)
        {
            case "first":
                _list.AddFirst(Word(args));
                break;
            case "last":
                _list.AddLast(Word(args));
                break;
            case "remove":
                _list.Remove(Find(Word(args)));
                break;
            case "front":
                _list.MoveToFront(Find(Word(args)));
                break;
            default:
                UnknownCommand(command);
                break;
        }
    }

    protected override void Render() =>
        Ui.State(_list.Count == 0 ? "(empty)" : $"null ← {string.Join(" ⇄ ", _list)} → null    (count {_list.Count})");

    private DoublyLinkedListNode<string> Find(string value)
    {
        for (DoublyLinkedListNode<string>? node = _list.First; node is not null; node = node.Next)
        {
            if (node.Value == value)
            {
                return node;
            }
        }

        throw new KeyNotFoundException($"'{value}' is not in the list.");
    }
}
