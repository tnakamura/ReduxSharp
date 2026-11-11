```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.5.1 (25F80) [Darwin 25.5.0]
Unknown processor
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  Job-YKWPNT : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a

Platform=Arm64  InvocationCount=1  UnrollFactor=1

```
| Method        | Depth | Mean     | Error   | StdDev  | Allocated |
|-------------- |------ |---------:|--------:|--------:|----------:|
| **FirstDispatch** | **0**     | **160.1 ns** | **0.32 ns** | **0.27 ns** |     **488 B** |
| **FirstDispatch** | **1**     | **186.2 ns** | **0.73 ns** | **0.72 ns** |     **576 B** |
| **FirstDispatch** | **3**     | **236.9 ns** | **4.63 ns** | **4.95 ns** |     **752 B** |
| **FirstDispatch** | **8**     | **343.8 ns** | **6.62 ns** | **7.36 ns** |    **1192 B** |
