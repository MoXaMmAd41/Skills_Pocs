using AIDeveloperAssistantPOC.Interfaces;
using AIDeveloperAssistantPOC.Models;
using AIDeveloperAssistantPOC.Services;

namespace AIDeveloperAssistantPOC.Skills;

public sealed class GeneralAssistant : IAssistantSkill
{
    private static readonly string Instructions = PromptBuilder
        .ForRole("a pragmatic senior software engineer helping a colleague")
        .WithTask("Answer the developer's question.")
        .WithRules("Lead with the answer, then the reasoning.", "Prefer short, runnable code examples.")
        .Build();

    public string Title => "Ask a question";

    public string InputHint => "Type your question (you can include code).";

    public AIRequest CreateRequest(string input) => new(Instructions, input);
}
