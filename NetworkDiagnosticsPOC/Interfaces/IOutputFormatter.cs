using NetworkDiagnosticsPOC.Models;

namespace NetworkDiagnosticsPOC.Interfaces;

public interface IOutputFormatter
{
    void PrintResult(NetworkCheckResult result);

    void PrintResults(
        IEnumerable<NetworkCheckResult> results);
}