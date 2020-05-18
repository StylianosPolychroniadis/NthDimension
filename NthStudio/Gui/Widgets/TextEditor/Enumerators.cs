namespace NthStudio.Gui.Widgets.TextEditor
{
    /// <summary>
    /// In this enumeration are all caret modes listed.
    /// </summary>
    public enum CaretMode
    {
        /// <summary>
        /// If the caret is in insert mode typed characters will be
        /// inserted at the caret position
        /// </summary>
        InsertMode,

        /// <summary>
        /// If the caret is in overwirte mode typed characters will
        /// overwrite the character at the caret position
        /// </summary>
        OverwriteMode
    }

    /// <summary>
    /// This enum describes all implemented request types
    /// </summary>
    public enum TextAreaUpdateType
    {
        WholeTextArea,
        SingleLine,
        SinglePosition,
        PositionToLineEnd,
        PositionToEnd,
        LinesBetween
    }

    /// <summary>
    /// Describes the selection mode of the text area
    /// </summary>
    public enum DocumentSelectionMode
    {
        /// <summary>
        /// The 'normal' selection mode.
        /// </summary>
        Normal,

        /// <summary>
        /// Selections will be added to the current selection or new
        /// ones will be created (multi-select mode)
        /// </summary>
        Additive
    }

    /// <summary>
    /// Describes the caret marker
    /// </summary>
    public enum LineViewerStyle
    {
        /// <summary>
        /// No line viewer will be displayed
        /// </summary>
        None,

        /// <summary>
        /// The row in which the caret is will be marked
        /// </summary>
        FullRow
    }

    public enum FoldType
    {
        Unspecified,
        MemberBody,
        Region,
        TypeBody
    }

    public enum TextMarkerType
    {
        Invisible,
        SolidBlock,
        Underlined,
        WaveLine
    }

    public enum BracketMatchingStyle
    {
        Before,
        After
    }

    /// <summary>
    /// Describes the indent style
    /// </summary>
    public enum IndentStyle
    {
        /// <summary>
        /// No indentation occurs
        /// </summary>
        None,

        /// <summary>
        /// The indentation from the line above will be
        /// taken to indent the curent line
        /// </summary>
        Auto,

        /// <summary>
        /// Inteligent, context sensitive indentation will occur
        /// </summary>
        Smart
    }

    public enum TextWordType
    {
        Word,
        Space,
        Tab
    }
}
