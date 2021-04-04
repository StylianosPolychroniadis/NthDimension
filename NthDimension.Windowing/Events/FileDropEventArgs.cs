using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Windowing
{
    /// <summary>
    /// Defines the event data for files being dropped onto the window.
    /// </summary>
    public readonly struct FileDropEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDropEventArgs"/> struct.
        /// </summary>
        /// <param name="paths">
        /// The <see cref="System.Array" /> of strings giving the names of all files dropped.
        /// </param>
        public FileDropEventArgs(string[] paths)
        {
            FileNames = paths;
        }

        /// <summary>
        /// Gets the names of the files.
        /// </summary>
        /// <value>An <see cref="System.Array" /> of strings giving the names of all files dropped.</value>
        public string[] FileNames { get; }
    }
}
