using DesignPatterns.FactoryMethod.Solution.Channels;

namespace DesignPatterns.FactoryMethod.Solution.Senders;

/// <summary>
/// Factory Method = the base class owns the workflow (Send) but leaves ONE step,
/// creating the channel, to subclasses. Adding WhatsApp means adding a subclass, not editing Send.
/// </summary>
public abstract class NotificationSender
{
    // The factory method.
    protected abstract INotificationChannel CreateChannel();

    public string Send(string recipient, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message is required.", nameof(message));
        }

        INotificationChannel channel = CreateChannel();

        return channel.Deliver(recipient, message.Trim());
    }
}
