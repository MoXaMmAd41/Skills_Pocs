# Data Structures & Algorithms

The core DSA topics implemented from scratch in plain C#: one file per topic, each with a `Demo()` you can run from a menu.

```powershell
dotnet run --project DataStructuresAlgorithmsPOC     # pick a topic from the menu
```

| # | Topic | File | What's inside |
|---|---|---|---|
| 1 | Arrays | `Topics/01_Arrays.cs` | `DynamicArray`: traverse, insert, delete, search |
| 2 | Strings | `Topics/02_Strings.cs` | Reverse, reverse words, substrings, palindrome, char count |
| 3 | Linked Lists | `Topics/03_LinkedLists.cs` | `SinglyLinkedList` (with reverse), `DoublyLinkedList` |
| 4 | Stack | `Topics/04_Stack.cs` | `MyStack` (push, pop, peek), balanced brackets |
| 5 | Queue | `Topics/05_Queue.cs` | `Queue<T>`, `CircularQueue`, `Deque` |
| 6 | Hash Table | `Topics/06_HashTable.cs` | `MyHashMap` (chaining), `MyHashSet`, `Dictionary` word count, Two Sum |
| 7 | Big O | `Topics/07_BigO.cs` | O(1), O(log n), O(n), O(n log n), O(n²) with step counts; space |
| 8 | Searching | `Topics/08_Searching.cs` | Linear search, binary search |
| 9 | Sorting | `Topics/09_Sorting.cs` | Bubble, merge, quick, heap sort |
| 10 | Recursion | `Topics/10_Recursion.cs` | Factorial, Fibonacci, sum of digits, reverse, fast power |
| 11 | Binary Search Patterns | `Topics/11_BinarySearchPatterns.cs` | Lower bound, first / last occurrence, integer sqrt, min in rotated array |
| 12 | BFS & DFS | `Topics/12_BfsDfs.cs` | Tree level-order and pre-order, graph BFS / DFS, shortest path |

## Cheat sheet

| Structure | Access | Search | Insert | Delete |
|---|---|---|---|---|
| Array | O(1) | O(n) | O(n) | O(n) |
| Linked list | O(n) | O(n) | O(1) at head | O(1) at head |
| Stack / Queue | top/front O(1) | O(n) | O(1) | O(1) |
| Hash table | – | O(1) avg | O(1) avg | O(1) avg |

| Sort | Time | Extra space |
|---|---|---|
| Bubble | O(n²) | O(1) |
| Merge | O(n log n) | O(n) |
| Quick | O(n log n) avg, O(n²) worst | O(log n) |
| Heap | O(n log n) | O(1) |
