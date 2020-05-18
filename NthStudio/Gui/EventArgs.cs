using NthDimension.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui
{
    //public class BaseEventArgs : EventArgs
    //{
    //    public bool IsNotification
    //    {
    //        get;
    //        internal set;
    //    }

    //    //
    //    // Static Fields
    //    //
    //    public static readonly new BaseEventArgs Empty;

    //    static BaseEventArgs()
    //    {
    //        Empty = new BaseEventArgs();
    //    }
    //}

    //public class HandledEventArgs : EventArgs
    //{
    //    //
    //    // Properties
    //    //
    //    public bool Handled
    //    {
    //        get;
    //        set;
    //    }

    //    //
    //    // Constructors
    //    //
    //    public HandledEventArgs()
    //    {
    //    }

    //    public HandledEventArgs(bool defaultHandledValue)
    //    {
    //        Handled = defaultHandledValue;
    //    }
    //}

    //public class MouseButtonEventArgs : MouseEventArgs
    //{
    //    public bool IsPressed
    //    {
    //        get;
    //        internal set;
    //    }

    //    //
    //    // Constructors
    //    //
    //    public MouseButtonEventArgs()
    //        : base(EMouseButtons.None, 0, 0, 0, 0)
    //    {
    //        IsPressed = false;
    //    }

    //    public MouseButtonEventArgs(int x, int y, EMouseButtons button, bool pressed)
    //        : base(button, x, y, 0, 0)
    //    {
    //        IsPressed = pressed;
    //    }

    //    public MouseButtonEventArgs(MouseButtonEventArgs args)
    //        : this(args.X, args.Y, args.Button, args.IsPressed)
    //    {
    //    }
    //}

    //public class MouseEventArgs : BaseEventArgs
    //{
    //    public EMouseButtons Button
    //    {
    //        get;
    //        private set;
    //    }

    //    public int Clicks
    //    {
    //        get;
    //        private set;
    //    }

    //    public int DeltaWheel
    //    {
    //        get;
    //        private set;
    //    }

    //    public int DeltaX
    //    {
    //        get;
    //        private set;
    //    }

    //    public int DeltaY
    //    {
    //        get;
    //        private set;
    //    }

    //    public Point Location
    //    {
    //        get
    //        {
    //            return new Point(X, Y);
    //        }
    //    }

    //    public int X
    //    {
    //        get;
    //        private set;
    //    }

    //    public int Y
    //    {
    //        get;
    //        private set;
    //    }

    //    //
    //    // Constructors
    //    /// <summary>
    //    /// X y Y, posición del puntero del ratón en coordenadas del Widget
    //    /// </summary>
    //    /// <param name="button">Button.</param>
    //    /// <param name="x">The x coordinate.</param>
    //    /// <param name="y">The y coordinate.</param>
    //    /// <param name="deltaX">Delta x.</param>
    //    /// <param name="deltaY">Delta y.</param>
    //    /// <param name = "deltaWheel"></param>
    //    /// <param name = "clicks"></param>
    //    public MouseEventArgs(EMouseButtons button, int x, int y, int deltaX,
    //                          int deltaY, int deltaWheel = 0, int clicks = 1)
    //    {
    //        this.Clicks = clicks;
    //        this.Button = button;
    //        this.DeltaX = deltaX;
    //        this.DeltaY = deltaY;
    //        this.X = x;
    //        this.Y = y;
    //        this.DeltaWheel = deltaWheel;
    //    }

    //    public MouseEventArgs(MouseDownEventArgs mea)
    //                : this(mea.Button, mea.X, mea.Y,
    //               mea.DeltaX, mea.DeltaY, mea.DeltaWheel, mea.Clicks)

    //    {
    //    }

    //    public MouseEventArgs(MouseEventArgs mea)
    //                : this(mea.Button, mea.X, mea.Y,
    //               mea.DeltaX, mea.DeltaY, mea.DeltaWheel, mea.Clicks)

    //    {
    //    }
    //}

    //public class MouseDownEventArgs : MouseEventArgs
    //{
    //    /// <summary>
    //    /// El Widget que tiene el foco lo pierde si se pulsa en otro Widget.
    //    /// Por defecto es 'true'.
    //    /// </summary>
    //    public bool FocusedLostFocusOnMouseDown
    //    {
    //        get;
    //        set;
    //    }

    //    public MouseDownEventArgs(EMouseButtons button, int x, int y, int deltaX, int deltaY,
    //                              int deltaWheel = 0, int clicks = 1)
    //        : base(button, x, y, deltaX, deltaY, deltaWheel, clicks)
    //    {
    //        FocusedLostFocusOnMouseDown = true;
    //    }
    //}

    //public class ScrollEventArgs : EventArgs
    //{
    //    //
    //    // Properties
    //    //
    //    public int NewValue
    //    {
    //        get;
    //        set;
    //    }

    //    public int OldValue
    //    {
    //        get;
    //        private set;
    //    }

    //    public EScrollOrientation ScrollOrientation
    //    {
    //        get;
    //        private set;
    //    }

    //    public ScrollEventType Type
    //    {
    //        get;
    //        private set;
    //    }

    //    //
    //    // Constructors
    //    //
    //    public ScrollEventArgs(ScrollEventType type, int newValue)
    //    {
    //    }

    //    public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue)
    //    {
    //    }

    //    public ScrollEventArgs(ScrollEventType type, int newValue, EScrollOrientation scroll)
    //    {
    //    }

    //    public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue, EScrollOrientation scroll)
    //    {
    //    }
    //}

    //public class PaintEventArgs : EventArgs
    //{
    //    //
    //    // Properties
    //    //
    //    public Rectangle ClipRect
    //    {
    //        get;
    //        private set;
    //    }

    //    public GContext GC
    //    {
    //        get;
    //        private set;
    //    }

    //    //
    //    // Constructors
    //    //

    //    public PaintEventArgs(GContext gc)
    //        : this(gc, Rectangle.Empty)
    //    {
    //    }

    //    public PaintEventArgs(GContext gc, Rectangle rect)
    //    {
    //        this.GC = gc;
    //        ClipRect = rect;
    //    }
    //}

    //public class MouseCursorChangedEventArgs : EventArgs
    //{
    //    public NanoCursor NewCursor;
    //    public MouseCursorChangedEventArgs(NanoCursor newCursor) : base()
    //    {
    //        NewCursor = newCursor;
    //    }
    //}

    //public class GotFocusEventArgs : EventArgs
    //{
    //    public GContext gc;

    //    public GotFocusEventArgs(GContext gc)
    //    {
    //        this.gc = gc;
    //    }
    //}

    //public class KeyPressedEventArgs : EventArgs
    //{
    //    //
    //    // Properties
    //    //
    //    public bool Handled
    //    {
    //        get;
    //        set;
    //    }

    //    public char KeyChar
    //    {
    //        get;
    //        set;
    //    }

    //    //
    //    // Constructors
    //    //
    //    public KeyPressedEventArgs(char keyChar)
    //    {
    //        this.KeyChar = keyChar;
    //    }
    //}

    //public class KeyEventArgs : EventArgs
    //{
    //    //
    //    // Properties
    //    //
    //    public virtual bool Alt
    //    {
    //        get;
    //        private set;
    //    }

    //    public bool Control
    //    {
    //        get;
    //        private set;
    //    }

    //    public bool Handled
    //    {
    //        get;
    //        set;
    //    }

    //    public Keys KeyCode
    //    {
    //        get;
    //        private set;
    //    }

    //    public Keys KeyData
    //    {
    //        get;
    //        private set;
    //    }

    //    public int KeyValue
    //    {
    //        get;
    //        private set;
    //    }

    //    public Keys Modifiers
    //    {
    //        get;
    //        private set;
    //    }

    //    public virtual bool Shift
    //    {
    //        get;
    //        private set;
    //    }

    //    public bool SuppressKeyPress
    //    {
    //        get;
    //        set;
    //    }

    //    //
    //    // Constructors
    //    //
    //    public KeyEventArgs(Keys keyData)
    //    {
    //        this.KeyData = keyData;
    //    }
    //}

    
}
