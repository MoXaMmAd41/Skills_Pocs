namespace DesignPatterns.FactoryMethod.Solution.Channels;

public sealed class PushChannel : INotificationChannel
{
    public string Name => "Push";

    public string Deliver(string recipient, string message) => $"[Push -> device of {recipient}] 🔔 {message}";
}
