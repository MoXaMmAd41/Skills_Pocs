namespace DesignPatterns.FactoryMethod.Problem;

/// <summary>
/// The naive version: one class decides HOW to send AND which channel to use, through a switch on a string.
/// Compare with Solution/Senders/NotificationSender.cs.
/// </summary>
public sealed class NotificationService
{
    public string Send(string channelType, string recipient, string message)
    {
        // ❌ 1. The workflow (validate, trim, deliver) is tangled with channel-specific details.
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message is required.", nameof(message));
        }

        message = message.Trim();

        // ❌ 2. Adding WhatsApp means EDITING this method (and every other switch like it in the codebase).
        //       Every edit risks breaking Email/SMS, which already work. This breaks Open/Closed.
        // ❌ 3. A typo like "Email" vs "email" compiles fine and only fails at runtime.
        switch (channelType)
        {
            case "email":
                return $"[Email -> {recipient}] Subject: Notice | {message}";

            case "sms":
                // ❌ 4. SMS-only rules live here too, so this method keeps growing with every channel's quirks.
                return $"[SMS -> {recipient}] {(message.Length > 30 ? message[..30] + "..." : message)}";

            case "push":
                return $"[Push -> device of {recipient}] 🔔 {message}";

            default:
                throw new NotSupportedException($"Unknown channel '{channelType}'.");
        }
    }
}
