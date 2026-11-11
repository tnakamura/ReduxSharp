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
                store.Dispatch(new Increment());
                store.Dispatch(new Increment());
                store.Dispatch(new Decrement());
                store.Dispatch(new Increment());
            }

            Console.ReadLine();
        }

        /// <inheritdoc/>
        public void OnNext(AppState value) =>
            Console.WriteLine(value.Count);

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
        }
    }

    /// <summary>Holds the application state.</summary>
    public class AppState
    {
        /// <summary>Initializes the counter with the supplied value.</summary>
        public AppState(int count) => Count = count;

        /// <summary>Gets the counter value.</summary>
        public int Count
        {
            get;
        }
    }

    /// <summary>Increments the counter.</summary>
    public readonly struct Increment
    {
    }

    /// <summary>Decrements the counter.</summary>
    public readonly struct Decrement
    {
    }

    /// <summary>Computes the next application state from dispatched actions.</summary>
    public class AppReducer : IReducer<AppState>
    {
        /// <inheritdoc/>
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
