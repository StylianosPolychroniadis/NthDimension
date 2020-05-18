
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Diagnostics;

using NthDimension.Forms.Delegates;
using NthDimension.Forms.Layout;
using NthDimension.Forms.Events;
using NthDimension.Forms.Widgets;

namespace NthDimension.Forms
{
    public partial class Widget
    {
        /// <summary>
        /// Widget HUD (Heads-Up Display)
        /// </summary>
        public abstract class WHUD : Widget
        {
        public event TitleChangedHandler TitleChangedEvent;

        static bool MouseDown = false;
        static bool MouseDownForClick = false;
        bool capturedMouseOvl = false;
        Widget LastHoveredWidget;

        public static double MainTimer()
        {
            return Timer.ElapsedTime; //Glfw.glfwGetTime();
        }

        static WHUD()
        {
            FrontLayoutsToUpdate = new LayoutList();
        }

        protected WHUD()
        {
            Overlays = new WidgetList(this);
            OverlaysToOnShowDialog = new List<Overlay>();

            BGColor = Color.White;
            Title_ = "Window ...";
            StartPosition = EWinStartPosition.CenterScreen;

            Widgets.WidgetAddedEvent += Widgets_WidgetAddedEvent;
        }

        public WidgetList Overlays
        {
            get;
            protected set;
        }

        internal List<Overlay> OverlaysToOnShowDialog
        {
            get;
            set;
        }

public MenuStrip MainMenu
{
    get;
    private set;
}

        public EWinStartPosition StartPosition
        {
            get;
            set;
        }

        string Title_;

        /// <summary>
        /// Gets or sets the title of Shell Window.
        /// </summary>
        /// <value>The title.</value>
        public virtual string Title
        {
            get { return Title_; }
            set
            {
                Title_ = value;
                if (TitleChangedEvent != null)
                    TitleChangedEvent(value);
            }
        }

        /// <summary>
        /// OpenTK error in Windows, when the mouse 'MouseDown' is pressed the 'MouseLeave' event is also triggered. (?!?)
        /// </summary>
        protected bool MouseUpFromClick
        {
            get;
            private set;
        }

        internal static LayoutList FrontLayoutsToUpdate
        {
            get;
            private set;
        }

        public static IUiContext LibContext
        {
            get;
            protected set;
        }

        protected void InitMousePosition(int x, int y)
        {
            var me = new MouseEventArgs(MouseButton.None,
                                        x, y, 0, 0);
            ProcessMouseMove(me);
        }

        void Widgets_WidgetAddedEvent(Widget widgetAdded)
        {

            var ms = widgetAdded as MenuStrip;

            if (ms != null && MainMenu == null)
            {
                MainMenu = ms;
                MainMenu.Dock = EDocking.Top;
            }
        }

        protected override void DoPaint(GContext parentGContext)
        {
            if (DoRepaintTree == false)
                return;

            DoRepaintTree = false;

            base.DoPaint(parentGContext);

            var warray = new Widget[Overlays.Count];
            Overlays.CopyTo(warray, 0);

            foreach (Widget ovl in warray)
            {
                ovl.DoPaint(parentGContext);
            }

            foreach (Overlay ovl in OverlaysToOnShowDialog)
            {
                ovl.OnShowDialog();
            }
            OverlaysToOnShowDialog.Clear();
        }

        void LookUpNotifyMouseEnter(Widget w, BaseEventArgs ea)
        {
            Widget wp = w.Parent;

            while (wp != null && wp.onEnter == false)
            {
                wp.onEnter = true;
                ea.IsNotification = true;
                wp.OnMouseEnter(ea);
                wp = wp.Parent;
            }
            w.onEnter = true;
            Cursors.Cursor = Cursors.Default;
            ea.IsNotification = true;
            w.OnMouseEnter(ea);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="mousePosX">GameWindow coordinates</param>
        /// <param name="mousePosY">GameWindow coordinates</param>
        /// <param name="ea"></param>
        void LookUpNotifyMouseLeave(Widget w, int mousePosX, int mousePosY, BaseEventArgs ea)
        {
            var mp = new Point(mousePosX, mousePosY);

            Widget wp = w.Parent;

            while (wp != null && wp.onEnter == true)
            {
                Point localPoint = wp.WindowToLocal(mp);

                if (wp.VisibleAfterClipping == false
                    || localPoint.X < 0
                    || localPoint.Y < 0
                    || localPoint.X >= wp.Width
                    || localPoint.Y >= wp.Height)
                {
                    wp.onEnter = false;
                    ea.IsNotification = true;
                    wp.OnMouseLeave(ea);
                }
                wp = wp.Parent;
            }

            w.onEnter = false;
            ea.IsNotification = true;
            w.OnMouseLeave(ea);
        }

        void LookUpNotifyMouseMove(Widget w, MouseEventArgs mea)
        {
            if (w.Parent != null)
            {
                // New Mouse Event Args
                var nmea = new MouseEventArgs(mea.Button,
                                              w.X + mea.X, w.Y + mea.Y,
                                              mea.DeltaX, mea.DeltaY, mea.DeltaWheel);

                LookUpNotifyMouseMove(w.Parent, nmea);

                // Go down the hierarchy of Widgets until you reach 'WHUD' whose 'Parent == null', 
                // and then call 'OnMouseMove ()' up until you get to 'HoveredWidget' which is called 
                // out of this method. The logical thing is to first call the Widget closest to the 
                // root Widget and finally to the most superficial Widget, 
                // both must comply with 'NotifyForMouseMove'.
                if (w.Parent.NotifyForMouseMove)
                {
                    // Es una notificación. Por encima hay otros Widgets.
                    nmea.IsNotification = true;
                    w.Parent.OnMouseMove(nmea);
                }
            }
        }

        void LookUpNotifyMouseDown(Widget w, MouseEventArgs mea)
        {
            if (w.Parent != null)
            {
                // New Mouse Event Args
                var nmea = new MouseEventArgs(mea.Button,
                                              w.X + mea.X, w.Y + mea.Y,
                                              mea.DeltaX, mea.DeltaY, mea.DeltaWheel);


                LookUpNotifyMouseDown(w.Parent, nmea);

                // Go down the hierarchy of Widgets until you reach 'WHUD' whose 'Parent == null', 
                // and then call 'OnMouseDown ()' up until you get to 'HoveredWidget' which is called 
                // out of this method. The logical thing is to first call the Widget closest to the 
                // root Widget and finally to the most superficial Widget, 
                // both must comply with 'NotifyForMouseDown'.
                if (w.Parent.NotifyForMouseDown)
                {
                    // Notify higher order Widgets
                    nmea.IsNotification = true;
                    w.Parent.OnNotifyMouseDown(nmea);
                }
            }
        }

        void LookUpNotifyMouseUp(Widget w, MouseEventArgs mea)
        {
            if (w.Parent != null)
            {
                // New Mouse Event Args
                var nmea = new MouseEventArgs(mea.Button,
                                              w.X + mea.X, w.Y + mea.Y,
                                              mea.DeltaX, mea.DeltaY, mea.DeltaWheel);


                LookUpNotifyMouseUp(w.Parent, nmea);

                if (w.Parent.NotifyForMouseUp)
                {
                    nmea.IsNotification = true;
                    w.Parent.OnMouseUp(nmea);
                }
            }
        }

        public virtual Widget GetWidgetAtOverlay(Widget ovl, int x, int y, out int lx, out int ly)
        {
            if (VisibleAfterClipping == false || ovl.IsHide || x < 0 || y < 0 || x >= ovl.Width || y >= ovl.Height)
            {
                lx = 0;
                ly = 0;
                return null;
            }

            foreach (Widget child in ovl.Widgets)
            {
                Widget found = child.GetControlAt(x - child.X, y - child.Y, out lx, out ly);

                if (found != null)
                {
                    return found;
                }
            }
            lx = x;
            ly = y;
            return ovl;
        }

        int hoveredMouseX, hoveredMouseY;

        public Overlay OverlayCurrent;
        protected void ProcessMouseMove(MouseEventArgs e)
        {
            // e.X & e.Y in GameWindow Coordinates
            //WindowHUD.Title= String.Format("e.X = {0}; e.Y = {1}", e.X, e.Y);

            // TODO This code is necessary because in Windows when entering the mouse pointer in the window 'OpenTK.GameWindow', 
            // TODO     the coordinates e.X or e.Y, are negative or greater than the width or height of the window!?. In theory, 
            // TODO     the coordinates of 'MouseEventArgs' are limited to the client area.
            if (e.X < 0 || e.Y < 0
                || e.X >= Width || e.Y >= Height)
                return;

                if (WindowDialog != null)
                {
                    if (WindowDialog.DownForMove)
                    {

                        WindowDialog.NotifyForMouseGlobalMove(e.X, e.Y);
                        return;
                    }

                    if (!WindowDialog.Bounds.Contains(e.X, e.Y))
                        return;
                }

            capturedMouseOvl = false;
            bool foundInOverlay = false;
            MouseEventArgs nmea;

            //WindowHUD.Title = String.Format("FocusCaptured = {0}", MouseFocusCaptured);

            if (MouseFocusCaptured == false)
            {
                //        //Widget ovl = Overlays[Overlays.Count - 1];

                if (null != OverlayCurrent && OverlayCurrent.IsVisible)
                {
                    Point ll = OverlayCurrent.WindowToLocal(e.X, e.Y);

                    HoveredWidget = GetWidgetAtOverlay(OverlayCurrent, ll.X, ll.Y, out hoveredMouseX, out hoveredMouseY);

                    if (HoveredWidget != null)
                    {
                        foundInOverlay = true;
                        goto found;
                    }
                }


                foreach (Widget ovl in Overlays)
                {
                    Point ll = ovl.WindowToLocal(e.X, e.Y);

                    HoveredWidget = GetWidgetAtOverlay(ovl, ll.X, ll.Y, out hoveredMouseX, out hoveredMouseY);

                    if (HoveredWidget != null)
                    {
                        foundInOverlay = true;
                        break;
                    }
                }
            }

            found:

            if (!foundInOverlay)
            {
                HoveredWidget = GetControlAt(e.X, e.Y, out hoveredMouseX, out hoveredMouseY);
                //TopLevelWindow.Title = String.Format("e.X = {0}; e.Y = {1}", e.X, e.Y);
            }

            //WindowHUD.Title = "HoveredWidget = " + HoveredWidget;

            if (capturedMouseOvl == false)
            {
                if (MouseDown && FocusedWidget != null)
                {
                    MouseDownForClick = false;
                    CaptureMouseFocus();

                    Point winPoint = FocusedWidget.LocalToWindow();
                    var smp = new Point(e.X, e.Y);

                    int leftAreaX = winPoint.X;
                    int rightAreaX = winPoint.X + FocusedWidget.Width;

                    int upAreaY = winPoint.Y;
                    int downAreaY = winPoint.Y + FocusedWidget.Height;
                    Point localPoint = FocusedWidget.WindowToLocal(smp);

                    if (e.X <= leftAreaX)
                        mouseLocalX = e.X - leftAreaX;
                    else if (e.X >= rightAreaX)
                        mouseLocalX = FocusedWidget.Width + (e.X - rightAreaX);
                    else
                        mouseLocalX = localPoint.X;

                    if (e.Y <= upAreaY)
                        mouseLocalY = e.Y - upAreaY;
                    else if (e.Y >= downAreaY)
                        mouseLocalY = FocusedWidget.Height + (e.Y - downAreaY);
                    else
                        mouseLocalY = localPoint.Y;

                    //TopLevelWindow.Title = String.Format("mLocalX = {0}; mLocalY = {1}; GameWindowX = {2}; GameWindowY = {3}; MouseFocusX = {4}; MouseFocus = {5}  ",
                    //                                     mouseLocalX, mouseLocalY, e.X, e.Y, winPoint.X, MouseFocus);

                    nmea = new MouseEventArgs(e.Button, mouseLocalX, mouseLocalY, e.DeltaX, e.DeltaY);
                    FocusedWidget.OnMouseMove(nmea);

                    return;
                }

                // When the mouse coordinates are outside the application window or the main widget "WHUD" does not occupy the entire client area of the OpenTK window.
                if (HoveredWidget == null)
                {
                    //throw new Exception("WHUD.ProcessMouseMove(): This should not happen ...");

                    if (!MouseFocusCaptured)
                    {
                        if (LastHoveredWidget != null)
                        {
                            //LastHoveredWidget.OnMouseLeave(EventArgs.Empty);
                            LookUpNotifyMouseLeave(LastHoveredWidget, e.X, e.Y, BaseEventArgs.Empty);
                        }
                    }
                    return;
                }
                else
                {
                    if (LastHoveredWidget != HoveredWidget)
                    {
                        if (LastHoveredWidget != null)
                        {
                            //LastHoveredWidget.OnMouseLeave(EventArgs.Empty);
                            LookUpNotifyMouseLeave(LastHoveredWidget, e.X, e.Y, BaseEventArgs.Empty);
                        }

                        //HoveredWidget.OnMouseEnter(EventArgs.Empty);
                        LookUpNotifyMouseEnter(HoveredWidget, BaseEventArgs.Empty);
                    }
                }

                LastHoveredWidget = HoveredWidget;

                //Point localPoint = HoveredWidget.WindowToLocal(new Point(e.X, e.Y));

                nmea = new MouseEventArgs(e.Button, hoveredMouseX, hoveredMouseY, e.DeltaX, e.DeltaY);
                LookUpNotifyMouseMove(HoveredWidget, nmea);
                HoveredWidget.OnMouseMove(nmea);
            }
        }
        protected void ProcessMouseDown(MouseEventArgs e)
        {
            //WindowHUD.Title = "WHUD_MouseDown";

            if (WindowDialog != null)
            {
                if (!WindowDialog.Bounds.Contains(e.X, e.Y))
                    return;
            }

            MouseUpFromClick = false;
            MouseDownForClick = true;
            MouseDown = true;

            MouseDownEventArgs mea;

            // Can be 'null' by the overlays
            if (HoveredWidget != null)
            {
                clicks = 1;

                if (clickTimer.ElapsedMilliseconds > 0 &&
                    clickTimer.ElapsedMilliseconds < DoubleClick)
                {
                    clickTimer.Reset();
                    clicks = 2;
                }
                else
                {
                    clickTimer.Restart();
                }

                Point localPoint = HoveredWidget.WindowToLocal(new Point(e.X, e.Y));
                mea = new MouseDownEventArgs(e.Button, localPoint.X, localPoint.Y,
                                             e.DeltaX, e.DeltaY, e.DeltaWheel, clicks);

                LookUpNotifyMouseDown(HoveredWidget, mea);
                // 'HoveredWidget' gains focus if the implementation of 'HoveredWidget.OnMouseDown ()' called 'Focus ().
                HoveredWidget.OnMouseDown(mea);

                if (mea != null && mea.FocusedLostFocusOnMouseDown)
                {
                    if (FocusedWidget != null
                       && FocusedWidget != HoveredWidget)
                        // MouseFocus loses focus. 'MouseFocus = null'
                        FocusedWidget.Focus(true);
                }
            }
           // else
           //     throw new Exception("WHUD.ProcessMouseDown(): This should not happen ...");
        }

        Stopwatch clickTimer = new Stopwatch();
        public static int DoubleClick = 500;//ms
        int clicks = 0;

        protected void ProcessMouseUp(MouseEventArgs e)
        {
            //WindowHUD.Title = "WHUD_MouseUp";

            if (WindowDialog != null)
            {
                if (!WindowDialog.Bounds.Contains(e.X, e.Y))
                    return;
            }

            MouseUpFromClick = true;

            // Can be null by the Overlays
            if (HoveredWidget != null)
            {
                Point localPoint = HoveredWidget.WindowToLocal(new Point(e.X, e.Y));
                var mea = new MouseEventArgs(e.Button, localPoint.X, localPoint.Y,
                                             e.DeltaX, e.DeltaY, e.DeltaWheel, clicks);

                LookUpNotifyMouseUp(HoveredWidget, mea);

                if (LastHoveredWidget == HoveredWidget)
                {
                    if (MouseDownForClick)
                    {
                        HoveredWidget.OnMouseClick(mea);
                        if (mea.Clicks == 2)
                            HoveredWidget.OnMouseDoubleClick(mea);
                    }
                    HoveredWidget.OnMouseUp(mea);
                }
                else if (FocusedWidget != null)
                {
                    if (MouseDownForClick)
                    {
                        FocusedWidget.OnMouseClick(mea);
                        if (mea.Clicks == 2)
                            FocusedWidget.OnMouseDoubleClick(mea);
                    }
                    FocusedWidget.OnMouseUp(mea);
                }
            }

            ReleaseCapturedMouseFocus();
            MouseDown = false;
            MouseDownForClick = false;
        }

        /// <summary>
        /// MouseEnter from WHUD instances.
        /// </summary>
        /// <param name="ea"></param>
        protected void ProcessMouseEnter(EventArgs ea)
        {
            //WindowHUD.Title = "WHUD_MouseEnter";
            HoveredWidget = LastHoveredWidget = null;
        }

        /// <summary>
        /// MouseLeave from WHUD instances.
        /// </summary>
        /// <param name="e"></param>
        protected void ProcessMouseLeave(EventArgs e)
        {
            if (LastHoveredWidget != null)
            {
                LastHoveredWidget.OnMouseLeave(e);
            }
            HoveredWidget = LastHoveredWidget = null;
            //WindowHUD.Title = "WHUD_MouseLeave";
        }

        protected void ProcessMouseWheel(MouseEventArgs e)
        {
            if (HoveredWidget != null)
                HoveredWidget.OnMouseWheel(e);
        }

        protected void ProcessKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Alt)
                ModifierKeys |= Keys.Alt;
            if (e.KeyData == Keys.LControlKey)
                ModifierKeys |= Keys.Control;
            if (e.KeyData == Keys.LShiftKey)
                ModifierKeys |= Keys.Shift;

            if (FocusedWidget != null)
            {
                bool pdk = FocusedWidget.ProcessDialogKey(e.KeyData | ModifierKeys);
                if (!pdk)
                    FocusedWidget.OnKeyDown(e);
            }
        }

        protected void ProcessKeyPress(KeyPressedEventArgs e)
        {
            if (FocusedWidget != null)
                FocusedWidget.OnKeyPress(e);
        }

        protected void ProcessKeyUp(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Alt)
                ModifierKeys ^= Keys.Alt;
            if (e.KeyData == Keys.LControlKey)
                ModifierKeys ^= Keys.Control;
            if (e.KeyData == Keys.LShiftKey)
                ModifierKeys ^= Keys.Shift;
        }


            //public /*protected*/ virtual void ProcessUpdate()
            protected virtual void ProcessUpdate()
            {
            while (BackLayoutsToUpdate.Count > 0)
            {
                FrontLayoutsToUpdate = BackLayoutsToUpdate;
                BackLayoutsToUpdate = new LayoutList();

                if (FrontLayoutsToUpdate.Count > 0)
                {
                    foreach (LayoutBase ltu in FrontLayoutsToUpdate)
                    {
                        if (ltu.Owner.IsHide)
                            continue;

                        ltu.Owner.LayoutRunning = true;

                        var winHUD = ltu.Owner as WHUD;

                        if (winHUD != null)
                        {
                            foreach (Widget winw in winHUD.Overlays)
                                winw.OnBeforeParentLayout();
                        }

                        foreach (Widget w in ltu.Owner.Widgets)
                            w.OnBeforeParentLayout();

                        ltu.Owner.OnLayout();
                        ltu.DoLayout();
                        ltu.Owner.LayoutRunning = false;
                    }
                }
            }
        }
    }

    }
}
