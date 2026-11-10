```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.5.1 (25F80) [Darwin 25.5.0]
Unknown processor
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  Job-YKWPNT : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a

Platform=Arm64  InvocationCount=1  UnrollFactor=1

```
| Method        | Depth | Mean     | Error   | StdDev   | Allocated |
|-------------- |------ |---------:|--------:|---------:|----------:|
| **FirstDispatch** | **0**     | **179.7 ns** | **2.45 ns** |  **1.91 ns** |     **488 B** |
| **FirstDispatch** | **1**     | **197.0 ns** | **3.91 ns** |  **7.15 ns** |     **576 B** |
| **FirstDispatch** | **3**     | **244.3 ns** | **4.86 ns** | **11.16 ns** |     **752 B** |
| **FirstDispatch** | **8**     | **374.9 ns** | **7.46 ns** | **13.83 ns** |    **1192 B** |
