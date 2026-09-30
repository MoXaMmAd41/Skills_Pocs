using AIDeveloperAssistantPOC.Interfaces;
using AIDeveloperAssistantPOC.Models;
using AIDeveloperAssistantPOC.Skills;

namespace AIDeveloperAssistantPOC.Tests;

public sealed class SkillTests
{
    public static TheoryData<IAssistantSkill> AllSkills =>
    [
        new ExceptionAnalyzer(),
        new DocumentationGenerator(),
        new SecurityTestGenerator(),
        new GeneralAssistant()
    ];

    [Theory]
    [MemberData(nameof(AllSkills))]
    public void CreateRequest_PassesInputThroughUnchanged(IAssistantSkill skill)
    {
        AIRequest request = skill.CreateRequest("some input");

        Assert.Equal("some input", request.Input);
        Assert.False(string.IsNullOrWhiteSpace(request.Instructions));
    }

    [Theory]
    [MemberData(nameof(AllSkills))]
    public void EverySkill_HasTitleAndInputHint(IAssistantSkill skill)
    {
        Assert.False(string.IsNullOrWhiteSpace(skill.Title));
        Assert.False(string.IsNullOrWhiteSpace(skill.InputHint));
    }

    [Fact]
    public void SkillTitles_AreUnique()
    {
        List<string> titles = AllSkills.Select(row => row.Data.Title).ToList();

        Assert.Equal(titles.Count, titles.Distinct().Count());
    }

    [Theory]
    [InlineData(typeof(ExceptionAnalyzer), "Root cause")]
    [InlineData(typeof(DocumentationGenerator), "Documented code")]
    [InlineData(typeof(SecurityTestGenerator), "Security tests")]
    public void Skill_AsksForItsSpecificSections(Type skillType, string expectedSection)
    {
        IAssistantSkill skill = (IAssistantSkill)Activator.CreateInstance(skillType)!;

        Assert.Contains($"## {expectedSection}", skill.CreateRequest("x").Instructions);
    }
}
