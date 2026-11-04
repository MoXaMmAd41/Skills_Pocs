using DesignPatterns.Facade.Solution.Models;
using DesignPatterns.Facade.Solution.Subsystems;

namespace DesignPatterns.Facade.Problem;

/// <summary>
/// The naive version: the website talks to all four subsystems itself.
/// This one happens to be correct, but every caller has to know all of this.
/// Compare with Solution/CheckoutFacade.cs.
/// </summary>
public sealed class WebCheckout(
    InventoryService inventory,
    PaymentService payment,
    ShippingService shipping,
    NotificationService notifications)
{
    // ❌ 1. The caller depends on FOUR services and has to know the right order to call them in.
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
            // ❌ 2. Easy to forget. See MobileCheckout, which did forget.
            inventory.Release(lines);
            notifications.Notify(customer, $"Payment of {total:N2} was declined.");
            return new OrderResult(false, $"Payment of {total:N2} declined.");
        }

        string tracking = shipping.CreateShipment(address);
        notifications.Notify(customer, $"Order confirmed ({total:N2}). Tracking: {tracking}");

        return new OrderResult(true, $"Paid {total:N2}.", tracking);
    }
}
