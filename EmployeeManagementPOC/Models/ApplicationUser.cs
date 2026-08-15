using Microsoft.AspNetCore.Identity;

namespace EmployeeManagementPOC.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}