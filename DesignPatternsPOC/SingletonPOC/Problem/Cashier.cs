namespace DesignPatterns.Singleton.Problem;

/// <summary>Shows what the public constructor leads to in practice.</summary>
public sealed class Cashier(string name)
{
    // ❌ Each cashier news up its own generator, so cashier A and cashier B
    //    both issue INV-00001, INV-00002, ... → duplicate invoice numbers.
    private readonly InvoiceNumberGenerator _generator = new();

    public string IssueInvoice() => $"{name}: {_generator.Next()}";
}
