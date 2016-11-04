using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp
{
    /// <summary>
    /// ReduxSharp's built in action type.
    /// </summary>
    public class StandardAction : IAction
    {
        /// <summary>
        /// Initializes a new instance of <see cref="StandardAction"/> class.
        /// </summary>
        /// <param name="type">String representation of the Action type</param>
        /// <param name="payload">Payload convertable to JSON</param>
        public StandardAction(string type, IDictionary<string, object> payload = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            Type = type;
            Payload = payload;
        }

        /// <summary>
        /// A string that identifies the type of this <see cref="StandardAction"/>
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// An untyped, JSON-compatible payload
        /// </summary>
        public IDictionary<string, object> Payload { get; }
    }
}
