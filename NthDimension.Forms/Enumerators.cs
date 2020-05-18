using System;

namespace NthDimension.Forms
{
    public enum EColorDepth
    {
        Depth4Bit       = 4,
        Depth8Bit       = 8,
        Depth16Bit      = 16,
        Depth24Bit      = 24,
        Depth32Bit      = 32
    }

    public enum EDialogResult
    {
        Abort,
        Cancel,
        Ignore,
        No,
        None,
        Accept,
        Retry,
        Yes
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
    public enum EWinStartPosition
    {
        DefaultLocation,
        CenterScreen
    }
    [Flags]
    public enum ETextAlignment
    {
        None = 0,
        Left = 1 << 1,
        Right = 1 << 2,
        Top = 1 << 3,
        Bottom = 1 << 4,
        CenterV = 1 << 5,
        CenterH = 1 << 6,
        Center = CenterH | CenterV
    }
    [Flags]
    public enum EAnchorStyle
    {
        None = 0x0000,
        Top = 0x0001,
        Bottom = 0x0002,
        Left = 0x0004,
        Right = 0x0008,
        All = Top | Bottom | Left | Right
    };
    public enum ESplitterType
    {
        HorizontalScroll,
        VerticalScroll
    }
    /// <summary>
	/// Represents relative docking.
	/// </summary>
	public enum EDocking
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 3,
        Bottom = 4,
        Fill = 5
    }

    


}
