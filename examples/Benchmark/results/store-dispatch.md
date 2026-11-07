```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.5.1 (25F80) [Darwin 25.5.0]
Unknown processor
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a
  Job-JMQMTU : .NET 10.0.5 (10.0.5, 10.0.526.15411), Arm64 RyuJIT armv8.0-a

Platform=Arm64

```
| Method      | Categories  | Depth | Mean      | Error     | StdDev    | Gen0   | Allocated |
|------------ |------------ |------ |----------:|----------:|----------:|-------:|----------:|
| **TypedCached** | **CachedClass** | **0**     |  **7.845 ns** | **0.0298 ns** | **0.0279 ns** |      **-** |         **-** |
| **TypedCached** | **CachedClass** | **1**     |  **7.785 ns** | **0.0195 ns** | **0.0172 ns** |      **-** |         **-** |
| **TypedCached** | **CachedClass** | **3**     |  **9.579 ns** | **0.0095 ns** | **0.0080 ns** |      **-** |         **-** |
| **TypedCached** | **CachedClass** | **8**     | **11.139 ns** | **0.1892 ns** | **0.1677 ns** |      **-** |         **-** |
|             |             |       |           |           |           |        |           |
| **TypedClass**  | **Class**       | **0**     |  **9.973 ns** | **0.0209 ns** | **0.0186 ns** | **0.0029** |      **24 B** |
| **TypedClass**  | **Class**       | **1**     |  **9.941 ns** | **0.0195 ns** | **0.0173 ns** | **0.0029** |      **24 B** |
| **TypedClass**  | **Class**       | **3**     | **12.022 ns** | **0.0060 ns** | **0.0056 ns** | **0.0029** |      **24 B** |
| **TypedClass**  | **Class**       | **8**     | **13.197 ns** | **0.0208 ns** | **0.0184 ns** | **0.0029** |      **24 B** |
|             |             |       |           |           |           |        |           |
| **TypedStruct** | **Struct**      | **0**     |  **8.015 ns** | **0.0046 ns** | **0.0036 ns** |      **-** |         **-** |
| **TypedStruct** | **Struct**      | **1**     |  **8.408 ns** | **0.0366 ns** | **0.0324 ns** |      **-** |         **-** |
| **TypedStruct** | **Struct**      | **3**     | **10.146 ns** | **0.0228 ns** | **0.0190 ns** |      **-** |         **-** |
| **TypedStruct** | **Struct**      | **8**     | **14.180 ns** | **0.0333 ns** | **0.0278 ns** |      **-** |         **-** |
