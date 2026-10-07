using System.Runtime.CompilerServices;
using AIDeveloperAssistantPOC.Interfaces;
using AIDeveloperAssistantPOC.Models;

namespace AIDeveloperAssistantPOC.Tests.Fakes;

/// <summary>Stands in for OpenAI: records requests and streams canned chunks, or throws.</summary>
internal sealed class FakeAIService(params string[] chunks) : IAIService
{
    public List<AIRequest> Requests { get; } = [];

    public Exception? ExceptionToThrow { get; init; }

    public async IAsyncEnumerable<string> StreamAsync(
        AIRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Requests.Add(request);

        if (ExceptionToThrow is not null)
        {
            throw ExceptionToThrow;
        }

        foreach (string chunk in chunks)
        {
            await Task.Yield();
            yield return chunk;
        }
    }
}
