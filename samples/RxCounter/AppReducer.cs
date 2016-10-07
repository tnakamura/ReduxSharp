using ReduxSharp;

namespace RxCounter
{
    public class AppReducer : IReducer<AppState>
    {
        public AppState Invoke(AppState state, IAction action)
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
        private CountUpAction() { }

        public static readonly CountUpAction Instance = new CountUpAction();
    }

    public class CountDownAction : IAction
    {
        private CountDownAction() { }

        public static readonly CountDownAction Instance = new CountDownAction();
    }
}
