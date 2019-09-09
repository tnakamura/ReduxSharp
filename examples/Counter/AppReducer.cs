using System.Threading.Tasks;
using ReduxSharp;

namespace Counter
{
    public class AppReducer : IReducer<AppState>
    {
        public async ValueTask<AppState> Invoke<TAction>(AppState state, TAction action)
        {
            if (action is CountUpAction)
            {
                return await Task.Run(() =>
                {
                    return new AppState
                    {
                        Counter = new CounterState
                        {
                            Count = state.Counter.Count + 1,
                        }
                    };
                });
            }
            else if (action is CountDownAction)
            {
                return await Task.Run(() =>
                {
                    return new AppState
                    {
                        Counter = new CounterState
                        {
                            Count = state.Counter.Count - 1,
                        }
                    };
                });
            }
            else
            {
                return state;
            }
        }
    }

    public class CountUpAction
    {
        CountUpAction() { }

        public static readonly CountUpAction Instance = new CountUpAction();
    }

    public class CountDownAction
    {
        CountDownAction() { }

        public static readonly CountDownAction Instance = new CountDownAction();
    }
}
