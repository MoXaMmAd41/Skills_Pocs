using DataStructuresAlgorithms.Topics;

Console.OutputEncoding = System.Text.Encoding.UTF8;

(string Title, Action Demo)[] topics =
[
    ("Arrays", Arrays.Demo),
    ("Strings", Strings.Demo),
    ("Linked Lists", LinkedLists.Demo),
    ("Stack", StackTopic.Demo),
    ("Queue", QueueTopic.Demo),
    ("Hash Table", HashTable.Demo),
    ("Big O Notation", BigO.Demo),
    ("Searching", Searching.Demo),
    ("Sorting", Sorting.Demo),
    ("Recursion", Recursion.Demo),
    ("Binary Search Patterns", BinarySearchPatterns.Demo),
    ("BFS & DFS", BfsDfs.Demo)
];

while (true)
{
    Console.WriteLine("\n=== DATA STRUCTURES & ALGORITHMS ===");

    for (int i = 0; i < topics.Length; i++)
    {
        Console.WriteLine($"  {i + 1,2}. {topics[i].Title}");
    }

    Console.WriteLine("   0. Exit");
    Console.Write("\nChoose: ");

    string? choice = Console.ReadLine();

    if (choice is null || choice.Trim() == "0")
    {
        return;
    }

    if (int.TryParse(choice, out int n) && n >= 1 && n <= topics.Length)
    {
        Console.WriteLine($"\n--- {topics[n - 1].Title} ---");
        topics[n - 1].Demo();
    }
    else
    {
        Console.WriteLine("Invalid choice.");
    }
}
