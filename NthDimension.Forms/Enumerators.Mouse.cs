namespace NthDimension.Forms
{




    public enum ECursors
    {
        Default,
        VSplit,
        HSplit,
        Hand
    }

    /// <summary>
	/// Description of enums.
	/// </summary>
	public enum EScrollOrientation
    {
        Horizontal,
        Vertical
    }

    public enum EScrollButtonState
    {
        NotFocused,
        Focused,
        MouseHover,
        Pressed
    }

    public enum EScrollButtonStyle
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
}
