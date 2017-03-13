using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp
{
    public delegate Task AsyncActionCreatorDelegate<TState>(
        TState state,
        IStore<TState> store,
        Action<ActionCreatorDelegate<TState>> callback);
}
