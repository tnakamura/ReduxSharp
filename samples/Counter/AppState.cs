using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReduxSharp;

namespace Counter
{
    public class AppState
    {
        public CounterState Counter { get; set; } = new CounterState();
    }
}
