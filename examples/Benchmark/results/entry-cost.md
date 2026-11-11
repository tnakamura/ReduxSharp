```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.5.1 (25F80) [Darwin 25.5.0]
Unknown processor
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  Job-JMQMTU : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  Job-YKWPNT : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a

Platform=Arm64

```
| Method            | Job        | InvocationCount | UnrollFactor | Depth | Mean       | Error     | StdDev    | Median     | Allocated |
|------------------ |----------- |---------------- |------------- |------ |-----------:|----------:|----------:|-----------:|----------:|
| **LookupAndDispatch** | **Job-JMQMTU** | **Default**         | **16**           | **0**     |   **9.797 ns** | **0.2054 ns** | **0.1921 ns** |   **9.869 ns** |         **-** |
| FirstFor          | Job-YKWPNT | 1               | 1            | 0     | 161.761 ns | 1.3285 ns | 1.1776 ns | 162.029 ns |     488 B |
| **LookupAndDispatch** | **Job-JMQMTU** | **Default**         | **16**           | **1**     |   **9.604 ns** | **0.0988 ns** | **0.0825 ns** |   **9.580 ns** |         **-** |
| FirstFor          | Job-YKWPNT | 1               | 1            | 1     | 184.947 ns | 3.2405 ns | 6.0066 ns | 181.315 ns |     576 B |
| **LookupAndDispatch** | **Job-JMQMTU** | **Default**         | **16**           | **3**     |  **11.933 ns** | **0.1681 ns** | **0.1573 ns** |  **11.995 ns** |         **-** |
| FirstFor          | Job-YKWPNT | 1               | 1            | 3     | 231.155 ns | 2.4508 ns | 2.4070 ns | 231.994 ns |     752 B |
| **LookupAndDispatch** | **Job-JMQMTU** | **Default**         | **16**           | **8**     |  **14.352 ns** | **0.1197 ns** | **0.0999 ns** |  **14.341 ns** |         **-** |
| FirstFor          | Job-YKWPNT | 1               | 1            | 8     | 346.546 ns | 5.5343 ns | 5.1768 ns | 347.533 ns |    1192 B |
