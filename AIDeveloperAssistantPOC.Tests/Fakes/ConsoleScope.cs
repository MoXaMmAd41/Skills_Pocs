namespace AIDeveloperAssistantPOC.Tests.Fakes;

/// <summary>
/// Feeds scripted lines to <see cref="Console.In"/> and captures <see cref="Console.Out"/>,
/// restoring the real console on dispose.
/// </summary>
internal sealed class ConsoleScope : IDisposable
{
    private readonly TextReader _originalIn = Console.In;
    private readonly TextWriter _originalOut = Console.Out;
    private readonly StringWriter _output = new();

    public ConsoleScope(params string[] inputLines)
    {
        Console.SetIn(new StringReader(string.Join(Environment.NewLine, inputLines) + Environment.NewLine));
        Console.SetOut(_output);
    }

    public string Output => _output.ToString();

    public void Dispose()
    {
        Console.SetIn(_originalIn);
        Console.SetOut(_originalOut);
    }
}
