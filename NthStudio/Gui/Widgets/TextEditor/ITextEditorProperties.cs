using NthDimension.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor
{
    public interface ITextEditorProperties
    {
        #region Fonts

        /// <value>
        /// The scaled, regular version of the base font
        /// </value>
        NanoFont RegularFont
        {
            get;
        }

        /// <value>
        /// The scaled, bold version of the base font
        /// </value>
        NanoFont BoldFont
        {
            get;
        }

        /// <value>
        /// The scaled, italic version of the base font
        /// </value>
        NanoFont ItalicFont
        {
            get;
        }

        /// <value>
        /// The scaled, bold/italic version of the base font
        /// </value>
        NanoFont BoldItalicFont
        {
            get;
        }

        /// <value>
        /// The base font
        /// </value>
        NanoFont DefaultFont
        {
            get;
        }

        #endregion Fonts

        bool AllowCaretBeyondEOL
        {
            get;
            set;
        }

        /// <summary>
        /// The width of a tab.
        /// </summary>
        int TabIndent
        { // is wrapped in text editor control
            get;
            set;
        }

        bool CaretLine
        {
            get;
            set;
        }

        LineViewerStyle LineViewerStyle
        { // is wrapped in text editor control
            get;
            set;
        }


        bool EnableFolding
        { // is wrapped in text editor control
            get;
            set;
        }

        bool ShowLineNumbers
        { // is wrapped in text editor control
            get;
            set;
        }

        bool IsIconBarVisible
        { // is wrapped in text editor control
            get;
            set;
        }

        bool SupportReadOnlySegments
        {
            get;
            set;
        }

        bool ShowInvalidLines
        { // is wrapped in text editor control
            get;
            set;
        }

        bool ShowVerticalRuler
        { // is wrapped in text editor control
            get;
            set;
        }


        BracketMatchingStyle BracketMatchingStyle
        { // is wrapped in text editor control
            get;
            set;
        }

        IndentStyle IndentStyle
        { // is wrapped in text editor control
            get;
            set;
        }

        int MouseWheelScrollLines
        {
            get;
            set;
        }

        bool MouseWheelScrollDown
        {
            get;
            set;
        }

        int VerticalRulerRow
        { // is wrapped in text editor control
            get;
            set;
        }

        NanoFont Font
        { // is wrapped in text editor control
            get;
            //set;
        }

        Encoding Encoding
        {
            get;
            set;
        }

        bool ShowSpaces
        { // is wrapped in text editor control
            get;
            set;
        }

        bool ShowTabs
        { // is wrapped in text editor control
            get;
            set;
        }

        bool ShowEOLMarker
        { // is wrapped in text editor control
            get;
            set;
        }

        bool ShowHorizontalRuler
        { // is wrapped in text editor control
            get;
            set;
        }

        bool ShowMatchingBracket
        { // is wrapped in text editor control
            get;
            set;
        }

        bool ConvertTabsToSpaces
        { // is wrapped in text editor control
            get;
            set;
        }

        bool HideMouseCursor
        { // is wrapped in text editor control
            get;
            set;
        }

        string LineTerminator
        {
            get;
            set;
        }

        DocumentSelectionMode DocumentSelectionMode
        {
            get;
            set;
        }

        /// <summary>
        /// The amount of spaces a tab is converted to if ConvertTabsToSpaces is true.
        /// </summary>
        int IndentationSize
        {
            get;
            set;
        }

    }
}
