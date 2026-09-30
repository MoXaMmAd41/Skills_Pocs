# AI Developer Assistant

A console assistant that uses the OpenAI Responses API to help with everyday .NET work.

| Skill | Input | Output |
|---|---|---|
| Analyze an exception | Exception + stack trace (optionally code) | Root cause, exact frame, minimal fix, prevention |
| Generate documentation | C# code | XML doc comments, overview, usage example |
| Generate security tests | Controller/service code | OWASP-based findings + xUnit tests + fixes |
| Ask a question | Free text | Direct answer |

## Run

```powershell
$env:OPENAI_API_KEY = "sk-..."          # or: dotnet user-secrets set AI:ApiKey "sk-..."
dotnet run --project AIDeveloperAssistantPOC
```

Choose a skill, then either paste the input and finish with a line containing only `END`, or load a file:

```
@..\EmployeeManagementPOC\src\EmployeeManagement.Api\Controllers\V1\EmployeesController.cs
```

Ctrl+C cancels the current request, not the whole app.

## Configuration (`appsettings.json`)

| Key | Default | Purpose |
|---|---|---|
| `AI:Model` | `gpt-5` | Model name |
| `AI:ReasoningEffort` | `Medium` | `Low` / `Medium` / `High`: trade speed and cost for depth |
| `AI:MaxInputCharacters` | `100000` | Rejects oversized input before it is sent |

## Design

```
Program.cs                 Host, DI, options validation
Configuration/AIOptions    Validated settings
Interfaces/                IAIService (model access), IAssistantSkill (one per task)
Services/AIService         OpenAI streaming implementation
Services/PromptBuilder     Consistent role → task → rules → output-format prompts
Services/ConsoleMenuService, ConsoleInputReader   Console UX
Skills/                    One class per skill
```

To add a skill, implement `IAssistantSkill` and register it in `Program.cs`. It shows up in the menu automatically.

Input is sent wrapped in `<input>` tags and declared as data, so instructions hidden in pasted code or logs are not followed. Responses are not stored on the provider side (`StoredOutputEnabled = false`).

## Tests

```powershell
dotnet test AIDeveloperAssistantPOC.Tests
```

The tests replace OpenAI with a fake `IAIService`, so they need no API key or network. They cover prompt structure, every skill, console input (`END`, `@file`, size limit) and the menu flow, including error handling.
