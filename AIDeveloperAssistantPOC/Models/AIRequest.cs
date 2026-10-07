namespace AIDeveloperAssistantPOC.Models;

/// <param name="Instructions">System-level guidance: role, task, rules and output format.</param>
/// <param name="Input">The user-supplied content the model should work on.</param>
public sealed record AIRequest(string Instructions, string Input);
