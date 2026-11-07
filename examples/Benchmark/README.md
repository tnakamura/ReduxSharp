# Typed middleware prototype

A StoreBuilder registers middleware factories with Use and snapshots the list when
Run creates a store. On the first For<TAction>() for that store, the store binds
the reducer method to a typed delegate and wraps it with the factories in reverse
registration order. Save the returned Action<TAction> when dispatching that type
repeatedly.

```csharp
var store = new StoreBuilder<AppState>(new AppState())
    .Use(new PassThrough())
    .Run(new AppReducer());
var increment = store.For<StructIncrementAction>();
increment(new StructIncrementAction());
```

Each entry shares the store's state, reducer lock and subscriptions. The reducer
updates state under the lock and the store notifies subscribers after releasing
it. Middleware can stop, call next repeatedly, or dispatch another action through
the store. Calling next continues with the current action type. Dispatching a
different type starts its chain at the beginning.

The cache lives as long as its store. A new action type incurs one build and a
copy of the type dictionary. The published dictionary supports concurrent reads.
Holding an entry keeps its store alive and avoids subsequent cache lookups.

## Reproduce

Run from the repository root on Arm64 with the .NET 10 SDK. The benchmark project
uses BenchmarkDotNet 0.15.8 and Redux.NET 2.0.0.

```sh
dotnet run -c Release --project examples/Benchmark -- --verify
dotnet run -c Release --project examples/Benchmark -- --filter '*Middleware*' '*EntryCost*' --artifacts artifacts/typed-middleware
```

The main matrix measures dispatch through each library's normal entry point.
The prototype uses held delegates. Each pass-through factory returns a wrapper
that calls next. All three stores use the existing no-op benchmark reducer with
no subscribers. The fresh-class cases include construction of the action, and
the cached-class cases reuse one instance. Constructors and middleware
registration run outside the timed methods.

EntryCost measures warm lookup plus struct dispatch, and separately the first
For<StructIncrementAction>() on fresh stores. The latter measures batches of
1,024 stores prepared outside the timed region with the JIT already warm.
Build-time and steady-state allocations are reported separately.

For the adoption decision, compare means, BenchmarkDotNet's reported errors and
bytes allocated per operation. Repeat comparisons with overlapping error
intervals or means within 5% in an independent invocation. Treat an ordering
that changes between runs as inconclusive.

## Results on 2026-11-07

On macOS 26.5.1 Arm64 with .NET 10.0.5 (SDK 10.0.201), the held typed entry
had the lowest mean in all 12 action/depth combinations. The smallest lead over
the faster of the other two stores was 32% (8-stage struct). No comparison met
the independent recheck criterion. All runs used normal process priority.

| Action | Depth | Typed (ns) | ReduxSharp (ns) | Redux.NET (ns) | Typed bytes/op |
| --- | ---: | ---: | ---: | ---: | ---: |
| Struct | 0 | 5.33 | 9.58 | 12.89 | 0 B |
| Struct | 1 | 5.34 | 14.91 | 13.14 | 0 B |
| Struct | 3 | 7.19 | 25.52 | 15.86 | 0 B |
| Struct | 8 | 11.39 | 63.70 | 16.87 | 0 B |
| Class | 0 | 7.29 | 12.64 | 13.08 | 24 B |
| Class | 1 | 7.56 | 17.94 | 13.13 | 24 B |
| Class | 3 | 8.44 | 29.05 | 15.57 | 24 B |
| Class | 8 | 10.24 | 73.98 | 17.39 | 24 B |
| CachedClass | 0 | 5.31 | 10.08 | 11.16 | 0 B |
| CachedClass | 1 | 5.31 | 15.67 | 11.17 | 0 B |
| CachedClass | 3 | 6.10 | 26.52 | 12.74 | 0 B |
| CachedClass | 8 | 8.02 | 66.25 | 14.58 | 0 B |

For a one-stage struct pipeline, obtaining the first entry took 184.95 ns
and 576 B. Subsequent lookup plus dispatch took 9.60 ns and 0 B. Dispatch
through the held entry took 5.34 ns and 0 B. At eight stages the corresponding
times were 346.55 ns, 14.35 ns and 11.39 ns. The first-entry timings use
short setup-isolated batches and are approximate.

The results support moving this API toward the library for callers that can
retain typed entries. Existing reducers work directly. Middleware needs a
factory per stage, and cross-type dispatch restarts at the head. These figures
cover pass-through middleware with a no-op reducer on this runtime.

See the [dispatch measurements](results/dispatch.md) and
[entry-cost measurements](results/entry-cost.md) for the reported errors,
standard deviations and managed allocations.
