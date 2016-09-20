using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReduxSharp;

namespace Counter
{
    public class CountDownAction : IAction
    {
        private CountDownAction() { }

        public static readonly CountDownAction Instance = new CountDownAction();
    }
}
