using NthDimension.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor
{
    /// <summary>
    /// Deafult properties for TextEditor
    /// </summary>
    public class DefaultProperties : ITextEditorProperties
    {
        NanoFont defaultFont;
        NanoFont regularfont, boldfont, italicfont, bolditalicfont;

        public DefaultProperties()
        {
            regularfont             = NanoFont.DefaultRegular;
            defaultFont             = regularfont;
            boldfont                = NanoFont.DefaultRegularBold;
            italicfont              = NanoFont.DefaultItalic;
            bolditalicfont          = NanoFont.DefaultItalicBold;
        }

        #region Fonts

        public void SetFonts(NanoFont regular, NanoFont regularBold, NanoFont italic, NanoFont italicBold)
        {
            defaultFont = regular;
            regularfont = regular;
            boldfont = regularBold;
            italicfont = italic;
            bolditalicfont = italicBold;
        }

        /// <value>
        /// The scaled, regular version of the base font
        /// </value>
        public NanoFont RegularFont
        {
            get
            {
                return regularfont;
            }
        }

        /// <value>
        /// The scaled, bold version of the base font
        /// </value>
        public NanoFont BoldFont
        {
            get
            {
                return boldfont;
            }
        }

        /// <value>
        /// The scaled, italic version of the base font
        /// </value>
        public NanoFont ItalicFont
        {
            get
            {
                return italicfont;
            }
        }

        /// <value>
        /// The scaled, bold/italic version of the base font
        /// </value>
        public NanoFont BoldItalicFont
        {
            get
            {
                return bolditalicfont;
            }
        }

        /// <value>
        /// The base font
        /// </value>
        public NanoFont DefaultFont
        {
            get
            {
                return defaultFont;
            }
        }

        #endregion Fonts

        bool allowCaretBeyondEOL = false;

        public bool AllowCaretBeyondEOL
        {
            get
            {
                return allowCaretBeyondEOL;
            }
            set
            {
                allowCaretBeyondEOL = value;
            }
        }

        int tabIndent = 4;

        public int TabIndent
        {
            get
            {
                return tabIndent;
            }
            set
            {
                tabIndent = value;
            }
        }

        bool caretLine = false;

        public bool CaretLine
        {
            get
            {
                return caretLine;
            }
            set
            {
                caretLine = value;
            }
        }

        LineViewerStyle lineViewerStyle = LineViewerStyle.FullRow;

        public LineViewerStyle LineViewerStyle
        {
            get
            {
                return lineViewerStyle;
            }
            set
            {
                lineViewerStyle = value;
            }
        }

        bool enableFolding = true;

        public bool EnableFolding
        {
            get
            {
                return enableFolding;
            }
            set
            {
                enableFolding = value;
            }
        }

        bool showLineNumbers = true;

        public bool ShowLineNumbers
        {
            get
            {
                return showLineNumbers;
            }
            set
            {
                showLineNumbers = value;
            }
        }

        bool isIconBarVisible = true;

        public bool IsIconBarVisible
        {
            get
            {
                return isIconBarVisible;
            }
            set
            {
                isIconBarVisible = value;
            }
        }

        bool supportReadOnlySegments = false;

        public bool SupportReadOnlySegments
        {
            get
            {
                return supportReadOnlySegments;
            }
            set
            {
                supportReadOnlySegments = value;
            }
        }

        bool showInvalidLines = false;

        public bool ShowInvalidLines
        {
            get
            {
                return showInvalidLines;
            }
            set
            {
                showInvalidLines = value;
            }
        }

        bool showVerticalRuler = true;

        public bool ShowVerticalRuler
        {
            get
            {
                return showVerticalRuler;
            }
            set
            {
                showVerticalRuler = value;
            }
        }

        BracketMatchingStyle bracketMatchingStyle = BracketMatchingStyle.After;

        public BracketMatchingStyle BracketMatchingStyle
        {
            get
            {
                return bracketMatchingStyle;
            }
            set
            {
                bracketMatchingStyle = value;
            }
        }

        IndentStyle indentStyle = IndentStyle.Smart;

        public IndentStyle IndentStyle
        {
            get
            {
                return indentStyle;
            }
            set
            {
                indentStyle = value;
            }
        }

        int MouseWheelScrollLines_ = 3;

        public int MouseWheelScrollLines
        {
            get { return MouseWheelScrollLines_; }
            set { MouseWheelScrollLines_ = value; }
        }

        bool mouseWheelScrollDown = true;

        public bool MouseWheelScrollDown
        {
            get
            {
                return mouseWheelScrollDown;
            }
            set
            {
                mouseWheelScrollDown = value;
            }
        }

        int verticalRulerRow = 80;

        public int VerticalRulerRow
        {
            get
            {
                return verticalRulerRow;
            }
            set
            {
                verticalRulerRow = value;
            }
        }

        public NanoFont Font
        {
            get
            {
                return DefaultFont;
            }
            /*set
			{
				DefaultFont = value;
			}*/
        }

        Encoding encoding = System.Text.Encoding.UTF8;

        public Encoding Encoding
        {
            get
            {
                return encoding;
            }
            set
            {
                encoding = value;
            }
        }

        bool showSpaces = false;

        public bool ShowSpaces
        {
            get
            {
                return showSpaces;
            }
            set
            {
                showSpaces = value;
            }
        }

        bool showTabs = false;

        public bool ShowTabs
        {
            get
            {
                return showTabs;
            }
            set
            {
                showTabs = value;
            }
        }

        bool showEOLMarker = false;

        public bool ShowEOLMarker
        {
            get
            {
                return showEOLMarker;
            }
            set
            {
                showEOLMarker = value;
            }
        }

        bool showHorizontalRuler = false;

        public bool ShowHorizontalRuler
        {
            get
            {
                return showHorizontalRuler;
            }
            set
            {
                showHorizontalRuler = value;
            }
        }

        bool showMatchingBracket = true;

        public bool ShowMatchingBracket
        {
            get
            {
                return showMatchingBracket;
            }
            set
            {
                showMatchingBracket = value;
            }
        }

        bool convertTabsToSpaces = false;

        public bool ConvertTabsToSpaces
        {
            get
            {
                return convertTabsToSpaces;
            }
            set
            {
                convertTabsToSpaces = value;
            }
        }

        bool hideMouseCursor = false;

        public bool HideMouseCursor
        {
            get
            {
                return hideMouseCursor;
            }
            set
            {
                hideMouseCursor = value;
            }
        }

        string lineTerminator = "\r\n";

        public string LineTerminator
        {
            get
            {
                return lineTerminator;
            }
            set
            {
                lineTerminator = value;
            }
        }

        DocumentSelectionMode documentSelectionMode = DocumentSelectionMode.Normal;

        public DocumentSelectionMode DocumentSelectionMode
        {
            get
            {
                return documentSelectionMode;
            }
            set
            {
                documentSelectionMode = value;
            }
        }

        int indentationSize = 4;

        public int IndentationSize
        {
            get { return indentationSize; }
            set { indentationSize = value; }
        }

    }
}
