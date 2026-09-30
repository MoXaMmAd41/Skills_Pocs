using Asp.Versioning;

namespace EmployeeManagement.Api.Versioning;

public static class ApiVersions
{
    public const string V1 = "1.0";
    public const string V2 = "2.0";

    public static readonly ApiVersion Default = new(1, 0);
}
