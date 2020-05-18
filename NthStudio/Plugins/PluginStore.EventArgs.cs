using System;

namespace NthStudio.Plugins
{
    /// <summary>
    /// Event arguments to notify about plugin store refersh progress
    /// </summary>
    public class PluginStoreRefreshProgressEventArgs : EventArgs
    {
        /// <summary>
        /// The total number of files to process
        /// </summary>
        public int Total { get; private set; }

        /// <summary>
        /// The current file being processed
        /// </summary>
        public int Current { get; private set; }

        /// <summary>
        /// The plugin just discovered
        /// </summary>
        public PluginInfo CurrentPlugin { get; private set; }

        /// <summary>
        /// Creates a new  instance of <see cref="PluginStorerefershProgressEventArgs"/>
        /// </summary>
        /// <param name="currentPlugin"></param>
        /// <param name="total"></param>
        /// <param name="current"></param>
        public PluginStoreRefreshProgressEventArgs(PluginInfo currentPlugin, int total, int current)
        {
            Total = total;
            Current = current;
            CurrentPlugin = currentPlugin;
        }
    }
}
