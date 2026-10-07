using BenchmarkDotNet.Running;

// Run all:        dotnet run -c Release --project DataStructuresAlgorithmsPOC.Benchmarks -- --filter *
// Run one group:  dotnet run -c Release --project DataStructuresAlgorithmsPOC.Benchmarks -- --filter *Sorting*
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
