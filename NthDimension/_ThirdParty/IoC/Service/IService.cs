using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Service
{
    /// <summary>
    ///   Defines the interface for service classes.
    /// </summary>
    public interface IService : IConfigurable, IRunnable
    {
        /// <summary>
        ///   When implemented in a class, gets the logger for log operations.
        /// </summary>
        ILog Log { get; }

        /// <summary>
        ///   When implemented in a class, signals the implementer that it should execute its operations.
        /// </summary>
        void Execute();
    }
}
