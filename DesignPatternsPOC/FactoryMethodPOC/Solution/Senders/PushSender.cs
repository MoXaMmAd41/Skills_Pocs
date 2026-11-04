using DesignPatterns.FactoryMethod.Solution.Channels;

namespace DesignPatterns.FactoryMethod.Solution.Senders;

public sealed class PushSender : NotificationSender
{
    protected override INotificationChannel CreateChannel() => new PushChannel();
}
