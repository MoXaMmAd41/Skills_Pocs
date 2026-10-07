using AIDeveloperAssistantPOC.Configuration;
using AIDeveloperAssistantPOC.Interfaces;
using AIDeveloperAssistantPOC.Services;
using AIDeveloperAssistantPOC.Skills;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Console apps run as "Production" by default, where the host skips user secrets; load them explicitly.
builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Services
    .AddOptions<AIOptions>()
    .Bind(builder.Configuration.GetSection(AIOptions.SectionName))
    // Keep the conventional OpenAI environment variable working alongside configuration/user secrets.
    .PostConfigure(options =>
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            options.ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty;
        }
    })
    .ValidateDataAnnotations();

builder.Services.AddSingleton<IAIService, AIService>();
builder.Services.AddSingleton<ConsoleInputReader>();
builder.Services.AddSingleton<ConsoleMenuService>();

// Menu order follows registration order.
builder.Services.AddSingleton<IAssistantSkill, ExceptionAnalyzer>();
builder.Services.AddSingleton<IAssistantSkill, DocumentationGenerator>();
builder.Services.AddSingleton<IAssistantSkill, SecurityTestGenerator>();
builder.Services.AddSingleton<IAssistantSkill, GeneralAssistant>();

using IHost host = builder.Build();

try
{
    await host.Services.GetRequiredService<ConsoleMenuService>().RunAsync();
}
catch (OptionsValidationException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(string.Join(Environment.NewLine, ex.Failures));
    Console.ResetColor();
    Environment.ExitCode = 1;
}
