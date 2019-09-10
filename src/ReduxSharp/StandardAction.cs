using System;
using System.Collections.Generic;

namespace ReduxSharp
{
    /// <summary>
    /// ReduxSharp's built in action type.
    /// </summary>
#pragma warning disable CS0612
    public sealed class StandardAction : IAction
#pragma warning restore CS0612  
    {
        /// <summary>
        /// Initializes a new instance of <see cref="StandardAction"/> class.
        /// </summary>
        /// <param name="type">String representation of the Action type</param>
        /// <param name="payload">Payload convertable to JSON</param>
        public StandardAction(string type, IDictionary<string, object> payload = null)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
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
