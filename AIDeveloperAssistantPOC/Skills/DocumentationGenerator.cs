using AIDeveloperAssistantPOC.Interfaces;
using AIDeveloperAssistantPOC.Models;
using AIDeveloperAssistantPOC.Services;

namespace AIDeveloperAssistantPOC.Skills;

public sealed class DocumentationGenerator : IAssistantSkill
{
    private static readonly string Instructions = PromptBuilder
        .ForRole("a senior .NET engineer who writes concise, accurate technical documentation")
        .WithTask("Document the provided C# code for other developers.")
        .WithRules(
            "Never change behaviour, names or signatures — only add documentation.",
            "Add XML doc comments to public types and members; describe intent, parameters, return values and thrown exceptions.",
            "Explain why, not what: skip comments that only restate the code.",
            "Keep the overview short enough to read in two minutes.")
        .WithOutputSections("Overview", "Documented code", "Usage example", "Notes and caveats")
        .Build();

    public string Title => "Generate documentation";

    public string InputHint => "Paste C# code, or load a file with @path/to/File.cs.";

    public AIRequest CreateRequest(string input) => new(Instructions, input);
}
