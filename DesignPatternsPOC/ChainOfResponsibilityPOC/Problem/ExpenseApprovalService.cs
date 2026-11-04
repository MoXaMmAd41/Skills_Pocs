using DesignPatterns.ChainOfResponsibility.Solution.Models;

namespace DesignPatterns.ChainOfResponsibility.Problem;

/// <summary>
/// The naive version: every approval rule in one if/else ladder.
/// Compare with Solution/Approvers/, where each rule is its own class.
/// </summary>
public sealed class ExpenseApprovalService
{
    public ApprovalResult Approve(ExpenseRequest request)
    {
        // ❌ 1. Validation, limits and the "who's next" order are all hard-coded in one method.
        if (request.Amount <= 0 || string.IsNullOrWhiteSpace(request.Purpose))
        {
            throw new ArgumentException("An expense needs a positive amount and a purpose.");
        }

        // ❌ 2. Adding a "Finance Review" step between Manager and Director means editing this ladder,
        //       and the order of the branches silently matters (swap two and the wrong person approves).
        // ❌ 3. Can't reuse the rules: a different flow (say, travel expenses that skip Team Lead)
        //       needs a second copy of this method.
        // ❌ 4. Testing the Manager rule alone is impossible; every test goes through the whole ladder.
        if (request.Amount <= 500)
        {
            return new ApprovalResult(true, "Team Lead", "within Team Lead limit");
        }
        else if (request.Amount <= 5_000)
        {
            return new ApprovalResult(true, "Manager", "within Manager limit");
        }
        else if (request.Amount <= 20_000)
        {
            return new ApprovalResult(true, "Director", "within Director limit");
        }
        else
        {
            return new ApprovalResult(false, "nobody", "no one in the chain can approve this amount");
        }
    }
}
