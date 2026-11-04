namespace DesignPatterns.Facade.Solution.Models;

public sealed record OrderLine(string Sku, int Quantity, decimal UnitPrice);
