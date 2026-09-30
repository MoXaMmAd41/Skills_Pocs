using AIDeveloperAssistantPOC.Configuration;
using AIDeveloperAssistantPOC.Interfaces;
using AIDeveloperAssistantPOC.Models;
using AIDeveloperAssistantPOC.Services;
using AIDeveloperAssistantPOC.Skills;
using AIDeveloperAssistantPOC.Tests.Fakes;
using Microsoft.Extensions.Options;

namespace AIDeveloperAssistantPOC.Tests;

public sealed class ConsoleMenuServiceTests
{
    [Fact]
    public async Task RunAsync_SendsSelectedSkillsRequest_AndPrintsStreamedAnswer()
    {
        FakeAIService ai = new("Hello ", "world");
        using ConsoleScope console = new("1", "NullReferenceException at Foo.Bar()", "END", "0");

        await CreateMenu(ai).RunAsync();

        AIRequest request = Assert.Single(ai.Requests);
        Assert.Equal($"NullReferenceException at Foo.Bar(){Environment.NewLine}", request.Input);
        Assert.Equal(new ExceptionAnalyzer().CreateRequest("x").Instructions, request.Instructions);
        Assert.Contains("Hello world", console.Output);
    }

    [Fact]
    public async Task RunAsync_RejectsInvalidOption_AndShowsMenuAgain()
    {
        FakeAIService ai = new();
        using ConsoleScope console = new("9", "abc", "0");

        await CreateMenu(ai).RunAsync();

        Assert.Contains("'9' is not a valid option.", console.Output);
        Assert.Contains("'abc' is not a valid option.", console.Output);
        Assert.Empty(ai.Requests);
    }

    [Fact]
    public async Task RunAsync_DoesNotCallAI_WhenInputIsInvalid()
    {
        FakeAIService ai = new();
        using ConsoleScope console = new("2", "@missing-file.cs", "0");

        await CreateMenu(ai).RunAsync();

        Assert.Contains("File not found", console.Output);
        Assert.Empty(ai.Requests);
    }

    [Fact]
    public async Task RunAsync_ReportsNetworkFailure_AndKeepsRunning()
    {
        FakeAIService ai = new() { ExceptionToThrow = new HttpRequestException("No such host") };
        using ConsoleScope console = new("4", "question", "END", "4", "again", "END", "0");

        await CreateMenu(ai).RunAsync();

        Assert.Contains("Could not reach the AI service: No such host", console.Output);
        Assert.Equal(2, ai.Requests.Count);
    }

    [Theory]
    [InlineData("credit_balance_exhausted", "has no credits")]
    [InlineData("insufficient_quota", "has no credits")]
    [InlineData("server_error", "Something broke")]
    public async Task RunAsync_ExplainsProviderFailures(string code, string expectedMessage)
    {
        FakeAIService ai = new() { ExceptionToThrow = new AIServiceException(code, "Something broke") };
        using ConsoleScope console = new("4", "question", "END", "0");

        await CreateMenu(ai).RunAsync();

        Assert.Contains($"AI request failed: ", console.Output);
        Assert.Contains(expectedMessage, console.Output);
    }

    [Fact]
    public async Task RunAsync_Exits_WhenInputEnds()
    {
        using ConsoleScope console = new();

        await CreateMenu(new FakeAIService()).RunAsync();

        Assert.Contains("AI DEVELOPER ASSISTANT", console.Output);
    }

    private static ConsoleMenuService CreateMenu(IAIService ai)
    {
        IAssistantSkill[] skills =
        [
            new ExceptionAnalyzer(),
            new DocumentationGenerator(),
            new SecurityTestGenerator(),
            new GeneralAssistant()
        ];

        ConsoleInputReader reader = new(Options.Create(new AIOptions { ApiKey = "test" }));

        return new ConsoleMenuService(skills, ai, reader);
    }
}
