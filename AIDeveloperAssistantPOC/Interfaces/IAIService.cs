using AIDeveloperAssistantPOC.Models;

namespace AIDeveloperAssistantPOC.Interfaces;

public interface IAIService
{
    /// <summary>Streams the model's answer as text chunks while it is generated.</summary>
    IAsyncEnumerable<string> StreamAsync(AIRequest request, CancellationToken cancellationToken = default);
}
