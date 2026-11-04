namespace DesignPatterns.Facade.Solution.Subsystems;

public sealed class PaymentService(decimal cardLimit)
{
    public bool Charge(string customer, decimal amount) => amount <= cardLimit;
}
