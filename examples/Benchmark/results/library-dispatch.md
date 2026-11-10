```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.5.1 (25F80) [Darwin 25.5.0]
Unknown processor
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  Job-JMQMTU : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a

Platform=Arm64

```
| Method           | Categories  | Depth | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|----------------- |------------ |------ |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **ReduxNetCached**   | **CachedClass** | **0**     | **11.079 ns** | **0.1067 ns** | **0.0833 ns** |  **1.00** |    **0.01** |      **-** |         **-** |          **NA** |
| ReduxSharpCached | CachedClass | 0     |  7.770 ns | 0.0259 ns | 0.0230 ns |  0.70 |    0.01 |      - |         - |          NA |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetCached**   | **CachedClass** | **1**     | **11.189 ns** | **0.0115 ns** | **0.0102 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| ReduxSharpCached | CachedClass | 1     |  8.012 ns | 0.0055 ns | 0.0048 ns |  0.72 |    0.00 |      - |         - |          NA |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetCached**   | **CachedClass** | **3**     | **12.800 ns** | **0.0484 ns** | **0.0404 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| ReduxSharpCached | CachedClass | 3     |  9.603 ns | 0.0135 ns | 0.0113 ns |  0.75 |    0.00 |      - |         - |          NA |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetCached**   | **CachedClass** | **8**     | **14.635 ns** | **0.0187 ns** | **0.0166 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| ReduxSharpCached | CachedClass | 8     | 11.162 ns | 0.0578 ns | 0.0512 ns |  0.76 |    0.00 |      - |         - |          NA |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetClass**    | **Class**       | **0**     | **12.893 ns** | **0.0385 ns** | **0.0342 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpClass  | Class       | 0     |  9.956 ns | 0.0153 ns | 0.0143 ns |  0.77 |    0.00 | 0.0029 |      24 B |        1.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetClass**    | **Class**       | **1**     | **13.136 ns** | **0.0222 ns** | **0.0197 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpClass  | Class       | 1     | 10.224 ns | 0.0138 ns | 0.0115 ns |  0.78 |    0.00 | 0.0029 |      24 B |        1.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetClass**    | **Class**       | **3**     | **15.012 ns** | **0.0234 ns** | **0.0208 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpClass  | Class       | 3     | 11.845 ns | 0.0458 ns | 0.0383 ns |  0.79 |    0.00 | 0.0029 |      24 B |        1.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetClass**    | **Class**       | **8**     | **16.951 ns** | **0.2715 ns** | **0.2267 ns** |  **1.00** |    **0.02** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpClass  | Class       | 8     | 13.246 ns | 0.0475 ns | 0.0371 ns |  0.78 |    0.01 | 0.0029 |      24 B |        1.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetStruct**   | **Struct**      | **0**     | **12.845 ns** | **0.0226 ns** | **0.0200 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpStruct | Struct      | 0     |  7.724 ns | 0.0131 ns | 0.0116 ns |  0.60 |    0.00 |      - |         - |        0.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetStruct**   | **Struct**      | **1**     | **13.132 ns** | **0.0216 ns** | **0.0180 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpStruct | Struct      | 1     |  8.089 ns | 0.0426 ns | 0.0398 ns |  0.62 |    0.00 |      - |         - |        0.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetStruct**   | **Struct**      | **3**     | **15.197 ns** | **0.2860 ns** | **0.2535 ns** |  **1.00** |    **0.02** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpStruct | Struct      | 3     | 10.146 ns | 0.0245 ns | 0.0191 ns |  0.67 |    0.01 |      - |         - |        0.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetStruct**   | **Struct**      | **8**     | **16.847 ns** | **0.0432 ns** | **0.0361 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpStruct | Struct      | 8     | 14.154 ns | 0.0577 ns | 0.0482 ns |  0.84 |    0.00 |      - |         - |        0.00 |
