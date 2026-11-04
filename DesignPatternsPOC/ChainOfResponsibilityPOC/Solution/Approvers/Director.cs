using DesignPatterns.ChainOfResponsibility.Solution.Models;

namespace DesignPatterns.ChainOfResponsibility.Solution.Approvers;

public sealed class Director : Approver
{
    protected override string Title => "Director";

    protected override bool CanApprove(ExpenseRequest request) => request.Amount <= 20_000;
}
