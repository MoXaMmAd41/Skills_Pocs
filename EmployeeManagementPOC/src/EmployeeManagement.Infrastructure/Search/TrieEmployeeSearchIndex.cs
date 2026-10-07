using System.Data.Common;
using Dapper;
using DataStructuresAlgorithms.DataStructures;
using EmployeeManagement.Application.Features.Employees;
using EmployeeManagement.Infrastructure.Queries;

namespace EmployeeManagement.Infrastructure.Search;

/// <summary>
/// Autocomplete backed by a <see cref="Trie{TValue}"/>. Each employee is indexed under their first name,
/// last name, full name and email, so "doe", "jane" and "jane.d" all find Jane Doe.
/// </summary>
/// <remarks>
/// The trie is built lazily from the database, then served from memory: a lookup costs O(prefix + matches)
/// regardless of headcount, instead of a LIKE scan per keystroke.
/// Writes on this instance invalidate it immediately; the TTL bounds staleness from writes made by
/// other instances when the API is scaled out.
/// </remarks>
internal sealed class TrieEmployeeSearchIndex(SqlConnectionFactory connectionFactory, TimeProvider timeProvider)
    : IEmployeeSearchIndex, IDisposable
{
    private static readonly TimeSpan TimeToLive = TimeSpan.FromMinutes(5);

    private const string Sql = """
        SELECT
            e.Id,
            e.FirstName,
            e.LastName,
            e.Email,
            d.Name AS DepartmentName
        FROM Employees e
        INNER JOIN Departments d ON d.Id = e.DepartmentId;
        """;

    private readonly SemaphoreSlim _buildLock = new(1, 1);

    // Published snapshots are never mutated, so concurrent readers need no locking.
    private volatile Snapshot? _snapshot;
    private int _version;

    public async Task<IReadOnlyList<EmployeeSuggestion>> SuggestAsync(
        string prefix,
        int limit,
        CancellationToken cancellationToken = default)
    {
        Trie<EmployeeSuggestion> trie = await GetTrieAsync(cancellationToken);

        return trie.FindByPrefix(prefix.Trim(), limit);
    }

    public void Invalidate()
    {
        Interlocked.Increment(ref _version);
        _snapshot = null;
    }

    public void Dispose() => _buildLock.Dispose();

    private async Task<Trie<EmployeeSuggestion>> GetTrieAsync(CancellationToken cancellationToken)
    {
        if (IsFresh(_snapshot))
        {
            return _snapshot!.Trie;
        }

        await _buildLock.WaitAsync(cancellationToken);

        try
        {
            // Another request may have rebuilt it while this one waited.
            if (IsFresh(_snapshot))
            {
                return _snapshot!.Trie;
            }

            int version = Volatile.Read(ref _version);
            Trie<EmployeeSuggestion> trie = await BuildAsync(cancellationToken);

            // An invalidation during the build means the data just loaded may already be stale:
            // serve it to this caller, but don't cache it.
            if (version == Volatile.Read(ref _version))
            {
                _snapshot = new Snapshot(trie, timeProvider.GetUtcNow());
            }

            return trie;
        }
        finally
        {
            _buildLock.Release();
        }
    }

    private async Task<Trie<EmployeeSuggestion>> BuildAsync(CancellationToken cancellationToken)
    {
        await using DbConnection connection = await connectionFactory.OpenAsync(cancellationToken);

        IEnumerable<Row> rows = await connection.QueryAsync<Row>(
            new CommandDefinition(Sql, cancellationToken: cancellationToken));

        Trie<EmployeeSuggestion> trie = new(ignoreCase: true);

        foreach (Row row in rows)
        {
            string fullName = $"{row.FirstName} {row.LastName}";
            EmployeeSuggestion suggestion = new(row.Id, fullName, row.Email, row.DepartmentName);

            trie.Insert(row.FirstName, suggestion);
            trie.Insert(row.LastName, suggestion);
            trie.Insert(fullName, suggestion);
            trie.Insert(row.Email, suggestion);
        }

        return trie;
    }

    private bool IsFresh(Snapshot? snapshot) =>
        snapshot is not null && timeProvider.GetUtcNow() - snapshot.BuiltAt < TimeToLive;

    private sealed record Snapshot(Trie<EmployeeSuggestion> Trie, DateTimeOffset BuiltAt);

    private sealed record Row(int Id, string FirstName, string LastName, string Email, string DepartmentName);
}
