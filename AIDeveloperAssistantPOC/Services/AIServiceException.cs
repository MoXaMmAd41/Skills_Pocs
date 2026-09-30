namespace AIDeveloperAssistantPOC.Services;

/// <summary>
/// The AI provider accepted the request but reported a failure while generating the answer
/// (e.g. exhausted credits, content filter, server error).
/// </summary>
public sealed class AIServiceException(string? code, string message) : Exception(message)
{
    public string? Code { get; } = code;
}
