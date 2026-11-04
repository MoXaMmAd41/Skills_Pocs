using DesignPatterns.Facade.Solution.Models;
using DesignPatterns.Facade.Solution.Subsystems;

namespace DesignPatterns.Facade.Solution;

/// <summary>
/// Facade = one simple entry point over several subsystems.
/// The caller says "place this order"; the facade knows the steps, their order,
/// and how to undo the stock reservation if payment fails.
/// </summary>
public sealed class CheckoutFacade(
    InventoryService inventory,
    PaymentService payment,
    ShippingService shipping,
    NotificationService notifications)
{
    public OrderResult PlaceOrder(string customer, string address, IReadOnlyList<OrderLine> lines)
    {
        if (!inventory.IsAvailable(lines))
        {
            return new OrderResult(false, "Some items are out of stock.");
        }

        inventory.Reserve(lines);

        decimal total = lines.Sum(l => l.Quantity * l.UnitPrice);

        if (!payment.Charge(customer, total))
        {
            inventory.Release(lines);
            notifications.Notify(customer, $"Payment of {total:N2} was declined.");
            return new OrderResult(false, $"Payment of {total:N2} declined.");
        }

        string tracking = shipping.CreateShipment(address);
        notifications.Notify(customer, $"Order confirmed ({total:N2}). Tracking: {tracking}");

        return new OrderResult(true, $"Paid {total:N2}.", tracking);
    }
}
