using AIDeveloperAssistantPOC.Configuration;
using AIDeveloperAssistantPOC.Services;
using AIDeveloperAssistantPOC.Tests.Fakes;
using Microsoft.Extensions.Options;

namespace AIDeveloperAssistantPOC.Tests;

public sealed class ConsoleInputReaderTests : IDisposable
{
    private readonly string _tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");

    [Fact]
    public void Read_ReturnsLinesUntilEndMarker_WithoutTheMarker()
    {
        using ConsoleScope console = new("line one", "line two", "END", "ignored");

        string? input = CreateReader().Read(out string? error);

        Assert.Null(error);
        Assert.Equal($"line one{Environment.NewLine}line two{Environment.NewLine}", input);
    }

    [Fact]
    public void Read_AcceptsEndMarkerCaseInsensitively()
    {
        using ConsoleScope console = new("text", "  end  ");

        Assert.Equal($"text{Environment.NewLine}", CreateReader().Read(out _));
    }

    [Theory]
    [InlineData("diff between list and dictionary end", "diff between list and dictionary")]
    [InlineData("my question END  ", "my question")]
    public void Read_AcceptsEndMarkerAtEndOfLine(string line, string expected)
    {
        using ConsoleScope console = new(line, "never read");

        Assert.Equal($"{expected}{Environment.NewLine}", CreateReader().Read(out _));
    }

    [Fact]
    public void Read_DoesNotTreatWordsEndingInEndAsMarker()
    {
        using ConsoleScope console = new("please append", "END");

        Assert.Equal($"please append{Environment.NewLine}", CreateReader().Read(out _));
    }

    [Fact]
    public void Read_LoadsFile_WhenFirstLineStartsWithAt()
    {
        File.WriteAllText(_tempFile, "public class Foo { }");
        using ConsoleScope console = new($"@{_tempFile}");

        string? input = CreateReader().Read(out string? error);

        Assert.Null(error);
        Assert.Contains($"File: {Path.GetFileName(_tempFile)}", input);
        Assert.Contains("public class Foo { }", input);
    }

    [Fact]
    public void Read_AcceptsQuotedFilePath()
    {
        File.WriteAllText(_tempFile, "content");
        using ConsoleScope console = new($"@\"{_tempFile}\"");

        Assert.Contains("content", CreateReader().Read(out _));
    }

    [Fact]
    public void Read_ReportsMissingFile()
    {
        using ConsoleScope console = new("@does-not-exist.cs");

        Assert.Null(CreateReader().Read(out string? error));
        Assert.StartsWith("File not found", error);
    }

    [Fact]
    public void Read_RejectsEmptyInput()
    {
        using ConsoleScope console = new("END");

        Assert.Null(CreateReader().Read(out string? error));
        Assert.Equal("Input is empty.", error);
    }

    [Fact]
    public void Read_RejectsInputOverTheLimit()
    {
        using ConsoleScope console = new(new string('a', 1_500), "END");

        Assert.Null(CreateReader(maxCharacters: 1_000).Read(out string? error));
        Assert.Contains("the limit is 1,000", error);
    }

    public void Dispose() => File.Delete(_tempFile);

    private static ConsoleInputReader CreateReader(int maxCharacters = 100_000) =>
        new(Options.Create(new AIOptions { ApiKey = "test", MaxInputCharacters = maxCharacters }));
}
