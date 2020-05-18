﻿using NthDimension.Forms;
using System;
using System.ComponentModel;
using System.Drawing;

namespace NthStudio.Gui.Widgets.TabStrip
{
    public interface ICaptionSupport
    {
        string Caption
        {
            get;
        }
    }

    public class TabStripItem : Widget
    {
        #region Events

        public event EventHandler Changed;

        #endregion

        #region Fields

        //private DrawItemState drawState = DrawItemState.None;
        private RectangleF stripRect = Rectangle.Empty;
        private Image image = null;
        private bool canClose = true;
        private bool selected = false;
        private bool visible = true;
        private bool isDrawn = false;
        private string title = string.Empty;

        #endregion

        #region Props

        string TabPageToolTipText_;
        [Browsable(false)]
        public string TabPageToolTipText
        {
            get { return TabPageToolTipText_; }
            set
            {
                TabPageToolTipText_ = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Size Size
        {
            get { return base.Size; }
            set { base.Size = value; }
        }

        [DefaultValue(true)]
        public bool IsVisible
        {
            get { return visible; }
            set
            {
                if (visible == value)
                    return;

                visible = value;
                OnChanged();
            }
        }

        internal RectangleF StripRect
        {
            get { return stripRect; }
            set { stripRect = value; }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsDrawn
        {
            get { return isDrawn; }
            set
            {
                if (isDrawn == value)
                    return;

                isDrawn = value;
            }
        }

        /// <summary>
        /// Image of <see cref="TabStripItem"/> which will be displayed
        /// on menu items.
        /// </summary>
        [DefaultValue(null)]
        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        [DefaultValue(true)]
        public bool CanClose
        {
            get { return canClose; }
            set { canClose = value; }
        }

        [DefaultValue("Name")]
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (title == value)
                    return;

                title = value;
                OnChanged();
            }
        }

        /// <summary>
        /// Gets and sets a value indicating if the page is selected.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false)]
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected == value)
                    return;

                selected = value;
            }
        }

        [Browsable(false)]
        public string Caption
        {
            get { return Title; }
        }

        #endregion

        #region Ctor

        public TabStripItem()
            : this(string.Empty, null)
        {
        }

        public TabStripItem(Label displayControl)
            : this(string.Empty, displayControl)
        {
        }

        public TabStripItem(string caption, Label displayControl)
        {
            /*SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ContainerControl, true);
            */
            selected = false;
            IsVisible = true;

            UpdateText(caption, displayControl);

            //Add to controls
            if (displayControl != null)
                Widgets.Add(displayControl);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Handles proper disposition of the tab page control.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (image != null)
                    image.Dispose();
            }
        }

        #endregion

        #region ShouldSerialize

        public bool ShouldSerializeIsDrawn()
        {
            return false;
        }

        public bool ShouldSerializeDock()
        {
            return false;
        }

        public bool ShouldSerializeControls()
        {
            return Widgets != null && Widgets.Count > 0;
        }

        public bool ShouldSerializeVisible()
        {
            return true;
        }

        #endregion

        #region Methods

        private void UpdateText(string caption, Label displayControl)
        {
            if (displayControl != null && displayControl is ICaptionSupport)
            {
                ICaptionSupport capControl = displayControl as ICaptionSupport;
                Title = capControl.Caption;
            }
            else if (caption.Length <= 0 && displayControl != null)
            {
                Title = displayControl.Text;
            }
            else if (caption != null)
            {
                Title = caption;
            }
            else
            {
                Title = string.Empty;
            }
        }

        public string Text
        {
            get;
            set;
        }

        public void Assign(TabStripItem item)
        {
            IsVisible = item.IsVisible;
            Text = item.Text;
            CanClose = item.CanClose;
            Tag = item.Tag;
        }

        protected internal virtual void OnChanged()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Return a string representation of page.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Caption;
        }

        #endregion
    }
}
