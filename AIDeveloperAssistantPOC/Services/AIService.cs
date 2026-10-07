using System.Runtime.CompilerServices;
using AIDeveloperAssistantPOC.Configuration;
using AIDeveloperAssistantPOC.Interfaces;
using AIDeveloperAssistantPOC.Models;
using Microsoft.Extensions.Options;
using OpenAI.Responses;

namespace AIDeveloperAssistantPOC.Services;

/// <summary>OpenAI Responses API implementation of <see cref="IAIService"/>.</summary>
public sealed class AIService(IOptions<AIOptions> options) : IAIService
{
    private readonly AIOptions _options = options.Value;
    private readonly ResponsesClient _client = new(options.Value.ApiKey);

    public async IAsyncEnumerable<string> StreamAsync(
        AIRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Input);

        CreateResponseOptions responseOptions = new()
        {
            Model = _options.Model,
            Instructions = request.Instructions,
            InputItems = { ResponseItem.CreateUserMessageItem(PromptBuilder.WrapInput(request.Input)) },
            ReasoningOptions = new ResponseReasoningOptions { ReasoningEffortLevel = ToSdkEffort(_options.ReasoningEffort) },
            StreamingEnabled = true,
            // Nothing here needs to be retrievable later; don't keep code or logs on the provider side.
            StoredOutputEnabled = false
        };

        await foreach (StreamingResponseUpdate update in _client.CreateResponseStreamingAsync(responseOptions, cancellationToken))
        {
            switch (update)
            {
                case StreamingResponseOutputTextDeltaUpdate delta:
                    yield return delta.Delta;
                    break;

                // Failures after the stream has opened arrive as events, not HTTP errors.
                // The "failed" event carries the detailed error, so surface that one.
                case StreamingResponseFailedUpdate failed:
                    throw new AIServiceException(
                        failed.Response.Error?.Code.ToString(),
                        failed.Response.Error?.Message ?? "The AI service failed to generate a response.");

                case StreamingResponseIncompleteUpdate incomplete:
                    throw new AIServiceException(
                        incomplete.Response.IncompleteStatusDetails?.Reason?.ToString(),
                        "The response was cut off before it finished.");
            }
        }
    }

    private static ResponseReasoningEffortLevel ToSdkEffort(ReasoningEffort effort) => effort switch
    {
        ReasoningEffort.Low => ResponseReasoningEffortLevel.Low,
        ReasoningEffort.High => ResponseReasoningEffortLevel.High,
        _ => ResponseReasoningEffortLevel.Medium
    };
}
