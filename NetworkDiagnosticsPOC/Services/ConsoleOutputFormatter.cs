using NetworkDiagnosticsPOC.Interfaces;
using NetworkDiagnosticsPOC.Models;

namespace NetworkDiagnosticsPOC.Services;

public sealed class ConsoleOutputFormatter : IOutputFormatter
{
    public void PrintResult(NetworkCheckResult result)
    {
        Console.WriteLine();
        Console.WriteLine("------------------------------------");
        Console.WriteLine($"{result.CheckType} RESULT");
        Console.WriteLine("------------------------------------");

        Console.WriteLine(
            $"Target        : {result.Target}");

        Console.WriteLine(
            $"Status        : {result.Status}");

        Console.WriteLine(
            $"Success       : {result.IsSuccess}");

        if (result.ResponseTimeMs.HasValue)
        {
            Console.WriteLine(
                $"Response Time : {result.ResponseTimeMs} ms");
        }

        if (!string.IsNullOrWhiteSpace(result.Details))
        {
            Console.WriteLine(
                $"Details       : {result.Details}");
        }

        if (result.Addresses.Count > 0)
        {
            Console.WriteLine("Addresses:");

            foreach (string address in result.Addresses)
            {
                Console.WriteLine($"  - {address}");
            }
        }

        Console.WriteLine("------------------------------------");
    }

    public void PrintResults(
        IEnumerable<NetworkCheckResult> results)
    {
        foreach (NetworkCheckResult result in results)
        {
            PrintResult(result);
        }
    }
}