using System;
using System.Drawing;

namespace NthDimension.Forms.Dialogs
{
    /// <summary>
	/// Description of DialogBase.
	/// </summary>
	public abstract class DialogBase : Window
    {
        protected DialogBase()
        {
            IsDialog = true;
            Size = new Size(800, 600);
        }

        public override void Show(Widget.WHUD win)
        {

            if (WindowDialog != null)
                if (WindowDialog.IsHide)
                    throw new Exception("WindowDialog active, not Show");

            base.Show(win);

            WindowDialog = this;

            if (Parent != null)
            {
                int locX = (Parent.Width / 2) - (Width / 2);
                int locY = (Parent.Height / 2) - (Height / 2);
                Location = new Point(locX, locY);
            }
        }

        public override void Hide()
        {
            base.Hide();

            WindowDialog = null;
        }

        public abstract ImageList ImgList
        {
            get;
            set;
        }
    }
}
