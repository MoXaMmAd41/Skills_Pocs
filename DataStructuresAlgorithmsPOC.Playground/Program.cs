using DataStructuresAlgorithms.Playground;
using DataStructuresAlgorithms.Playground.Demos;

Console.OutputEncoding = System.Text.Encoding.UTF8;

CommandDemo[] demos =
[
    new StackDemo(),
    new QueueDemo(),
    new LinkedListDemo(),
    new HashMapDemo(),
    new BinarySearchTreeDemo(),
    new HeapDemo(),
    new TrieDemo(),
    new LruCacheDemo(),
    new SortingDemo(),
    new BinarySearchDemo(),
    new GraphDemo()
];

while (true)
{
    Ui.Heading("DATA STRUCTURES & ALGORITHMS PLAYGROUND");

    for (int i = 0; i < demos.Length; i++)
    {
        Console.WriteLine($"  {i + 1,2}. {demos[i].Title}");
    }

    Console.WriteLine("   0. Exit");
    Console.Write("\nChoose: ");

    string? choice = Console.ReadLine()?.Trim();

    if (choice is null or "0")
    {
        return;
    }

    if (int.TryParse(choice, out int index) && index >= 1 && index <= demos.Length)
    {
        demos[index - 1].Run();
    }
    else
    {
        Ui.Error($"'{choice}' is not a valid option.");
    }
}
