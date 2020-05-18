using NthDimension.Forms;
using NthDimension.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NthStudio.Plugins
{
    public static class PluginFactory
    {

        public static PluginWidget CreateFromStore(PluginStore pluginStore,
                                                    ref Action<INotifyWidget, NotificationEventArgs> actionNotify)
        {
            PluginWidget pCtrls = new PluginWidget();
            foreach (var p in pluginStore.Plugins)
            {
                if (p is IService)
                    ServiceManager.SingleInstance.Services.Add(p as IService);

                Widget pluginControl =
                    (Widget)
                    Activator.CreateInstance(
                        Assembly.LoadFile(EnvironmentSettings.GetFullPath(p.AssemblyFile)).GetType(p.Type));


                if (pluginControl is INotifyWidget)
                    ((INotifyWidget)pluginControl).OnNotify += actionNotify;







                // Note: I believe this is ok, TODO:: Resolve the mapping mechanism I store in the Dictionary... Re-engineer see where is useful 
                pCtrls.Plugins.Add(pluginControl, new List<Action<INotifyWidget, NotificationEventArgs>>()
                    {
                        actionNotify //,
                        //action
                    });
            }
            return pCtrls;
        }


        //public static List<Type> GetInversionInterfaces(string Namespace = "Syscon.Ioc")
        public static List<Type> GetInversionInterfaces(string Namespace = "NthStudio") // NthDimension.Context???
        {
            string @namespace = Namespace; // "Inversion";

            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsInterface && t.Namespace == @namespace
                    select t;

            return q.ToList();
        }

        //private static Dictionary<string, Dictionary<object, Delegate[]>> GetHandlersFrom(Control ctrl)
        //{
        //    var ctrlEventsCollection = (EventHandlerList) typeof (Control)
        //                                                      .GetProperty("Events",
        //                                                                   BF.GetProperty | BF.NonPublic | BF.Instance)
        //                                                      .GetValue(ctrl, null);

        //    var headInfo = typeof (EventHandlerList)
        //        .GetField("head", BF.Instance | BF.NonPublic);
        //    var handlers = BuildList(headInfo, ctrlEventsCollection);

        //    var eventName = GetEventNameFromKey(ctrl, handlers.First().Key);

        //    //TODO: use key value pair.
        //    var result = new Dictionary<string, Dictionary<object,
        //        Delegate[]>> {{eventName, handlers}};

        //    return result;
        //}

        //public static 
    }
}
