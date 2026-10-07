using EmployeeManagement.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Persistence.Configurations;

internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).HasMaxLength(Department.NameMaxLength).IsRequired();

        builder.Property(d => d.Description).HasMaxLength(Department.DescriptionMaxLength);

        builder.HasIndex(d => d.Name).IsUnique();

        builder.HasData(
            new { Id = 1, Name = "IT", Description = "Information Technology" },
            new { Id = 2, Name = "HR", Description = "Human Resources" },
            new { Id = 3, Name = "Finance", Description = "Finance Department" });
    }
}
