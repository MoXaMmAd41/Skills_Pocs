using AIDeveloperAssistantPOC.Interfaces;
using AIDeveloperAssistantPOC.Models;
using AIDeveloperAssistantPOC.Services;

namespace AIDeveloperAssistantPOC.Skills;

public sealed class ExceptionAnalyzer : IAssistantSkill
{
    private static readonly string Instructions = PromptBuilder
        .ForRole("a senior .NET engineer who specialises in production debugging")
        .WithTask(
            "Analyze the exception message and stack trace (and any code provided) and explain " +
            "why it happened and how to fix it.")
        .WithRules(
            "Point to the exact frame (type, method, file:line) where the fault originates, not where it surfaced.",
            "Separate the root cause from its symptoms.",
            "Give the fix as a minimal C# code change, not a rewrite.",
            "Rank multiple possible causes from most to least likely.")
        .WithOutputSections("Summary", "Root cause", "Where it happens", "Fix", "How to prevent it")
        .Build();

    public string Title => "Analyze an exception";

    public string InputHint => "Paste the exception message and stack trace (optionally the related code).";

    public AIRequest CreateRequest(string input) => new(Instructions, input);
}
