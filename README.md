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

## License

[MIT](https://opensource.org/licenses/MIT)


