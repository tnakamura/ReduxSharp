using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace Benchmark;

/// <summary>Configures the benchmark platform and allocation measurements.</summary>
public class BenchmarkConfig : ManualConfig
{
    /// <summary>Initializes a new instance of this type.</summary>
    public BenchmarkConfig()
    {
        AddJob(Job.Default.WithPlatform(Platform.Arm64));
        AddDiagnoser(MemoryDiagnoser.Default);
    }
}
