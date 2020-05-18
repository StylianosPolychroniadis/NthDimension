/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

using NthDimension.Forms.Events;
using NthDimension.Forms.Layout;
using System;
using System.Drawing;

namespace NthDimension.Forms.Delegates
{
    #region Old Implementation
    public delegate void RunOnNextPaintUpdate();

    public delegate void ResizeHandler(object sender, EventArgs e);
    public delegate void PaintHandler(object sender, PaintEventArgs e);

    public delegate void HideChangedHandler(object sender, EventArgs ea);
    public delegate void SizeChangedHandler(object sender, EventArgs ea);

    public delegate void KeyDownHandler(object sender, KeyEventArgs e);
    public delegate void KeyPressHandler(object sender, KeyPressedEventArgs e);
    public delegate void KeyUpHandler(object sender, KeyEventArgs e);

    public delegate void WidgetAddHandler(Widget widgetAdded);
    public delegate void WidgetRemoveHandler(Widget widgetRemoved);

    public delegate void LayoutAddHandler(LayoutBase layoutAdded);
    public delegate void LayoutRemoveHandler(LayoutBase layoutRemoved);

    public delegate void MouseClickHandler(object sender, MouseEventArgs mea);
    public delegate void MouseDoubleClickHandler(object sender, MouseEventArgs mea);
    public delegate void MouseDownHandler(Widget sender, MouseDownEventArgs mea);
    public delegate void MouseHandler(object sender, MouseEventArgs mea);
    public delegate void MouseUpHandler(object sender, MouseEventArgs mea);
    public delegate void MouseMoveHandler(object sender, MouseEventArgs mea);
    public delegate void MouseEnterHandler(object sender, EventArgs e);
    public delegate void MouseLeaveHandler(object sender, EventArgs e);

    public delegate void MouseWheelHandler(object sender, MouseEventArgs e);                // SP Jan-08-18

    public delegate void TitleChangedHandler(string text);

    public delegate void ScrollEventHandler(object sender, ScrollEventArgs e);

    public delegate void MouseCursorChangedHandler(object sender, MouseCursorChangedEventArgs args);

    public delegate void MarginMouseEventHandler(AbstractMargin sender, Point mousepos, MouseButton mouseButtons);
    public delegate void MarginPaintEventHandler(AbstractMargin sender, GContext gc, Rectangle rect);

    public delegate void ItemClickedHandler(object sender, EventArgs ea);

    #endregion Old Implementation

}
