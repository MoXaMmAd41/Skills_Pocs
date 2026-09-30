using EmployeeManagement.Domain.Departments;
using EmployeeManagement.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Persistence.Configurations;

internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName).HasMaxLength(Employee.NameMaxLength).IsRequired();

        builder.Property(e => e.LastName).HasMaxLength(Employee.NameMaxLength).IsRequired();

        builder.Property(e => e.Email)
            .HasConversion(email => email.Value, value => Email.Create(value))
            .HasMaxLength(Email.MaxLength)
            .IsRequired();

        builder.Property(e => e.Phone).HasMaxLength(Employee.PhoneMaxLength);

        builder.Property(e => e.Salary).HasColumnType("decimal(18,2)");

        builder.Ignore(e => e.FullName);

        builder.HasIndex(e => e.Email).IsUnique();

        // Aggregates reference each other by identity only; the FK keeps the data consistent.
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
