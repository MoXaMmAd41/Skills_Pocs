namespace DesignPatterns.FactoryMethod.Solution.Channels;

/// <summary>The product: what every channel can do.</summary>
public interface INotificationChannel
{
    string Name { get; }

    string Deliver(string recipient, string message);
}
