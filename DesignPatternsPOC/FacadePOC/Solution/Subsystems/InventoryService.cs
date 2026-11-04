using DesignPatterns.Facade.Solution.Models;

namespace DesignPatterns.Facade.Solution.Subsystems;

/// <summary>Subsystem: useful on its own, but the caller would have to know when to reserve and release.</summary>
public sealed class InventoryService(Dictionary<string, int> stock)
{
    public bool IsAvailable(IEnumerable<OrderLine> lines) =>
        lines.All(l => stock.GetValueOrDefault(l.Sku) >= l.Quantity);

    public void Reserve(IEnumerable<OrderLine> lines)
    {
        foreach (OrderLine line in lines) stock[line.Sku] -= line.Quantity;
    }

    public void Release(IEnumerable<OrderLine> lines)
    {
        foreach (OrderLine line in lines) stock[line.Sku] += line.Quantity;
    }

    public int InStock(string sku) => stock.GetValueOrDefault(sku);
}
