using NthDimension.Forms.Events;
using System;

namespace NthDimension.Forms.Widgets
{
    internal class LabelEditTextBox : TextField
    {

        public LabelEditTextBox(string text)
            : base(text)
        {
        }

        protected bool IsInputKey(Keys key_data)
        {
            if ((key_data & Keys.Alt) == 0)
            {
                switch (key_data & Keys.KeyCode)
                {
                    case Keys.Enter:
                        return true;
                    case Keys.Escape:
                        return true;
                }
            }
            return false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!VisibleAfterClipping)
                return;

            switch (e.KeyCode)
            {
                case Keys.Return:
                    IsHide = true;
                    Parent.Focus();
                    e.Handled = true;
                    OnEditingFinished(e);
                    break;
                case Keys.Escape:
                    IsHide = true;
                    Parent.Focus();
                    e.Handled = true;
                    OnEditingCancelled(e);
                    break;
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (VisibleAfterClipping)
            {
                OnEditingFinished(e);
            }
        }

        protected void OnEditingCancelled(EventArgs e)
        {
            if (EditingCancelledEvent != null)
                EditingCancelledEvent(this, e);
        }

        protected void OnEditingFinished(EventArgs e)
        {
            if (EditingFinishedEvent != null)
                EditingFinishedEvent(this, e);
        }

        public event EventHandler EditingCancelledEvent;

        public event EventHandler EditingFinishedEvent;
    }
}
