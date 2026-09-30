# Data Structures & Algorithms

Classic data structures and algorithms implemented from scratch in C#, with unit tests, benchmarks
against the .NET built-ins, and a real use in the Employee Management app.

```
DataStructuresAlgorithmsPOC/             Library (namespace DataStructuresAlgorithms)
DataStructuresAlgorithmsPOC.Tests/       69 xUnit tests (incl. randomized comparisons with the BCL)
DataStructuresAlgorithmsPOC.Benchmarks/  BenchmarkDotNet: ours vs .NET
DataStructuresAlgorithmsPOC.Playground/  Interactive console: try every structure by hand
```

## Data structures

| Structure | Key operations | Complexity | Notes |
|---|---|---|---|
| `ArrayStack<T>` | Push / Pop / Peek | O(1) amortised | Array doubles on growth |
| `CircularQueue<T>` | Enqueue / Dequeue | O(1) amortised | Ring buffer: dequeue never shifts elements |
| `DoublyLinkedList<T>` | Add/remove at ends, remove/move a node | O(1) | Exposes nodes so callers can unlink in O(1) |
| `HashMap<TKey,TValue>` | Get / Add / Remove | O(1) avg, O(n) worst | Separate chaining, resizes at load factor 0.75 |
| `BinarySearchTree<T>` | Add / Contains / Remove | O(h): log n random, **n sorted** | Unbalanced on purpose; a test documents the degeneration |
| `MinHeap<T>` | Push / Pop | O(log n); heapify O(n) | Reverse the comparer for a max-heap |
| `Trie<TValue>` | Insert / FindByPrefix | O(k) / O(p + matches) | Independent of total key count; alphabetical results |
| `Graph<T>` | AddEdge / Neighbors | O(1) | Weighted adjacency list, directed or undirected |
| `LruCache<TKey,TValue>` | Get / Set | O(1) | `HashMap` + `DoublyLinkedList`; evicts least recently used |

## Algorithms

| Algorithm | Time | Space | Stable |
|---|---|---|---|
| Insertion sort | O(n²) | O(1) | ✔ |
| Quick sort (median-of-3, insertion cut-off, tail-loop) | O(n log n) avg, O(n²) worst | O(log n) | ✘ |
| Merge sort (skips merge when runs are already ordered) | O(n log n) | O(n) | ✔ |
| Heap sort | O(n log n) | O(1) | ✘ |
| Binary search / lower bound | O(log n) | O(1) | – |
| BFS / DFS (iterative) | O(V + E) | O(V) | – |
| Shortest path by edges (BFS) | O(V + E) | O(V) | – |
| Dijkstra (binary heap, lazy deletion) | O((V + E) log V) | O(V) | – |
| Topological sort (Kahn, detects cycles) | O(V + E) | O(V) | – |

## Benchmarks

`--job short` on the dev machine (Intel, .NET 10). Error bars are wide on the short job, so read these as rough ratios.

| Benchmark | Result |
|---|---|
| **Prefix search, 10k names** | Trie **3.7 µs** vs linear scan 192 µs → **~50× faster** |
| **Prefix search, 200k names** | Trie **4.8 µs** vs linear scan 5,620 µs → **~1,170× faster**; the trie barely grows |
| Sort 1k ints | QuickSort ≈ `Array.Sort`; MergeSort 1.35×; HeapSort 3.4× slower |
| Sort 100k ints | All within ~±25% of `Array.Sort` (noisy at this size) |
| HashMap, 100k insert + lookup | 1.75× slower than `Dictionary` |

The takeaway is a real one: **the runtime's collections are hard to beat** (they're struct-based and heavily tuned), so use them in production. A custom structure pays off when it changes the complexity class of the problem, as the Trie does for prefix search.

```powershell
dotnet test DataStructuresAlgorithmsPOC.Tests
dotnet run -c Release --project DataStructuresAlgorithmsPOC.Benchmarks -- --filter *          # everything
dotnet run -c Release --project DataStructuresAlgorithmsPOC.Benchmarks -- --filter *Prefix*   # one group
```

## Applied: employee autocomplete

The employee search box in the Angular app suggests names as you type, served by this library's `Trie`:

- `EmployeeManagement.Infrastructure/Search/TrieEmployeeSearchIndex.cs` indexes each employee under their
  first name, last name, full name and email, so "doe", "jane" and "jane.d" all find Jane Doe.
- It is built lazily from the database and kept in memory as an immutable snapshot, so reads need no locks.
- Employee writes invalidate it immediately. A 5-minute TTL bounds staleness from other instances when scaled out.
- Endpoint: `GET /api/v1/employees/suggestions?prefix=am&limit=8`.
- Application code only sees the `IEmployeeSearchIndex` port, so the Trie is an Infrastructure detail.

## Try it by hand: the Playground

```powershell
dotnet run --project DataStructuresAlgorithmsPOC.Playground
```

Pick a structure, type commands, and it redraws the structure after each one. Type `help` for commands and `back` for the menu.

| Screen | Try | What you'll see |
|---|---|---|
| Stack | `push 1 2 3 4 5`, `pop` | Top-to-bottom order; the array doubles from 4 to 8 |
| Queue | `enqueue 1 2 3`, `dequeue`, `dequeue`, `enqueue 4 5` | The ring buffer wraps: `[ 5 \| _ \| *3 \| 4 ]` |
| Linked list | `last a`, `last b`, `last c`, `front c` | O(1) relinking, the move the LRU cache relies on |
| Hash map | `fill 10` | Resizes 4 → 8 → 16 as the load factor passes 0.75 |
| BST | `add 50 30 70 20 40`, `remove 30`; then `clear`, `add 1 2 3 4 5` | The tree drawn; sorted input degenerates (height 5, not 3) |
| Heap | `push 7 3 9 1 5`, `pop` | Array layout level by level; the minimum is always on top |
| Trie | `seed`, `find app`, `find ban` | Prefix matches in alphabetical order |
| LRU cache | `set a 1`, `set b 2`, `set c 3`, `get a`, `set d 4` | `b` is evicted, not `a`, because `get a` refreshed it |
| Sorting | `random 10000`, then `sorted 10000` | Comparison counts: insertion sort does 25M on random input but only 9,999 on sorted |
| Binary search | `range 1000000`, `find 777777` | Found in about 20 comparisons instead of up to 1,000,000 |
| Graph | `path A E` vs `cheapest A E`, `new directed`, `edge ...`, `topo` | Fewest edges vs lowest cost; build order |
