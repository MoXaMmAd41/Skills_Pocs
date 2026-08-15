#pragma warning disable OPENAI001

using OpenAI.Responses;

namespace AIDeveloperAssistantPOC.Services;

public sealed class AIService
{
    private readonly ResponsesClient _client;

    public AIService(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key is not configured.");
        }

        _client = new ResponsesClient(
            apiKey: apiKey);
    }

    public async Task<string> AskAsync(
        string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException(
                "Prompt is required.",
                nameof(prompt));
        }

        ResponseResult response =
            await _client.CreateResponseAsync(
                "gpt-5",
                prompt);

        return response.GetOutputText();
    }
}

#pragma warning restore OPENAI001