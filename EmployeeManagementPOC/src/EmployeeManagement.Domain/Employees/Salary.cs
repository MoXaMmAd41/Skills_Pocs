using EmployeeManagement.Domain.Common;

namespace EmployeeManagement.Domain.Employees;

public sealed record Salary : IComparable<Salary>
{
    public const decimal Max = 1_000_000m;

    private Salary(decimal amount) => Amount = amount;

    public decimal Amount { get; }

    public static Salary Create(decimal amount) =>
        amount is < 0 or > Max
            ? throw new DomainException(EmployeeErrorCodes.InvalidSalary, $"Salary must be between 0 and {Max:N0}.")
            : new Salary(decimal.Round(amount, 2));

    public int CompareTo(Salary? other) => other is null ? 1 : Amount.CompareTo(other.Amount);

    public override string ToString() => Amount.ToString("N2");
}
