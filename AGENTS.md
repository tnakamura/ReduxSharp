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

Follow the Microsoft Learn [C# identifier naming rules and conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names) and [common C# code conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions). Use the repository `.editorconfig` for formatting and naming checks.

Use four-space indentation and Allman braces. Use PascalCase for types, methods, properties, events, and constants; camelCase for parameters and locals; `_camelCase` for private/internal instance fields; and `s_camelCase` for private/internal static fields (`t_camelCase` for thread-static fields). Prefix interfaces with `I` and descriptive generic type parameters with `T`. Use descriptive names, explicit types when the initializer does not make the type apparent, and braces around control-flow bodies. Document public APIs with XML comments.

Keep the library compatible with C# 8, .NET Standard 2.0, and nullable reference types. Keep the older console and WPF examples compatible with their existing language versions; use block-scoped namespaces in these projects and the library. Use file-scoped namespaces in the tests and .NET 10 benchmarks. Do not adopt language features or APIs unsupported by a project's target. Preserve the existing public `AsyncActionCreator<TState>` delegate for API compatibility; prefer `Func<>` and `Action<>` for new delegates. Leave designer-generated code to its generator. The article's 65-character line guidance is intended for documentation samples; wrap repository source where it improves readability.

Validate supported projects with `dotnet format <project> --no-restore --verify-no-changes` in addition to builds and tests. WPF projects require the Windows tooling described above.

## Testing Guidelines

Use xUnit `[Fact]` methods with descriptive PascalCase names, such as `FailedBuildCanBeRetriedWithoutCachingAPartialChain`. Add regression coverage for changed behavior, especially middleware order/caching, exception propagation, and subscription disposal. No numeric coverage threshold is configured. For performance changes, follow `examples/Benchmark/README.md` and report timings and allocations.

## Commit & Pull Request Guidelines

Write commit messages in English.

Recent commits use short, imperative summaries such as `Route dispatch through cached middleware pipelines`; no fixed prefix convention is evident. Keep commits focused. PRs should explain the change, link relevant issues, list validation commands/results, and update documentation/examples when public APIs change. Include screenshots for WPF visual changes.
