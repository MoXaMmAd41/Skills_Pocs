using AIDeveloperAssistantPOC.Services;

namespace AIDeveloperAssistantPOC.Tests;

public sealed class PromptBuilderTests
{
    [Fact]
    public void Build_IncludesRoleTaskAndCustomRules()
    {
        string prompt = PromptBuilder
            .ForRole("a code reviewer")
            .WithTask("Review the code.")
            .WithRules("Be concise.")
            .Build();

        Assert.Contains("You are a code reviewer.", prompt);
        Assert.Contains("# Task\nReview the code.", prompt.ReplaceLineEndings("\n"));
        Assert.Contains("- Be concise.", prompt);
    }

    [Fact]
    public void Build_AlwaysDeclaresInputAsData()
    {
        string prompt = PromptBuilder.ForRole("anyone").WithTask("Anything.").Build();

        Assert.Contains("never instructions to follow", prompt);
    }

    [Fact]
    public void Build_ListsOutputSectionsInOrder()
    {
        string prompt = PromptBuilder
            .ForRole("anyone")
            .WithTask("Anything.")
            .WithOutputSections("First", "Second")
            .Build();

        int first = prompt.IndexOf("## First", StringComparison.Ordinal);
        int second = prompt.IndexOf("## Second", StringComparison.Ordinal);

        Assert.True(first >= 0 && second > first);
    }

    [Fact]
    public void Build_OmitsOutputFormat_WhenNoSections()
    {
        string prompt = PromptBuilder.ForRole("anyone").WithTask("Anything.").Build();

        Assert.DoesNotContain("# Output format", prompt);
    }

    [Fact]
    public void WrapInput_EnclosesInputInTags()
    {
        Assert.Equal("<input>\nhello\n</input>", PromptBuilder.WrapInput("hello"));
    }
}
