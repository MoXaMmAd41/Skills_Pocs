namespace DataStructuresAlgorithms.Playground;

/// <summary>
/// A small REPL for one data structure: read a command, run it, then redraw the structure's state.
/// </summary>
internal abstract class CommandDemo
{
    public abstract string Title { get; }

    /// <summary>One line per command, e.g. "push &lt;n&gt;...   add numbers on top".</summary>
    protected abstract IReadOnlyList<string> Commands { get; }

    protected abstract void Execute(string command, string[] args);

    protected abstract void Render();

    /// <summary>Queries can opt out of redrawing, so their result isn't buried under an unchanged picture.</summary>
    protected virtual bool ChangesState(string command) => true;

    public void Run()
    {
        Ui.Heading(Title);
        PrintHelp();
        Render();

        while (true)
        {
            Console.Write("\n> ");
            string? line = Console.ReadLine();

            if (line is null)
            {
                return;
            }

            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length == 0)
            {
                continue;
            }

            string command = parts[0].ToLowerInvariant();

            switch (command)
            {
                case "back" or "exit" or "q":
                    return;
                case "help" or "?":
                    PrintHelp();
                    continue;
            }

            try
            {
                Execute(command, parts[1..]);

                if (ChangesState(command))
                {
                    Render();
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException
                                           or ArgumentException or FormatException)
            {
                Ui.Error(ex.Message);
            }
        }
    }

    protected static int Number(string[] args, int index = 0) =>
        args.Length > index && int.TryParse(args[index], out int value)
            ? value
            : throw new FormatException("Expected a whole number. Type 'help' for usage.");

    protected static int[] Numbers(string[] args) =>
        args.Length == 0
            ? throw new FormatException("Expected one or more numbers, e.g. 5 3 8.")
            : args.Select((_, i) => Number(args, i)).ToArray();

    protected static string Word(string[] args, int index = 0) =>
        args.Length > index ? args[index] : throw new FormatException("Missing argument. Type 'help' for usage.");

    protected static void UnknownCommand(string command) =>
        throw new FormatException($"Unknown command '{command}'. Type 'help' to see the commands.");

    private void PrintHelp()
    {
        Ui.Muted("Commands:");

        foreach (string line in Commands)
        {
            Ui.Muted($"  {line}");
        }

        Ui.Muted("  help                  show this list");
        Ui.Muted("  back                  return to the main menu");
    }
}
