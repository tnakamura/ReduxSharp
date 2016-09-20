using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReduxSharp;

namespace Counter
{
    public class CounterReducer : IReducer<AppState>
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
}
