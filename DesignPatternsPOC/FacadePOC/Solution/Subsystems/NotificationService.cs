namespace DesignPatterns.Facade.Solution.Subsystems;

public sealed class NotificationService
{
    public List<string> Sent { get; } = [];

    public void Notify(string customer, string text) => Sent.Add($"{customer}: {text}");
}
