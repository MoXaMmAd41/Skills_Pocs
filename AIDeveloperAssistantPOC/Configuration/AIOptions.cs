using System.ComponentModel.DataAnnotations;

namespace AIDeveloperAssistantPOC.Configuration;

public sealed class AIOptions
{
    public const string SectionName = "AI";

    /// <summary>
    /// Read from <c>AI:ApiKey</c> (user secrets) or the <c>OPENAI_API_KEY</c> environment variable.
    /// Never commit it to appsettings.json.
    /// </summary>
    [Required(ErrorMessage = "OpenAI API key is not configured. Set the OPENAI_API_KEY environment variable.")]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = "gpt-5";

    public ReasoningEffort ReasoningEffort { get; set; } = ReasoningEffort.Medium;

    /// <summary>
    /// Guards against pasting or loading huge inputs by accident (cost and context-window limits).
    /// </summary>
    [Range(1_000, 1_000_000)]
    public int MaxInputCharacters { get; set; } = 100_000;
}

public enum ReasoningEffort
{
    Low,
    Medium,
    High
}
