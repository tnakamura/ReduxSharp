# ReduxSharp

Unidirectional Data Flow in C# - Inspired by Redux

[![NuGet](https://img.shields.io/nuget/v/ReduxSharp.svg?maxAge=3600)](https://www.nuget.org/packages/ReduxSharp/)


## Installation

First, [install Nuget](http://docs.nuget.org/docs/start-here/installing-nuget).
Then, install [ReduxSharp](http://www.nuget.org/packages/ReduxSharp) from the package manager console:

```
PM> Install-Package ReduxSharp
```


## Quick-start

### State

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

Actions are payloads of information that send data from your application to your store.

```cs
namespace HelloWorld
{
    public readonly struct Increment {}

    public readonly struct Decrement {}
}
```

### Reducers

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

The `Store<TState>` is the class that bring actions and reducer together.
The store has the following responsibilities:

- Holds application state of type TState.
- Allows state to be update via `Dispatch<TAction>(TAction action)`.
- Registers listeners via `Subscribe(IObserver observer)`.The `Store<TState>` class implements IObservable.

The `Store<TState>` take an initial state, of type TState, and a reducer.

```c#
using System;
using ReduxSharp;

namespace HelloWorld
{
    class Program : IObserver<AppState>
    {
        static void Main(string[] args)
        {
            var store = new Store<AppState>(
                new AppReducer(),
                new AppState(0));

            var p = new Program();
            using (store.Subscribe(p))
            {
                store.Dispatch(new Increment()); // => 1
                store.Dispatch(new Increment()); // => 2
                store.Dispatch(new Decrement()); // => 1
                store.Dispatch(new Increment()); // => 2
            }
        }

        public void OnNext(AppState value) =>
            Console.WriteLine(value.Count);

        public void OnCompleted() { }

        public void OnError(Exception error) { }
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


