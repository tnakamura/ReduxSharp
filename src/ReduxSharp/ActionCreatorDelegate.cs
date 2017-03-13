using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp
{
    public delegate IAction ActionCreatorDelegate<TState>(TState state, IStore<TState> store);
}
