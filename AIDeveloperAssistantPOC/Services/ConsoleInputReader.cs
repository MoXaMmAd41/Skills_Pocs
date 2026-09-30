using System.Text;
using AIDeveloperAssistantPOC.Configuration;
using Microsoft.Extensions.Options;

namespace AIDeveloperAssistantPOC.Services;

/// <summary>
/// Reads multi-line input from the console, or the contents of a file when the first line is <c>@path</c>.
/// </summary>
public sealed class ConsoleInputReader(IOptions<AIOptions> options)
{
    public const string EndMarker = "END";

    private readonly int _maxCharacters = options.Value.MaxInputCharacters;

    /// <returns>The input, or <c>null</c> with an explanation in <paramref name="error"/>.</returns>
    public string? Read(out string? error)
    {
        error = null;
        string? firstLine = Console.ReadLine();

        if (firstLine is null)
        {
            error = "No input received.";
            return null;
        }

        string input = firstLine.TrimStart().StartsWith('@')
            ? ReadFile(firstLine.Trim()[1..].Trim('"', ' '), out error) ?? string.Empty
            : ReadUntilEndMarker(firstLine);

        if (error is not null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(input))
        {
            error = "Input is empty.";
            return null;
        }

        if (input.Length > _maxCharacters)
        {
            error = $"Input is {input.Length:N0} characters; the limit is {_maxCharacters:N0}. Send a smaller piece.";
            return null;
        }

        return input;
    }

    private static string ReadUntilEndMarker(string firstLine)
    {
        StringBuilder input = new();

        for (string? line = firstLine; line is not null; line = Console.ReadLine())
        {
            string trimmed = line.TrimEnd();

            if (trimmed.Trim().Equals(EndMarker, StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            // Also accept the marker typed at the end of the last line: "my question END".
            if (EndsWithMarker(trimmed))
            {
                input.AppendLine(trimmed[..^EndMarker.Length].TrimEnd());
                break;
            }

            input.AppendLine(line);
        }

        return input.ToString();
    }

    private static bool EndsWithMarker(string line) =>
        line.Length > EndMarker.Length
        && char.IsWhiteSpace(line[^(EndMarker.Length + 1)])
        && line.EndsWith(EndMarker, StringComparison.OrdinalIgnoreCase);

    private static string? ReadFile(string path, out string? error)
    {
        error = null;
        string fullPath = Path.GetFullPath(path);

        if (!File.Exists(fullPath))
        {
            error = $"File not found: {fullPath}";
            return null;
        }

        // The file name gives the model useful context (layer, type name, language).
        return $"File: {Path.GetFileName(fullPath)}\n```\n{File.ReadAllText(fullPath)}\n```";
    }
}
