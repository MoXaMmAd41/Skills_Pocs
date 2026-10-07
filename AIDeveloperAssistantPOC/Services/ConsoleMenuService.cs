using System.ClientModel;
using System.Net;
using AIDeveloperAssistantPOC.Interfaces;

namespace AIDeveloperAssistantPOC.Services;

public sealed class ConsoleMenuService(
    IEnumerable<IAssistantSkill> skills,
    IAIService aiService,
    ConsoleInputReader inputReader)
{
    private const string Divider = "----------------------------------------";

    private readonly IReadOnlyList<IAssistantSkill> _skills = skills.ToList();

    // Cancels only the request in flight, so Ctrl+C doesn't kill the whole session.
    private CancellationTokenSource? _currentRequest;

    public async Task RunAsync()
    {
        Console.CancelKeyPress += OnCancelKeyPress;

        try
        {
            PrintBanner();

            while (SelectSkill() is { } skill)
            {
                await RunSkillAsync(skill);
            }
        }
        finally
        {
            Console.CancelKeyPress -= OnCancelKeyPress;
        }
    }

    private IAssistantSkill? SelectSkill()
    {
        while (true)
        {
            Console.WriteLine();

            for (int i = 0; i < _skills.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {_skills[i].Title}");
            }

            Console.WriteLine("  0. Exit");
            Console.Write("\nChoose an option: ");

            string? choice = Console.ReadLine()?.Trim();

            if (choice is null or "0")
            {
                return null;
            }

            if (int.TryParse(choice, out int index) && index >= 1 && index <= _skills.Count)
            {
                return _skills[index - 1];
            }

            WriteColored($"'{choice}' is not a valid option.", ConsoleColor.Yellow);
        }
    }

    private async Task RunSkillAsync(IAssistantSkill skill)
    {
        Console.WriteLine();
        Console.WriteLine(skill.InputHint);
        Console.WriteLine($"Finish with {ConsoleInputReader.EndMarker} (on its own line or at the end of the last line), or type @path to load a file.");
        Console.WriteLine();

        string? input = inputReader.Read(out string? error);

        if (input is null)
        {
            WriteColored(error!, ConsoleColor.Yellow);
            return;
        }

        using CancellationTokenSource request = new();
        _currentRequest = request;

        try
        {
            Console.WriteLine($"\n{skill.Title} — thinking... (Ctrl+C to cancel)\n{Divider}");

            await foreach (string chunk in aiService.StreamAsync(skill.CreateRequest(input), request.Token))
            {
                Console.Write(chunk);
            }

            Console.WriteLine($"\n{Divider}");
        }
        catch (OperationCanceledException) when (request.IsCancellationRequested)
        {
            WriteColored("\nRequest cancelled.", ConsoleColor.Yellow);
        }
        catch (ClientResultException ex)
        {
            WriteColored($"\nAI request failed: {Describe(ex)}", ConsoleColor.Red);
        }
        catch (AIServiceException ex)
        {
            WriteColored($"\nAI request failed: {Describe(ex)}", ConsoleColor.Red);
        }
        catch (HttpRequestException ex)
        {
            WriteColored($"\nCould not reach the AI service: {ex.Message}", ConsoleColor.Red);
        }
        finally
        {
            _currentRequest = null;
        }
    }

    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        if (_currentRequest is { } request)
        {
            e.Cancel = true;
            request.Cancel();
        }
    }

    private static string Describe(ClientResultException ex) => (HttpStatusCode)ex.Status switch
    {
        HttpStatusCode.Unauthorized => "the API key is invalid or revoked.",
        HttpStatusCode.Forbidden => "the API key has no access to this model.",
        HttpStatusCode.NotFound => "the configured model was not found. Check AI:Model in appsettings.json.",
        HttpStatusCode.TooManyRequests => "rate limit or quota exceeded. Wait a moment or check your billing.",
        >= HttpStatusCode.InternalServerError => "the AI service is having problems. Try again shortly.",
        _ => ex.Message
    };

    private static string Describe(AIServiceException ex) => ex.Code switch
    {
        "insufficient_quota" or "credit_balance_exhausted" =>
            "your OpenAI account has no credits. Add credits at https://platform.openai.com/settings/organization/billing",
        "rate_limit_exceeded" => "rate limit exceeded. Wait a moment and try again.",
        _ => ex.Message
    };

    private static void PrintBanner()
    {
        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("       AI DEVELOPER ASSISTANT");
        Console.WriteLine("========================================");
    }

    private static void WriteColored(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
