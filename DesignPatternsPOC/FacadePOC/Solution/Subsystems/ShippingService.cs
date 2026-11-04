namespace DesignPatterns.Facade.Solution.Subsystems;

public sealed class ShippingService
{
    private int _next = 1000;

    public string CreateShipment(string address) => $"TRK-{++_next}";
}
