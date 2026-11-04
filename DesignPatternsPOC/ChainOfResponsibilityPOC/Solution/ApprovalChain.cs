using DesignPatterns.ChainOfResponsibility.Solution.Approvers;

namespace DesignPatterns.ChainOfResponsibility.Solution;

public static class ApprovalChain
{
    public static Approver Build()
    {
        PolicyCheck head = new();
        head.SetNext(new TeamLead()).SetNext(new Manager()).SetNext(new Director());
        return head;
    }
}
