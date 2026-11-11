using ReduxSharp;

namespace RxCounter
{
    /// <summary>Computes the next application state from dispatched actions.</summary>
    public class AppReducer : IReducer<AppState>
    {
        /// <inheritdoc/>
        public AppState Invoke<TAction>(AppState state, TAction action)
        {
            if (action is CountUpAction)
            {
                return new AppState
                {
                    Counter = new CounterState
                    {
                        Count = state.Counter.Count + 1,
                    }
                };
            }
            else if (action is CountDownAction)
            {
                return new AppState
                {
                    Counter = new CounterState
                    {
                        Count = state.Counter.Count - 1,
                    }
                };
            }
            else
            {
                return state;
            }
        }
    }

    /// <summary>Increments the counter.</summary>
    public class CountUpAction
    {
        CountUpAction()
        {
        }

        /// <summary>Provides the shared action instance.</summary>
        public static readonly CountUpAction Instance = new CountUpAction();
    }

    /// <summary>Decrements the counter.</summary>
    public class CountDownAction
    {
        CountDownAction()
        {
        }

        /// <summary>Provides the shared action instance.</summary>
        public static readonly CountDownAction Instance = new CountDownAction();
    }
}
