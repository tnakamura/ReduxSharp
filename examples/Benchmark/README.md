# ReduxSharp middleware benchmarks

A StoreBuilder registers middleware with Use. Build snapshots the factories and
creates the store. Dispatch builds a typed pipeline on the first call for an
action type and reuses it on subsequent calls.

```csharp
using Benchmark;
using Benchmark.Examples;
using ReduxSharp;

var store = new StoreBuilder<AppState>(new AppState())
    .UseLoggingMiddleware()
    .Build(new AppReducer());

store.Dispatch(new StructIncrementAction());
store.Dispatch(new ClassIncrementAction());
```

Extension methods can accept and return `IStoreBuilder<TState>` and register a
factory through its `Use` method. `Build` returns the concrete `Store<TState>`.
The [logging extension](Examples/LoggingMiddlewareExtensions.cs) writes action
names to the console by default, or accepts a callback such as `log.Add`.

Each entry shares the store's state, reducer lock and subscriptions. The reducer
updates state under the lock and the store notifies subscribers after releasing
it. Middleware can stop, call next repeatedly, or dispatch another action through
the store. Calling next continues with the current action type. Dispatching a
different type starts its chain at the beginning.

The cache lives as long as its store. A new action type incurs one build and a
copy of the type dictionary. The published dictionary supports concurrent reads.
Dispatch looks up the delegate in this store-local dictionary.

## Reproduce

Run from the repository root on Arm64 with the .NET 10 SDK. The benchmark project
uses BenchmarkDotNet 0.15.8 and Redux.NET 2.0.0.

```sh
DOTNET_ROLL_FORWARD=Major dotnet test -c Release test/ReduxSharp.Tests
dotnet run -c Release --project examples/Benchmark -- --filter '*Middleware*' '*EntryCost*' --artifacts artifacts/typed-middleware
```

The main matrix measures dispatch through each library's normal entry point.
ReduxSharp calls store.Dispatch(action), including the cache lookup. Each pass-through factory returns a wrapper
that calls next. Both stores use the existing no-op benchmark reducer with
no subscribers. The fresh-class cases include construction of the action, and
the cached-class cases reuse one instance. Constructors and middleware
registration run outside the timed methods.

EntryCost measures the first struct Dispatch, including pipeline construction and
reduction, on batches of 1,024 fresh stores prepared outside the timed region.
The JIT is already warm. The main matrix measures steady-state dispatch.

For the adoption decision, compare means, BenchmarkDotNet's reported errors and
bytes allocated per operation. Repeat comparisons with overlapping error
intervals or means within 5% in an independent invocation. Treat an ordering
that changes between runs as inconclusive.

## Library results on 2026-11-10

On macOS 26.5.1 Arm64 (.NET 10.0.5, SDK 10.0.201), ReduxSharp had a lower
mean than Redux.NET 2.0.0 across all 12 steady-state comparisons. The reduction
in time per dispatch ranged from 16% to 40%. The prototype column comes from
the November 8 store-managed experiment.

| Action | Stages | Library (ns) | Redux.NET (ns) | Prototype (ns) | Library bytes/op |
| --- | ---: | ---: | ---: | ---: | ---: |
| CachedClass | 0 | 7.77 | 11.08 | 7.84 | 0 B |
| CachedClass | 1 | 8.01 | 11.19 | 7.79 | 0 B |
| CachedClass | 3 | 9.60 | 12.80 | 9.58 | 0 B |
| CachedClass | 8 | 11.16 | 14.63 | 11.14 | 0 B |
| Class | 0 | 9.96 | 12.89 | 9.97 | 24 B |
| Class | 1 | 10.22 | 13.14 | 9.94 | 24 B |
| Class | 3 | 11.85 | 15.01 | 12.02 | 24 B |
| Class | 8 | 13.25 | 16.95 | 13.20 | 24 B |
| Struct | 0 | 7.72 | 12.85 | 8.02 | 0 B |
| Struct | 1 | 8.09 | 13.13 | 8.41 | 0 B |
| Struct | 3 | 10.15 | 15.20 | 10.15 | 0 B |
| Struct | 8 | 14.15 | 16.85 | 14.18 | 0 B |

The first struct dispatch took about 180, 197, 244 and 375 ns at 0, 1, 3 and
8 stages respectively. It allocated 488, 576, 752 and 1,192 B per fresh store.
All steady-state struct and cached-class cases allocated 0 B per dispatch.
[Dispatch results](results/library-dispatch.md) and
[first-dispatch results](results/library-first-dispatch.md) include measurement errors.

We independently reran all 16 library cases and rebuilt the prototype from
commit `7911228` for a separate run. Relative to that prototype, steady-state
library means ranged from 2.1% shorter to 2.6% longer, with identical
allocations. The first dispatch at zero stages took 179.8 ns in the library
and 170.8 ns in the prototype (5.3% longer). Cold timings use short batches;
the eight-stage prototype result varied widely and the ordering reversed
between comparisons. See the [independent measurements](results/library-recheck.md).

## Historical prototype with store-managed dispatch on 2026-11-08

The timed call is `typed.Dispatch(action)` through the concrete store returned
by Build. Each steady-state case repeats one action type. These 12 means and the
four first-dispatch cases were measured on the same Arm64 host with .NET 10.0.5.

| Action | 0 stages | 1 stage | 3 stages | 8 stages | Bytes/op |
| --- | ---: | ---: | ---: | ---: | ---: |
| Struct | 8.015 ns | 8.408 ns | 10.146 ns | 14.180 ns | 0 B |
| Class | 9.973 ns | 9.941 ns | 12.022 ns | 13.197 ns | 24 B |
| CachedClass | 7.845 ns | 7.785 ns | 9.579 ns | 11.139 ns | 0 B |

A fresh store's first struct dispatch through one stage took about 186 ns and
allocated 576 B to construct and cache the pipeline. Later struct dispatches
allocated 0 B. For comparison, the previous run's caller-held delegate took
5.34 ns for a one-stage struct dispatch. The November 7 results below provide
the separately measured ReduxSharp and Redux.NET controls.

See the [steady-state results](results/store-dispatch.md) and
[first-dispatch results](results/first-dispatch.md) for means and errors.

## Historical experiment on 2026-11-07: caller-held delegates

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

These measurements describe the earlier API which exposed typed entries. Existing reducers work directly. Middleware needs a
factory per stage, and cross-type dispatch restarts at the head. These figures
cover pass-through middleware with a no-op reducer on this runtime.

See the [dispatch measurements](results/dispatch.md) and
[entry-cost measurements](results/entry-cost.md) for the reported errors,
standard deviations and managed allocations.
