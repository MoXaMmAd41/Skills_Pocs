namespace DesignPatterns.FactoryMethod.Solution.Channels;

public sealed class EmailChannel : INotificationChannel
{
    public string Name => "Email";

    public string Deliver(string recipient, string message) => $"[Email -> {recipient}] Subject: Notice | {message}";
}
