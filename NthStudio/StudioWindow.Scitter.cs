#if SCITER

using SciterSharp;
using SciterSharp.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio
{
    public partial class StudioWindow
    {
        #region P/Invoke
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, Int32 nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            }
            else
            {
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            }
        }

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLong")]
        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "This declaration is not used on 64-bit Windows.")]
        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "2", Justification = "This declaration is not used on 64-bit Windows.")]
        private static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, Int32 nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtr")]
        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist", Justification = "Entry point does exist on 64-bit Windows.")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, Int32 nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, int uMsg, int wParam, int lParam);


        private const int GWL_WNDPROC = -4;

        
        private delegate IntPtr SciterWindowProc(IntPtr hWnd, int Msg, int wParam, int lParam);
        #endregion

        public SciterWindow     SciterWnd       { get; private set; }
        private Host            AppHost         = new Host();

        // Subclass
        private SciterWindowProc        newWndProc          = null;
        private IntPtr                  oldWndProc          = IntPtr.Zero;
        private IntPtr                  winHook             = IntPtr.Zero;

        private IntPtr newWindowProc(IntPtr hWnd, int Msg, int wParam, int lParam)
        {
            //switch (Msg)
            //{
            //    case (int)winMessage.WM_GETMINMAXINFO:
            //        MessageBox.Show("Moving");
            //        return DefWindowProc(hWnd, Msg, wParam, lParam);

            //}
            return DefWindowProc(hWnd, Msg, wParam, lParam);
        }

        private void sciterInitialize()
        {
            NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format(">>> Initializing Scitter wHnd {0} Width {1} Height {2}", this.WindowInfo.Handle, this.Width, this.Height));

            return;

            #region Sciter Window -> Child, Owned, etc
            ////SciterWnd = new SciterWindow(this.WindowInfo.Handle);
            ////SciterWnd.CreateOwnedWindow(this.WindowInfo.Handle, this.Width, this.Height, SciterSharp.Interop.SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_CHILD | SciterSharp.Interop.SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_OWNS_VM);
            ////SciterWnd.LoadHtml(
            ////    "<body>" +
            ////        "<code>Add an event handler to the <b>HandleCreated</b> event for any needed initialization (e.g.: load the HTML)</code><br /><br />" +
            ////        "<code>In the handler, use the <b>SciterWnd</b> property of this control to access the SciterWindow instance.</code>" +
            ////    "</body>"
            ////    );
            //////SciterWnd.LoadPage("http://www.polytronic.gr");
            ////SciterWnd.Show();
            //////SciterSharp.Interop.PInvokeUtils.RunMsgLoop();
            #endregion Sciter Window -> Child, Owned, etc

            #region GLFW Integration demo
            
            if (sciterAttachWindow(WindowInfo.Handle))
            {
                SciterX.API.SciterLoadFile(WindowInfo.Handle, @"D:\NthDimension 2018 Revisited\@References\@html\scitter-sdk\samples\charts\test.htm");
                //SciterLoadFile(hwnd(window), L"sciter-glfw-basic-facade.htm");

                IntPtr rootPtr;
                SciterX.API.SciterGetRootElement(WindowInfo.Handle, out rootPtr);


                ////    //sciter::dom::element root = sciter::dom::element::root_element(hwnd(window));


                SciterElement front;
                SciterElement back;

                //    //    //back_layer = root.find_first("section#back-layer");
                //    //    //fore_layer = root.find_first("section#fore-layer");
                //    //    ////assert(back_layer && fore_layer);
            }
            #endregion GLFW Integration demo
        }
            

        private bool sciterAttachWindow(IntPtr wHnd)
        {
            if (wHnd != IntPtr.Zero)
            {
                SciterX.API.SciterSetOption(wHnd, SciterXDef.SCITER_RT_OPTIONS.SCITER_ALPHA_WINDOW, IntPtr.Zero);
                //SciterSetOption(NULL, SCITER_SET_DEBUG_MODE, TRUE);

                IntPtr hw = wHnd;


                //// subclass the window
                var former = GetWindowLongPtr(hw, GWL_WNDPROC);
                newWndProc = new SciterWindowProc(newWindowProc);
                IntPtr newWndProcPtr = Marshal.GetFunctionPointerForDelegate(newWndProc);

                //if (IntPtr.Size == 4) // 32 v 64 bit system
                //    oldWndProc = SetWindowLongPtr32(hw, GWL_WNDPROC, newWndProcPtr);
                //else
                //    oldWndProc = SetWindowLongPtr64(hw, -4, newWndProcPtr);

                oldWndProc = SetWindowLongPtr64(hw, -4, newWndProcPtr);

                bool handled = false;
                SciterX.API.SciterProcND(wHnd, 0, IntPtr.Zero, IntPtr.Zero, ref handled);
                //sciter::attach_dom_event_handler(hw, &the_dom_events_handler);    // TODO
                return true;
            }
            else
                return false;

            //var host = new Host();
            //host.SetupWindow(wHnd);
            //host.AttachEvh(new HostEvh());
            //host.SetupPage(@"D:\NthDimension 2018 Revisited\@References\@html\scitter-sdk\samples\charts\test.htm");

            return true;
        }

        /// <summary>
        /// Call right after GL.Clear (not used in deferred)
        /// </summary>
        private void sciterRenderClear()
        {
            //// SCITER 
            //{
            //    SCITER_X_MSG_PAINT pc(back_layer, FALSE);
            //    pc.targetType = SPT_RECEIVER;
            //    pc.target.receiver.callback = bitmap_receiver;
            //    pc.target.receiver.param = 0;
            //    SciterProcX(hwnd(window), pc);
            //    // draw background layer
            //    //SciterProcX(hwnd(window), SCITER_X_MSG_PAINT(back_layer,FALSE));
            //}
            //// SCITER
        }

        /// <summary>
        /// Before calling SwapBuffers
        /// </summary>
        private void sciterRender()
        {
            //// SCITER 
            //{
            //    // draw foreground layer
            //    SciterProcX(hwnd(window), SCITER_X_MSG_PAINT(fore_layer, TRUE));
            //}
            //// SCITER
        }
    }
}

#endif