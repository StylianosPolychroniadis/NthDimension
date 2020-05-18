using System;
using System.Drawing;
using NthDimension;
using NthDimension.Forms;
using NthStudio.Gui.Widgets.TextEditor.Document;

namespace NthStudio.Gui.Widgets.TextEditor.Margins
{
    /// <summary>
    /// This class views the line numbers and folding markers.
    /// </summary>
    public abstract class AbstractMargin
    {
        NanoCursor cursor = Cursors.Default;

        protected Rectangle drawingPosition = new Rectangle(0, 0, 0, 0);
        protected TextCanvas iTextCanvas;

        protected AbstractMargin(TextCanvas pTextCanvas)
        {
            this.iTextCanvas = pTextCanvas;
        }

        public Rectangle DrawingPosition
        {
            get
            {
                return drawingPosition;
            }
            set
            {
                drawingPosition = value;
            }
        }

        public TextCanvas TextCanvas
        {
            get
            {
                return iTextCanvas;
            }
        }

        public IDocument Document
        {
            get
            {
                return iTextCanvas.Document;
            }
        }

        public ITextEditorProperties TextEditorProperties
        {
            get
            {
                return iTextCanvas.Document.TextEditorProperties;
            }
        }

        public virtual NanoCursor Cursor
        {
            get
            {
                return cursor;
            }
            set
            {
                cursor = value;
            }
        }

        public virtual Size Size
        {
            get
            {
                return new Size(-1, -1);
            }
        }

        public virtual bool IsVisible
        {
            get
            {
                return true;
            }
        }

        public virtual void HandleMouseDown(Point mousepos, MouseButton mouseButtons)
        {
            if (MouseDown != null)
            {
                MouseDown(this, mousepos, mouseButtons);
            }
        }
        public virtual void HandleMouseMove(Point mousepos, MouseButton mouseButtons)
        {
            if (MouseMove != null)
            {
                MouseMove(this, mousepos, mouseButtons);
            }
        }
        public virtual void HandleMouseLeave(EventArgs e)
        {
            if (MouseLeave != null)
            {
                MouseLeave(this, e);
            }
        }

        public virtual void Paint(GContext gc, Rectangle rect)
        {
            if (Painted != null)
            {
                Painted(this, gc, rect);
            }
        }

        public event MarginPaintEventHandler Painted;
        public event MarginMouseEventHandler MouseDown;
        public event MarginMouseEventHandler MouseMove;
        public event EventHandler MouseLeave;
    }
}
