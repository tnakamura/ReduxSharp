using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReduxSharp;

namespace Counter
{
    public class CountUpAction : IAction
    {
        private CountUpAction() { }

        public static readonly CountUpAction Instance = new CountUpAction();
    }
}
