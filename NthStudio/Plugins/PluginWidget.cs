using System;
using System.Collections.Generic;
using NthDimension.Forms;
using NthDimension.Service;

#region Microsoft Windows 7 - Microsoft Windows 10 
#if _WINFORMS_
    using System.Windows.Forms;
#endif

#if _WPF_

#endif
#endregion

#region Google Inc.
#if _GOOGLE_ANDROIDOS_
    // TODO as required
#endif
#endregion

#region Apple 
#if _APPLE_IOS_
    // TODO as required
#endif
#endregion

namespace NthStudio.Plugins
{
    public class PluginWidget
    {
        public Dictionary<Widget, List<Action<INotifyWidget, NotificationEventArgs>>> Plugins;

        //public Control                          Control;
        //public List<Action<T1, EventArgs>>      Actions;

        public PluginWidget() { Plugins = new Dictionary<Widget, List<Action<INotifyWidget, NotificationEventArgs>>>(); }
    }
}
