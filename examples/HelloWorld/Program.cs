namespace HelloWorld
{
    using System;
    using ReduxSharp;

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
                store.Dispatch(new Increment());
                store.Dispatch(new Increment());
                store.Dispatch(new Decrement());
                store.Dispatch(new Increment());
            }

            Console.ReadLine();
        }

        public void OnNext(AppState value) =>
            Console.WriteLine(value.Count);

        public void OnCompleted() { }

        public void OnError(Exception error) { }
    }

    public class AppState
    {
        public AppState(int count) => Count = count;

        public int Count { get; }
    }

    public readonly struct Increment { }

    public readonly struct Decrement { }

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
