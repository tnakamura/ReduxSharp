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
| **ReduxNetCached**   | **CachedClass** | **0**     | **11.158 ns** | **0.0226 ns** | **0.0211 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| ReduxSharpCached | CachedClass | 0     | 10.082 ns | 0.0149 ns | 0.0140 ns |  0.90 |    0.00 |      - |         - |          NA |
| TypedCached      | CachedClass | 0     |  5.313 ns | 0.0092 ns | 0.0086 ns |  0.48 |    0.00 |      - |         - |          NA |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetCached**   | **CachedClass** | **1**     | **11.166 ns** | **0.0390 ns** | **0.0365 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| ReduxSharpCached | CachedClass | 1     | 15.667 ns | 0.0247 ns | 0.0231 ns |  1.40 |    0.00 |      - |         - |          NA |
| TypedCached      | CachedClass | 1     |  5.307 ns | 0.0082 ns | 0.0073 ns |  0.48 |    0.00 |      - |         - |          NA |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetCached**   | **CachedClass** | **3**     | **12.741 ns** | **0.0235 ns** | **0.0219 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| ReduxSharpCached | CachedClass | 3     | 26.523 ns | 0.0253 ns | 0.0197 ns |  2.08 |    0.00 |      - |         - |          NA |
| TypedCached      | CachedClass | 3     |  6.097 ns | 0.0038 ns | 0.0034 ns |  0.48 |    0.00 |      - |         - |          NA |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetCached**   | **CachedClass** | **8**     | **14.584 ns** | **0.0145 ns** | **0.0128 ns** |  **1.00** |    **0.00** |      **-** |         **-** |          **NA** |
| ReduxSharpCached | CachedClass | 8     | 66.246 ns | 1.2271 ns | 1.0878 ns |  4.54 |    0.07 |      - |         - |          NA |
| TypedCached      | CachedClass | 8     |  8.022 ns | 0.0099 ns | 0.0088 ns |  0.55 |    0.00 |      - |         - |          NA |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetClass**    | **Class**       | **0**     | **13.080 ns** | **0.0201 ns** | **0.0178 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpClass  | Class       | 0     | 12.640 ns | 0.2012 ns | 0.1680 ns |  0.97 |    0.01 | 0.0029 |      24 B |        1.00 |
| TypedClass       | Class       | 0     |  7.285 ns | 0.0151 ns | 0.0134 ns |  0.56 |    0.00 | 0.0029 |      24 B |        1.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetClass**    | **Class**       | **1**     | **13.133 ns** | **0.0205 ns** | **0.0182 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpClass  | Class       | 1     | 17.944 ns | 0.0519 ns | 0.0433 ns |  1.37 |    0.00 | 0.0029 |      24 B |        1.00 |
| TypedClass       | Class       | 1     |  7.560 ns | 0.0154 ns | 0.0144 ns |  0.58 |    0.00 | 0.0029 |      24 B |        1.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetClass**    | **Class**       | **3**     | **15.568 ns** | **0.0254 ns** | **0.0238 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpClass  | Class       | 3     | 29.051 ns | 0.0670 ns | 0.0594 ns |  1.87 |    0.00 | 0.0029 |      24 B |        1.00 |
| TypedClass       | Class       | 3     |  8.437 ns | 0.0151 ns | 0.0141 ns |  0.54 |    0.00 | 0.0029 |      24 B |        1.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetClass**    | **Class**       | **8**     | **17.389 ns** | **0.0301 ns** | **0.0235 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpClass  | Class       | 8     | 73.978 ns | 0.6384 ns | 0.5972 ns |  4.25 |    0.03 | 0.0029 |      24 B |        1.00 |
| TypedClass       | Class       | 8     | 10.241 ns | 0.0145 ns | 0.0136 ns |  0.59 |    0.00 | 0.0029 |      24 B |        1.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetStruct**   | **Struct**      | **0**     | **12.889 ns** | **0.0491 ns** | **0.0410 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpStruct | Struct      | 0     |  9.583 ns | 0.0058 ns | 0.0048 ns |  0.74 |    0.00 |      - |         - |        0.00 |
| TypedStruct      | Struct      | 0     |  5.327 ns | 0.0059 ns | 0.0056 ns |  0.41 |    0.00 |      - |         - |        0.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetStruct**   | **Struct**      | **1**     | **13.145 ns** | **0.0423 ns** | **0.0330 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpStruct | Struct      | 1     | 14.908 ns | 0.0196 ns | 0.0174 ns |  1.13 |    0.00 |      - |         - |        0.00 |
| TypedStruct      | Struct      | 1     |  5.341 ns | 0.0394 ns | 0.0329 ns |  0.41 |    0.00 |      - |         - |        0.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetStruct**   | **Struct**      | **3**     | **15.855 ns** | **0.0309 ns** | **0.0274 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpStruct | Struct      | 3     | 25.524 ns | 0.0147 ns | 0.0123 ns |  1.61 |    0.00 |      - |         - |        0.00 |
| TypedStruct      | Struct      | 3     |  7.192 ns | 0.0180 ns | 0.0140 ns |  0.45 |    0.00 |      - |         - |        0.00 |
|                  |             |       |           |           |           |       |         |        |           |             |
| **ReduxNetStruct**   | **Struct**      | **8**     | **16.867 ns** | **0.0250 ns** | **0.0209 ns** |  **1.00** |    **0.00** | **0.0029** |      **24 B** |        **1.00** |
| ReduxSharpStruct | Struct      | 8     | 63.700 ns | 0.7753 ns | 0.7252 ns |  3.78 |    0.04 |      - |         - |        0.00 |
| TypedStruct      | Struct      | 8     | 11.389 ns | 0.0268 ns | 0.0251 ns |  0.68 |    0.00 |      - |         - |        0.00 |
