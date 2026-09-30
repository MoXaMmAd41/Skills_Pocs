namespace EmployeeManagement.Infrastructure.Persistence;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    /// <summary>
    /// Applies pending EF Core migrations at startup. Convenient for local/dev containers;
    /// production should run migrations from the pipeline instead.
    /// </summary>
    public bool ApplyMigrationsOnStartup { get; init; }

    /// <summary>
    /// Seeds roles and one demo account per role. Never enable outside of development/demo environments.
    /// </summary>
    public bool SeedDemoUsers { get; init; }
}
