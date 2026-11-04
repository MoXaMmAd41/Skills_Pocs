using DesignPatterns.ChainOfResponsibility.Solution.Models;

namespace DesignPatterns.ChainOfResponsibility.Solution.Approvers;

/// <summary>
/// Chain of Responsibility = pass a request along a chain of handlers until one of them deals with it.
/// The sender doesn't know who will approve; each approver only knows its own limit and the next link.
/// </summary>
public abstract class Approver
{
    private Approver? _next;

    /// <summary>Links the next handler and returns it, so a chain reads left to right.</summary>
    public Approver SetNext(Approver next)
    {
        _next = next;
        return next;
    }

    public ApprovalResult Handle(ExpenseRequest request) =>
        CanApprove(request)
            ? new ApprovalResult(true, Title, $"within {Title} limit")
            : _next?.Handle(request) ?? new ApprovalResult(false, "nobody", "no one in the chain can approve this amount");

    protected abstract string Title { get; }

    protected abstract bool CanApprove(ExpenseRequest request);
}
