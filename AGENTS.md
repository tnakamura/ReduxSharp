# Repository Guidelines

## Project Structure & Module Organization

- `src/ReduxSharp/` contains the .NET Standard 2.0 library: store, reducer interfaces, middleware factories, builder, and subscription extensions. Observer list implementation lives in `Internal/`.
- `test/ReduxSharp.Tests/` contains xUnit tests for store behavior and middleware pipelines.
- `examples/` contains the HelloWorld console app, Counter/RxCounter/TodoList WPF apps, and BenchmarkDotNet suites. WPF XAML and resources live alongside each example; benchmark reports live in `examples/Benchmark/results/`.
- `ReduxSharp.slnx` groups all projects; `README.md` documents the public API.

## Build, Test, and Development Commands

Run commands from the repository root:

- `dotnet build src/ReduxSharp/ReduxSharp.csproj -c Release` builds the library and generates NuGet packages.
- `dotnet test test/ReduxSharp.Tests/ReduxSharp.Tests.csproj -c Release` runs the .NET 7 test project. With only a newer runtime installed, use the documented `DOTNET_ROLL_FORWARD=Major` prefix.
- `dotnet run --project examples/HelloWorld` runs the console example; its target is .NET Core 3.1.
- `dotnet run -c Release --project examples/Benchmark -- --filter '*Middleware*' '*EntryCost*'` runs middleware and first-dispatch benchmarks with .NET 10.

Use Windows with Visual Studio/MSBuild and the appropriate .NET Framework targeting packs for WPF examples. Prefer individual project builds on other platforms.

## Coding Style & Naming Conventions

Use four-space indentation, braces on separate lines, and block-scoped namespaces. Match existing PascalCase type/method/property names and camelCase parameters, locals, and private fields. Prefix interfaces with `I` and generic type parameters with `T` (for example, `IStore<TState>`).

Keep library code compatible with C# 8 and nullable reference types. Document public APIs with XML comments. No repository-specific formatter or lint configuration is present; follow neighboring code.

## Testing Guidelines

Use xUnit `[Fact]` methods with descriptive PascalCase names, such as `FailedBuildCanBeRetriedWithoutCachingAPartialChain`. Add regression coverage for changed behavior, especially middleware order/caching, exception propagation, and subscription disposal. No numeric coverage threshold is configured. For performance changes, follow `examples/Benchmark/README.md` and report timings and allocations.

## Commit & Pull Request Guidelines

Write commit messages in English.

Recent commits use short, imperative summaries such as `Route dispatch through cached middleware pipelines`; no fixed prefix convention is evident. Keep commits focused. PRs should explain the change, link relevant issues, list validation commands/results, and update documentation/examples when public APIs change. Include screenshots for WPF visual changes.
