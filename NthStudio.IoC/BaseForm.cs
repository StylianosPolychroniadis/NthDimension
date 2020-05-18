using System;

using NthDimension;
using NthDimension.Forms;

namespace NthStudio.IoC
{
    public class BaseForm : Window
    {
        public delegate void PluginAction(object plugin);

        #region Default Instance

        public static BaseForm defaultInstance;

        /// <summary>
        /// Added by the VB.Net to C# Converter to support default instance behavour in C#
        /// </summary>
        public static BaseForm Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new BaseForm();
                    //defaultInstance. DefaultInstance_Closed;  
                }

                return defaultInstance;
            }
        }

        private static void DefaultInstance_Closed(object sender, System.EventArgs e)
        {
            defaultInstance = null;
        }
        #endregion

        public BaseForm()
        {

        }
    }
}
