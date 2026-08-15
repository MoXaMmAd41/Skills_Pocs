using NetworkDiagnosticsPOC.Models;

namespace NetworkDiagnosticsPOC.Services;

public sealed class ConsoleMenuService
{
    public void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("====================================");
        Console.WriteLine("      Network Diagnostics Tool");
        Console.WriteLine("====================================");
        Console.WriteLine("1. Ping Host");
        Console.WriteLine("2. DNS Lookup");
        Console.WriteLine("3. HTTP Check");
        Console.WriteLine("4. Run All Checks");
        Console.WriteLine("5. Exit");
        Console.WriteLine("====================================");
    }

    public string ReadOption()
    {
        Console.Write("Select option: ");

        return Console.ReadLine()?.Trim()
               ?? string.Empty;
    }

    public NetworkCheckType? ParseCheckType(string option)
    {
        return option switch
        {
            "1" => NetworkCheckType.Ping,
            "2" => NetworkCheckType.Dns,
            "3" => NetworkCheckType.Http,
            _ => null
        };
    }

    public string ReadTarget(NetworkCheckType checkType)
    {
        string label = checkType switch
        {
            NetworkCheckType.Ping => "host",
            NetworkCheckType.Dns => "domain",
            NetworkCheckType.Http => "URL",
            _ => "target"
        };

        Console.Write($"Enter {label}: ");

        return Console.ReadLine()?.Trim()
               ?? string.Empty;
    }

    public string ReadAllChecksTarget()
    {
        Console.Write("Enter host/domain/URL: ");

        return Console.ReadLine()?.Trim()
               ?? string.Empty;
    }
}