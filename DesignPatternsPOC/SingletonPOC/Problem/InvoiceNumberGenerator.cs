namespace DesignPatterns.Singleton.Problem;

/// <summary>
/// The naive version: a normal class with a public constructor.
/// Compare with Solution/Services/InvoiceNumberGenerator.cs.
/// </summary>
public sealed class InvoiceNumberGenerator
{
    private long _last;

    // ❌ 1. Public constructor: anyone can create their own generator, and each one starts counting from 1.
    public InvoiceNumberGenerator()
    {
    }

    // ❌ 2. `++` is read-add-write. Two threads can read the same value and both return INV-00007.
    public string Next() => $"INV-{++_last:D5}";
}
