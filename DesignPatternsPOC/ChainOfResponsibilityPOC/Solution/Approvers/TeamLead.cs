using DesignPatterns.ChainOfResponsibility.Solution.Models;

namespace DesignPatterns.ChainOfResponsibility.Solution.Approvers;

public sealed class TeamLead : Approver
{
    protected override string Title => "Team Lead";

    protected override bool CanApprove(ExpenseRequest request) => request.Amount <= 500;
}
