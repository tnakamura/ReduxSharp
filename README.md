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

```cs
using System;
using ReduxSharp;

namespace ReduxSharpSample
{
    public class IncrementAction {}

    public class DecrementAction {}
}
```

### Reducers

A reducer need to implement the interface `IReducer<TState>`.
It describes how an action transforms the state into the next state.

```cs
using System;
using ReduxSharp;

namespace ReduxSharpSample
{
    public class AppReducer : IReducer<AppState>
    {
        public async ValueTask<AppState> Invoke<TAction>(AppState state, TAction action)
        {
            if (action is IncrementAction)
            {
                return await Task.Run(() =>
                {
                    return new AppState
                    {
                        Count = state.Count + 1
                    };
                });
            }
            if (action is DecrementAction)
            {
                return new AppState
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
- Allows state to be update via `Dispatch<TAction>(TAction action)`.
- Registers listeners via `Subscribe(IObserver observer)`.The `Store<TState>` class implements IObservable.

The `Store<TState>` take an initial state, of type TState, and a reducer.

```c#
using System;
using ReduxSharp;

namespace ReduxSharpSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var store = new Store<AppState>(new AppReducer(), new AppState());

            Console.WriteLine(store.State.Count); // => 0

            await store.Dispatch(new IncrementAction());
            Console.WriteLine(store.State.Count); // => 1

            await store.Dispatch(new DecrementAction());
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


