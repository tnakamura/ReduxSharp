using ReduxSharp;

namespace RxCounter
{
    public static class AppReducer
    {
        public static AppState Invoke(AppState state, IAction action)
        {
            if (action is CountUpAction)
            {
                state.Counter = new CounterState()
                {
                    Count = state.Counter.Count + 1
                };
                return state;
            }
            else if (action is CountDownAction)
            {
                state.Counter = new CounterState()
                {
                    Count = state.Counter.Count - 1
                };
                return state;
            }
            else
            {
                return state;
            }
        }
    }

    public class CountUpAction : IAction
    {
        CountUpAction() { }

        public static readonly CountUpAction Instance = new CountUpAction();
    }

    public class CountDownAction : IAction
    {
        CountDownAction() { }

        public static readonly CountDownAction Instance = new CountDownAction();
    }
}
