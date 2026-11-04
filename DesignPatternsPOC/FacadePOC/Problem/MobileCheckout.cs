using DesignPatterns.Facade.Solution.Models;
using DesignPatterns.Facade.Solution.Subsystems;

namespace DesignPatterns.Facade.Problem;

/// <summary>
/// The same checkout, written again by another team for the mobile app.
/// Without a facade the steps are copied, and the copies drift apart.
/// </summary>
public sealed class MobileCheckout(
    InventoryService inventory,
    PaymentService payment,
    ShippingService shipping,
    NotificationService notifications)
{
    public OrderResult PlaceOrder(string customer, string address, IReadOnlyList<OrderLine> lines)
    {
        // ❌ 1. Duplicated logic: any rule change has to be made here AND in WebCheckout.

        // ❌ 2. Skipped the availability check, so stock can go negative.
        inventory.Reserve(lines);

        // ❌ 3. Wrong order: the shipment is created BEFORE payment succeeds.
        string tracking = shipping.CreateShipment(address);

        decimal total = lines.Sum(l => l.Quantity * l.UnitPrice);

        if (!payment.Charge(customer, total))
        {
            // ❌ 4. Forgot inventory.Release(lines): the items stay reserved forever after a failed payment.
            return new OrderResult(false, $"Payment of {total:N2} declined.");
        }

        notifications.Notify(customer, $"Order confirmed ({total:N2}). Tracking: {tracking}");

        return new OrderResult(true, $"Paid {total:N2}.", tracking);
    }
}
