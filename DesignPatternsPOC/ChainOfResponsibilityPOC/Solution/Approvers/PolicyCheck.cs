using DesignPatterns.ChainOfResponsibility.Solution.Models;

namespace DesignPatterns.ChainOfResponsibility.Solution.Approvers;

/// <summary>Not every link approves: this one can also stop the chain early.</summary>
public sealed class PolicyCheck : Approver
{
    protected override string Title => "Policy";

    protected override bool CanApprove(ExpenseRequest request) =>
        request.Amount <= 0 || string.IsNullOrWhiteSpace(request.Purpose)
            ? throw new ArgumentException("An expense needs a positive amount and a purpose.")
            : false;
}
