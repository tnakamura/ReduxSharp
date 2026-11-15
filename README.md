# ReduxSharp

Unidirectional Data Flow in C# - Inspired by Redux

[![NuGet](https://img.shields.io/nuget/v/ReduxSharp.svg?maxAge=3600)](https://www.nuget.org/packages/ReduxSharp/)


## About

There is a library in JavaScript called Redux. 
If you have ever developed a Single Page Application with React, you may have used it.
For more information on Redux, please read the official documentation.

ReduxSharp is a port of Redux to C#.


## Installation

First, [install Nuget](http://docs.nuget.org/docs/start-here/installing-nuget).
Then, install [ReduxSharp](http://www.nuget.org/packages/ReduxSharp) from the package manager console:

```
PM> Install-Package ReduxSharp
```


## Quick-start

### State

Define a class that represents a state of the app.

```cs
namespace HelloWorld
{
    public class AppState
    {
        public AppState(int count) => Count = count;

        public int Count { get; }
    }
}
```

### Actions

Define actions.
Actions are payloads of information that send data from your application to your store.

```cs
namespace HelloWorld
{
    public readonly struct Increment
    {
    }

    public readonly struct Decrement
    {
    }
}
```

### Reducers

Define a reducer.
A reducer need to implement the interface `IReducer<TState>`.
It describes how an action transforms the state into the next state.

```cs
using System;
using ReduxSharp;

namespace HelloWorld
{
    public class AppReducer : IReducer<AppState>
    {
        public AppState Invoke<TAction>(AppState state, TAction action)
        {
            switch (action)
            {
                case Increment _:
                    return new AppState(state.Count + 1);
                case Decrement _:
                    return new AppState(state.Count - 1);
                default:
                    return state;
            }
        }
    }
}
```

### Store

Build a store using `StoreBuilder<TState>`.
The `Store<TState>` is the class that bring actions and reducer together.
The store has the following responsibilities:

- Holds application state of type TState.
- Allows state to be update via `Dispatch<TAction>(TAction action)`.
- Registers listeners via `Subscribe(IObserver observer)`.The `Store<TState>` class implements IObservable.

Pass the initial state to the builder and the reducer to `Build`.

```c#
using System;
using ReduxSharp;

namespace HelloWorld
{
    class Program : IObserver<AppState>
    {
        static void Main(string[] args)
        {
            Store<AppState> store = new StoreBuilder<AppState>(new AppState(0)).Build(new AppReducer());

            var observer = new Program();
            using (store.Subscribe(observer))
            {
                // Prints 1.
                store.Dispatch(new Increment());
                // Prints 2.
                store.Dispatch(new Increment());
                // Prints 1.
                store.Dispatch(new Decrement());
                // Prints 2.
                store.Dispatch(new Increment());
            }
        }

        public void OnNext(AppState value) =>
            Console.WriteLine(value.Count);

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }
    }
}
```


### Middleware

Register factories in execution order with `Use`, then call `Build`.
Each store builds the chain once per dispatched action type and caches it.
The factory can capture per-store, per-action-type data when building the delegate.

```csharp
using System;
using ReduxSharp;

public sealed class ConsoleLoggingMiddleware<TState> : IMiddlewareFactory<TState>
{
    public Action<TAction> Build<TAction>(IStore<TState> store, Action<TAction> next)
    {
        string actionName = typeof(TAction).Name;
        return action =>
        {
            Console.WriteLine($"Dispatching {actionName}");
            next(action);
            Console.WriteLine($"Dispatched {actionName}");
        };
    }
}
```

```csharp
Store<AppState> store = new StoreBuilder<AppState>(new AppState(0))
    .Use(new ConsoleLoggingMiddleware<AppState>())
    .Build(new AppReducer());
store.Dispatch(new Increment());
```

Registration extensions can accept and return `IStoreBuilder<TState>`. See the
[logging extension](examples/Benchmark/Examples/LoggingMiddlewareExtensions.cs).

**Migrating from the old API.** Replace `new Store<TState>(reducer, state)` with
`new StoreBuilder<TState>(state).Build(reducer)`. For each old middleware,
move the per-dispatch body of `IMiddleware.Invoke` into the delegate returned by
`IMiddlewareFactory.Build`, as shown above. Replace `next.Invoke(action)` with
`next(action)` to continue the current chain. To send another action type, call
`store.Dispatch(otherAction)` to start its pipeline at the first middleware.
Existing reducers and async action creators can be reused.

## Examples

- Counter([source](https://github.com/tnakamura/ReduxSharp/blob/master/examples/Counter))
  - WPF app 
- RxCounter([source](https://github.com/tnakamura/ReduxSharp/blob/master/examples/RxCounter))
  - WPF app 
- TodoList([source](https://github.com/tnakamura/ReduxSharp/blob/master/examples/TodoList))
  - WPF app 

## Publishing to NuGet.org

Publishing a GitHub Release runs the `Publish NuGet package` workflow for the
release commit. Both stable and prerelease versions are supported. Creating or
pushing a tag alone does not publish a package.

### One-time setup

1. On NuGet.org, sign in as an owner of the `ReduxSharp` package and add a
   [Trusted Publishing policy](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing)
   with repository owner `tnakamura`, repository `ReduxSharp`, and workflow file
   `publish-nuget.yml` (the filename only). Leave the environment empty.
2. In the GitHub repository's **Settings > Secrets and variables > Actions**, add
   the repository secret `NUGET_USER` containing that owner's NuGet.org username
   (not an email address). No long-lived NuGet API key is required.
3. Merge the publishing workflow before creating a release tag so the tagged
   commit includes the workflow.

### Publish a version

1. Update `Version` in `src/ReduxSharp/ReduxSharp.csproj`, then commit and merge
   the change. Continue managing `AssemblyVersion` and `FileVersion` separately.
2. Create and push a tag matching `Version` exactly, such as `0.6.0` or
   `0.6.0-beta.1`. Use three numeric components without leading zeroes, optionally
   followed by a SemVer prerelease suffix; do not add a `v` prefix or build metadata.
3. Create a GitHub Release using that tag, prepare its release notes, and select
   **Publish release**. For a prerelease version, mark the release as a prerelease.
4. Check the workflow in **Actions**. It validates the tag against the evaluated
   project version, builds, checks formatting, runs tests, and validates the
   generated package versions before obtaining a short-lived publishing key.

The workflow saves `.nupkg` and `.snupkg` files as Actions artifacts and publishes
both to NuGet.org. If a run fails, fix the setup issue and use **Re-run failed jobs**;
already published packages are skipped, and symbols are pushed separately so a
partial publication can be retried. A published NuGet version cannot be replaced;
code changes require a new version and release. Keep release tags unchanged.

## License

[MIT](https://opensource.org/licenses/MIT)


