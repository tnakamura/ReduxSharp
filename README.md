# ReduxSharp

Unidirectional Data Flow in C# - Inspired by Redux

[![NuGet](https://img.shields.io/nuget/v/ReduxSharp.svg?maxAge=3600)](https://www.nuget.org/packages/ReduxSharp/)


## About ReduxSharp

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
    public readonly struct Increment {}

    public readonly struct Decrement {}
}
```

### Reducers

Define a Reducer.

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

Create an instance of `Store<TState>`.

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


### Middlewares

ReduxSharp supports middlewares.

You can insert processing before and after `IReducer<TState>.Invoke<TAction>(TState, TAction)` is called.

```cs
using Newtonsoft.Json;
using ReduxSharp;

public class ConsoleLoggingMiddleware<TState> : IMiddleware<TState>
{
    public void Invoke<TAction>(IStore<TState> store, IDispatcher next, TAction action)
    {
        Console.WriteLine(JsonConvert.SerializeObject(store.State));

        next.Invoke(action);

        Console.WriteLine(JsonConvert.SerializeObject(store.State));
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


