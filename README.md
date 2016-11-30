# ReduxSharp

Unidirectional Data Flow in C# - Inspired by Redux


## Install

First, [install Nuget](http://docs.nuget.org/docs/start-here/installing-nuget).
Then, install [ReduxSharp](http://www.nuget.org/packages/ReduxSharp) from the package manager console:

```
PM> Install-Package ReduxSharp
```


## Usage

```cs
using System;
using ReduxSharp;

namespace ReduxSharpSample
{
    public class AppState
    {
        public int Count { get; set; } = 0;
    }

    public class IncrementAction : IAction {}

    public class DecrementAction : IAction {}

    public class AppReducer : IReducer<AppState>
    {
        public AppState Invoke(AppState state, IAction action)
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

    class Program
    {
        static void Main(string[] args)
        {
            IStore<AppState> store = new StoreBuilder<AppState>(new AppReducer())
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


## Contribution

1. Fork it
2. Create your feature branch ( git checkout -b my-new-feature )
3. Commit your changes ( git commit -am 'Add some feature' )
4. Push to the branch ( git push origin my-new-feature )
5. Create new Pull Request


## License

[MIT](https://opensource.org/licenses/MIT)


## Author

[tnakamura](https://github.com/tnakamura)

