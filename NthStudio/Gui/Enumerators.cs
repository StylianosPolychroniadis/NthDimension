using System;

namespace NthStudio.Gui
{
    public enum EAction
    {
        Loaded,
        Saved,
        SaveAs,
        TextChanged,
        SelectionON,
        SelectionOFF,
        Undo,
        Redo
    }

    #region TabStrip

    public enum CollectionChangeAction
    {
        Add = 1,
        Remove,
        Refresh
    }

    /// <summary>
    /// Hit test result of <see cref="FATabStrip"/>
    /// </summary>
    public enum HitTestResult
    {
        CloseButton,
        MenuGlyph,
        TabItem,
        None
    }

    /// <summary>
    /// Theme Type
    /// </summary>
    public enum ThemeTypes
    {
        WindowsXP,
        Office2000,
        Office2003
    }

    /// <summary>
    /// Indicates a change into TabStrip collection
    /// </summary>
    public enum TabStripItemChangeTypes
    {
        Added,
        Removed,
        Changed,
        SelectionChanged
    }
    #endregion TabStrip

    public enum EColorDepth
    {
        Depth4Bit = 4,
        Depth8Bit = 8,
        Depth16Bit = 16,
        Depth24Bit = 24,
        Depth32Bit = 32
    }

   
    public enum EItemsAlignment
    {
        Horizontal,
        Vertical
    }

    public enum EMenuDisplayStyle
    {
        Text,
        Image,
        ImageText
    }


    #region Theme

    // flags indicating which corners are sharp (for grouping widgets)
    public enum BNDcornerFlags
    {
        // all corners are round
        BND_CORNER_NONE = 0,
        // sharp top left corner
        BND_CORNER_TOP_LEFT = 1,
        // sharp top right corner
        BND_CORNER_TOP_RIGHT = 2,
        // sharp bottom right corner
        BND_CORNER_DOWN_RIGHT = 4,
        // sharp bottom left corner
        BND_CORNER_DOWN_LEFT = 8,
        // all corners are sharp;
        // you can invert a set of flags using ^= BND_CORNER_ALL
        BND_CORNER_ALL = 0xF,
        // top border is sharp
        BND_CORNER_TOP = 3,
        // bottom border is sharp
        BND_CORNER_DOWN = 0xC,
        // left border is sharp
        BND_CORNER_LEFT = 9,
        // right border is sharp
        BND_CORNER_RIGHT = 6
    }

    // states altering the styling of a widget
    public enum BNDwidgetState
    {
        // not interacting
        BND_DEFAULT = 0,
        // the mouse is hovering over the control
        BND_HOVER,
        // the widget is activated (pressed) or in an active state (toggled)
        BND_ACTIVE
    }

    #endregion Theme

    public enum EWinStartPosition
    {
        DefaultLocation,
        CenterScreen
    }
   

  

    #region Scrolls

    /// <summary>
    /// Description of enums.
    /// </summary>
    public enum EScrollOrientation
    {
        Horizontal,
        Vertical
    }

    public enum EButtonState
    {
        NotFocused,
        Focused,
        MouseHover,
        Pressed
    }

    public enum EButtonStyle
    {
        Circular = 0,
        Rectangular = 1,
        Elliptical = 2,
    }

    public enum ScrollEventType
    {
        SmallDecrement,
        SmallIncrement,
        LargeDecrement,
        LargeIncrement,
        ThumbPosition,
        ThumbTrack,
        First,
        Last,
        EndScroll
    }
    #endregion Scrolls
}
