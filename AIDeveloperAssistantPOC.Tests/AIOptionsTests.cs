using System.ComponentModel.DataAnnotations;
using AIDeveloperAssistantPOC.Configuration;

namespace AIDeveloperAssistantPOC.Tests;

public sealed class AIOptionsTests
{
    [Fact]
    public void Validation_Fails_WhenApiKeyIsMissing()
    {
        List<ValidationResult> errors = Validate(new AIOptions());

        ValidationResult error = Assert.Single(errors);
        Assert.Contains("OPENAI_API_KEY", error.ErrorMessage);
    }

    [Theory]
    [InlineData(999)]
    [InlineData(1_000_001)]
    public void Validation_Fails_WhenInputLimitIsOutOfRange(int maxCharacters)
    {
        List<ValidationResult> errors = Validate(new AIOptions { ApiKey = "key", MaxInputCharacters = maxCharacters });

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(AIOptions.MaxInputCharacters)));
    }

    [Fact]
    public void Validation_Passes_WithDefaultsAndAKey()
    {
        Assert.Empty(Validate(new AIOptions { ApiKey = "key" }));
    }

    private static List<ValidationResult> Validate(AIOptions options)
    {
        List<ValidationResult> results = [];
        Validator.TryValidateObject(options, new ValidationContext(options), results, validateAllProperties: true);
        return results;
    }
}
