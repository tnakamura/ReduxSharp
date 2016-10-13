using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp.Tests
{
    public class AppReducer : IReducer<AppState>
    {
        public AppState Invoke(AppState state, IAction action)
        {
            return state ?? new AppState();
        }
    }
}
