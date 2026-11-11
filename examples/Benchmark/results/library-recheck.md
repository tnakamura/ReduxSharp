# Independent library and prototype measurements

The release library and the store-managed prototype from commit `7911228`
were each measured in a separate BenchmarkDotNet invocation on the same machine.
Times are means ± half of the reported 99.9% confidence interval in ns/op.
Change is relative to the prototype. FirstDispatch includes a cold entry build
and a struct dispatch, with a warm JIT.

| Case | Depth | Library (ns) | Prototype (ns) | Change | Bytes/op (library / prototype) |
| --- | ---: | ---: | ---: | ---: | ---: |
| ReduxSharpCached | 0 | 7.78 ± 0.02 | 7.91 ± 0.08 | -1.6% | 0 B / 0 B |
| ReduxSharpCached | 1 | 8.03 ± 0.03 | 7.83 ± 0.04 | +2.6% | 0 B / 0 B |
| ReduxSharpCached | 3 | 9.60 ± 0.02 | 9.58 ± 0.02 | +0.2% | 0 B / 0 B |
| ReduxSharpCached | 8 | 11.13 ± 0.04 | 11.32 ± 0.24 | -1.7% | 0 B / 0 B |
| ReduxSharpClass | 0 | 9.97 ± 0.04 | 9.91 ± 0.02 | +0.6% | 24 B / 24 B |
| ReduxSharpClass | 1 | 10.19 ± 0.02 | 9.95 ± 0.03 | +2.3% | 24 B / 24 B |
| ReduxSharpClass | 3 | 11.81 ± 0.03 | 11.80 ± 0.05 | +0.0% | 24 B / 24 B |
| ReduxSharpClass | 8 | 13.21 ± 0.04 | 13.49 ± 0.07 | -2.1% | 24 B / 24 B |
| ReduxSharpStruct | 0 | 7.83 ± 0.06 | 7.72 ± 0.01 | +1.4% | 0 B / 0 B |
| ReduxSharpStruct | 1 | 8.23 ± 0.03 | 8.03 ± 0.17 | +2.5% | 0 B / 0 B |
| ReduxSharpStruct | 3 | 10.34 ± 0.23 | 10.14 ± 0.09 | +2.0% | 0 B / 0 B |
| ReduxSharpStruct | 8 | 14.22 ± 0.09 | 14.22 ± 0.09 | -0.0% | 0 B / 0 B |
| FirstDispatch | 0 | 179.80 ± 3.08 | 170.80 ± 3.19 | +5.3% | 488 B / 488 B |
| FirstDispatch | 1 | 203.10 ± 4.06 | 200.00 ± 1.25 | +1.6% | 576 B / 576 B |
| FirstDispatch | 3 | 249.60 ± 4.10 | 248.60 ± 4.62 | +0.4% | 752 B / 752 B |
| FirstDispatch | 8 | 388.90 ± 7.35 | 525.80 ± 53.95 | -26.0% | 1192 B / 1192 B |
