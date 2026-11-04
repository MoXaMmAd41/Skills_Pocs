using DesignPatterns.FactoryMethod.Solution.Channels;

namespace DesignPatterns.FactoryMethod.Solution.Senders;

public sealed class EmailSender : NotificationSender
{
    protected override INotificationChannel CreateChannel() => new EmailChannel();
}
