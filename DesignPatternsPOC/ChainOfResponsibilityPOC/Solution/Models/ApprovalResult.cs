namespace DesignPatterns.ChainOfResponsibility.Solution.Models;

public sealed record ApprovalResult(bool Approved, string DecidedBy, string Reason);
