# ReduxSharp

Unidirectional Data Flow in C# - Inspired by Redux

[![NuGet](https://img.shields.io/nuget/v/ReduxSharp.svg?maxAge=2592000)](https://www.nuget.org/packages/ReduxSharp/)

## Installation

First, [install Nuget](http://docs.nuget.org/docs/start-here/installing-nuget).
Then, install [ReduxSharp](http://www.nuget.org/packages/ReduxSharp) from the package manager console:

```
PM> Install-Package ReduxSharp
```


## Quick-start

### Actions

Actions are payloads of information that send data from your application to your store.
They only need to implement the markup interface `IAction`.

```cs
using System;
using ReduxSharp;

namespace ReduxSharpSample
{
    public class IncrementAction : IAction {}

    public class DecrementAction : IAction {}
}
```

### Reducers

A reducer is a pure function with `(TState state, IAction action) => TState` signature.
It describes how an action transforms the state into the next state.

```cs
using System;
using ReduxSharp;

namespace ReduxSharpSample
{
    public static class AppReducer
    {
        public static AppState Invoke(AppState state, IAction action)
        {
            if (action is IncrementAction)
            {
                return new AppState()
                {
                    Count = state.Count + 1
                };
            }

            if (action is DecrementAction)
            {
                return new AppState()
                {
                    Count = state.Count - 1
                };
            }

            return state;
        }
    }
}
```

### State

```cs
using System;
using ReduxSharp;

namespace ReduxSharpSample
{
    public class AppState
    {
        public int Count { get; set; } = 0;
    }
}
```

### Store

The `Store<TState>` is the class that bring actions and reducer together.
The store has the following responsibilities:

- Holds application state of type TState.
- Allows state to be update via `Dispatch(IAction action)`.
- Registers listeners via `Subscribe(IObserver observer)`.The `Store<TState>` class implements IObservable.

The `StoreBuilder<TState>` take an initial state, of type TState, and a reducer.

```c#
using System;
using ReduxSharp;

namespace ReduxSharpSample
{
    class Program
    {
        static void Main(string[] args)
        {
            IStore<AppState> store = new StoreBuilder<AppState>(AppReducer.Invoke)
                .UseInitialState(new AppState())
                .Build();

            Console.WriteLine(store.State.Count); // => 0

            store.Dispatch(new IncrementAction());
            Console.WriteLine(store.State.Count); // => 1

            store.Dispatch(new DecrementAction());
            Console.WriteLine(store.State.Count); // => 0
        }
    } 
}
```

## Examples

- Counter([source](https://github.com/tnakamura/ReduxSharp/blob/master/examples/Counter))
  - WPF app 
- RxCounter([source](https://github.com/tnakamura/ReduxSharp/blob/master/examples/RxCounter))
  - WPF app 
- TodoList([source](https://github.com/tnakamura/ReduxSharp/blob/master/examples/TodoList))
  - WPF app 

## License

[MIT](https://opensource.org/licenses/MIT)


