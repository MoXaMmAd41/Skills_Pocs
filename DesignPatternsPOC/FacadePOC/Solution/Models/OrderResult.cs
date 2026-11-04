namespace DesignPatterns.Facade.Solution.Models;

public sealed record OrderResult(bool Success, string Message, string? TrackingNumber = null);
