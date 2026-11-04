namespace DesignPatterns.FactoryMethod.Solution.Channels;

public sealed class SmsChannel : INotificationChannel
{
    private const int MaxLength = 30;

    public string Name => "SMS";

    // SMS has a length limit, so the channel trims the text itself.
    public string Deliver(string recipient, string message) =>
        $"[SMS -> {recipient}] {(message.Length > MaxLength ? message[..MaxLength] + "..." : message)}";
}
