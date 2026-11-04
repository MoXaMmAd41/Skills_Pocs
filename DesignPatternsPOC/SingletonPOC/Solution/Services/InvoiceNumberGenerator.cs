namespace DesignPatterns.Singleton.Solution.Services;

/// <summary>
/// Singleton = exactly one instance for the whole process, reachable from anywhere.
/// Invoice numbers must never repeat, so every caller has to share ONE counter.
/// <para>
/// Lazy&lt;T&gt; makes creation thread-safe and deferred until first use;
/// Interlocked makes the counter itself thread-safe.
/// </para>
/// </summary>
public sealed class InvoiceNumberGenerator
{
    private static readonly Lazy<InvoiceNumberGenerator> LazyInstance = new(() => new InvoiceNumberGenerator());

    private long _last;

    // Private constructor: nobody outside can call `new`.
    private InvoiceNumberGenerator()
    {
        CreatedAt = DateTime.Now;
    }

    public static InvoiceNumberGenerator Instance => LazyInstance.Value;

    public DateTime CreatedAt { get; }

    public string Next() => $"INV-{Interlocked.Increment(ref _last):D5}";
}
