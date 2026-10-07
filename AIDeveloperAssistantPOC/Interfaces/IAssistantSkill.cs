using AIDeveloperAssistantPOC.Models;

namespace AIDeveloperAssistantPOC.Interfaces;

/// <summary>
/// A focused task the assistant can perform. Adding a skill = one class + one DI registration.
/// </summary>
public interface IAssistantSkill
{
    string Title { get; }

    /// <summary>Tells the user what to paste or load for this skill.</summary>
    string InputHint { get; }

    AIRequest CreateRequest(string input);
}
