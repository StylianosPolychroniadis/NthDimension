namespace NthStudio.Plugins
{
    /// <summary>
    /// Handler delegate for plugin store refersh progress event
    /// </summary>
    /// <param name="e"></param>
    public delegate void PluginStoreRefreshHandler(PluginStoreRefreshProgressEventArgs e);

    /// <summary>
    /// Handler delegate for plugin store refersh process start event
    /// </summary>
    public delegate void PluginStoreRefreshOnStartHandler();

    /// <summary>
    /// Handler delegate for plugin store refersh completed event
    /// </summary>
    public delegate void PluginStoreRefreshOnCompleteHandler();
}
