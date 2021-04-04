using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Windowing
{
    /// <summary>
    /// Defines the event data for the window minimizing event.
    /// </summary>
    public readonly struct MinimizedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinimizedEventArgs"/> struct.
        /// </summary>
        /// <param name="isMinimized">
        /// A value indicating whether the window is minimized.
        /// </param>
        public MinimizedEventArgs(bool isMinimized)
        {
            IsMinimized = isMinimized;
        }

        /// <summary>
        /// Gets a value indicating whether the window is minimized.
        /// </summary>
        public bool IsMinimized { get; }
    }
}
