using AIDeveloperAssistantPOC.Services;

string? apiKey =
    Environment.GetEnvironmentVariable(
        "OPENAI_API_KEY");

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine(
        "OPENAI_API_KEY is not configured.");

    return;
}

AIService aiService =
    new(apiKey);

Console.WriteLine();
Console.WriteLine("========================================");
Console.WriteLine("       AI DEVELOPER ASSISTANT");
Console.WriteLine("========================================");
Console.WriteLine();

Console.Write("Enter your prompt: ");

string? prompt =
    Console.ReadLine();

if (string.IsNullOrWhiteSpace(prompt))
{
    Console.WriteLine(
        "Prompt is required.");

    return;
}

try
{
    Console.WriteLine();
    Console.WriteLine("AI is thinking...");
    Console.WriteLine();

    string response =
        await aiService.AskAsync(prompt);

    Console.WriteLine("AI RESPONSE");
    Console.WriteLine("----------------------------------------");
    Console.WriteLine(response);
    Console.WriteLine("----------------------------------------");
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine("AI request failed.");
    Console.WriteLine(ex.Message);
}