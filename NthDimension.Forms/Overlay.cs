using NthDimension.Forms.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms
{
    /// <summary>
    /// Description of Overlay.
    /// </summary>
    /// 

    public enum enuBorderStyle
    {
        Fixed,
        Sizable
    }

    public class Overlay : Widget
    {
        public enuBorderStyle       BorderStyle = enuBorderStyle.Fixed;
        //public bool                 DownForResize
        //{
        //    get;
        //    /*private*/ set;
        //}

        private WHUD window;
        private int oldMouseMoveX = 0;
        private int oldMouseMoveY = 0;

        public Overlay()
        {

            IsHide = true;

        }

        // Evita estar llamando a 'win.Overlays.Contains(this)'
        // cada vez que se muestra el menú.
        bool ovlRegistered = false;

        protected void RegisterOverlay(WHUD win)
        {
            if (null == window)
                window = win;

            if (!ovlRegistered
                && !win.Overlays.Contains(this))
            {
                ovlRegistered = true;
                win.Overlays.Add(this);
                win.OverlaysToOnShowDialog.Add(this);

                win.OverlayCurrent = this;

                //In this point, 'this.Parent' is equal to 'HUD' instance.
                ParentPerformLayout();
            }

            win.OverlayCurrent = this;

        }

        public virtual void Show(WHUD win)
        {
            /*var wf = FocusedWidget as ContextMenuStrip;

			if (wf != null)
				return;*/

            if (null == window)
                window = win;

            RegisterOverlay(win);
            Focus();
            Show();

            if (FocusedWidget != this)
            {
                Rendering.Utilities.ConsoleUtil.errorlog(string.Format("Overlay {0}", Name), " Focused Widget != this at Overlay.Show(WHUD)");
                //throw new Exception();
            }
        }

        public virtual void OnShowDialog()
        {
        }


        public virtual void BringToFront(WHUD win)
        {
            Overlay ov = this;

            win.Overlays.Remove(ov);
            win.Overlays.Insert(win.Overlays.Count, ov);


        }

        public virtual void SendToBack(WHUD win)
        {
            Overlay ov = this;

            win.Overlays.Remove(ov);
            win.Overlays.Insert(0, ov);
        }

        protected override void OnGotFocus(EventArgs e)                     // Added Jun-06-18
        {
            base.OnGotFocus(e);

            RegisterOverlay(window);

        }

        protected override void OnMouseEnter(EventArgs e)                   // Added Jun-06-18
        {
            base.OnMouseEnter(e);

            RegisterOverlay(window);            
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (BorderStyle == enuBorderStyle.Sizable)                      // Added Jan-13-2020
            {
                int x = e.X;
                int y = e.Y;

                bool resizeH = (x >= this.Left - 5 && x <= this.Left + 5) ||
                               (x >= this.Right - 5 && x <= this.Right + 5);
                bool resizeV = (y >= this.Top - 5 && y <= this.Top + 15) ||
                               (y >= this.Bottom - 5 && y <= this.Bottom + 5);
                bool resizeAll = resizeH && resizeV;

                if (resizeAll)
                {
                    if (Cursors.Cursor != Cursors.NW)
                        Cursors.Cursor = Cursors.NW;
                }
                else if (resizeH)
                {
                    if (Cursors.Cursor != Cursors.SE)
                        Cursors.Cursor = Cursors.SE;
                }
                else if (resizeV)
                {
                    if (Cursors.Cursor != Cursors.SW)
                        Cursors.Cursor = Cursors.SW;
                }
                else
                {
                    if (Cursors.Cursor != Cursors.Default)
                        Cursors.Cursor = Cursors.Default;
                }

                if (e.Button == MouseButton.Left &&
                    (resizeAll || resizeH || resizeV))
                {
                    this.Size = new Size(this.Size.Width + x - oldMouseMoveX, this.Size.Height + y - oldMouseMoveY);
                    Repaint();
                }

                oldMouseMoveX = x;
                oldMouseMoveY = y;
            }

            base.OnMouseMove(e);
        }
    }
}
