using DesignPatterns.ChainOfResponsibility.Solution.Models;

namespace DesignPatterns.ChainOfResponsibility.Solution.Approvers;

public sealed class Manager : Approver
{
    protected override string Title => "Manager";

    protected override bool CanApprove(ExpenseRequest request) => request.Amount <= 5_000;
}
