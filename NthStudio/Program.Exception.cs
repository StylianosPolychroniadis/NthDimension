using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio
{
    partial class Program
    {
        /// <summary>
        /// Windows Forms unhandled exceptions are displayed in special form, and the error is logged. 
        /// Do not use on OpenGL applications. For WinForms native apps only (handles UI thread exceptions)
        /// </summary>
        static private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        /// <summary>
        /// CLR Unhandled exceptions are displayed in special form, and the error is logged.
        /// </summary>        
        static private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {            
            // 1. Create Dump File
            if (e == null)
                return;

#if _DEBUG
            // ExceptionPolicy.HandleException(ex, "Default Policy");
            //MessageBox.Show("An unhandled exception occurred, and the application is terminating. For more information, see your Application event log.");

            CCLRDump.Dump();
#endif

            // 2. Show Message Box
            HandleException(e.ExceptionObject as Exception);            
        }

        /// <summary>
        /// Displays error form for the exception and optionally
        /// saves the error to the log.
        /// </summary>
        /// <param name="e">Exception class instance.</param>
        static private void HandleException(Exception e)
        {
            NthDimension.ExceptionForm eForm = new NthDimension.ExceptionForm(e);
            System.Windows.Forms.DialogResult dr = eForm.ShowDialog();

            

            if (dr != null)
            {
                if (eForm.LogError)
                    NthDimension.Rendering.Utilities.ConsoleUtil.errorlog("Application error", e.Message);
                    // Log error to the log file
                    //NthDimension.Utilities.C(LOG_ERROR_FILENAME, e.ToString());

                //if(dr == System.Windows.Forms.DialogResult.Ignore)
                    
                if (dr == System.Windows.Forms.DialogResult.OK)
                    Environment.Exit(-1); // Shut down application, indicating an error
            }

        }
    }
}
