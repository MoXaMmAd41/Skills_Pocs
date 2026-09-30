using AIDeveloperAssistantPOC.Interfaces;
using AIDeveloperAssistantPOC.Models;
using AIDeveloperAssistantPOC.Services;

namespace AIDeveloperAssistantPOC.Skills;

/// <summary>
/// Reviews the developer's own code for security weaknesses and produces automated tests that
/// guard against them, so fixes stay fixed.
/// </summary>
public sealed class SecurityTestGenerator : IAssistantSkill
{
    private static readonly string Instructions = PromptBuilder
        .ForRole("an application security engineer reviewing a team's own ASP.NET Core code")
        .WithTask(
            "Identify security weaknesses in the provided code and write automated tests that " +
            "verify the code defends against them.")
        .WithRules(
            "Check against the OWASP Top 10: broken access control / IDOR, injection, input validation, " +
            "mass assignment, authentication and session handling, sensitive data exposure, error-message leakage.",
            "Rate each finding Critical / High / Medium / Low and cite the exact member it applies to.",
            "Write xUnit tests (with WebApplicationFactory for endpoints) that fail while the weakness exists and pass once fixed.",
            "Only report issues visible in the input; list anything you could not assess.")
        .WithOutputSections("Findings", "Security tests", "Recommended fixes", "Not assessed")
        .Build();

    public string Title => "Generate security tests";

    public string InputHint => "Paste a controller/service, or load it with @path/to/File.cs.";

    public AIRequest CreateRequest(string input) => new(Instructions, input);
}
