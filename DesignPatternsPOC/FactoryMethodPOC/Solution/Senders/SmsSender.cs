using DesignPatterns.FactoryMethod.Solution.Channels;

namespace DesignPatterns.FactoryMethod.Solution.Senders;

public sealed class SmsSender : NotificationSender
{
    protected override INotificationChannel CreateChannel() => new SmsChannel();
}
