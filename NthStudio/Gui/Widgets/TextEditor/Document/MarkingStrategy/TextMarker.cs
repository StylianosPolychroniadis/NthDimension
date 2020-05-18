using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor.Document.MarkingStrategy
{
    /// <summary>
    /// Marks a part of a document.
    /// </summary>
    public class TextMarker : AbstractSegment
    {
        TextMarkerType textMarkerType;
        Color color;
        Color foreColor;
        string toolTip = null;
        bool overrideForeColor = false;

        public TextMarkerType TextMarkerType
        {
            get
            {
                return textMarkerType;
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }
        }

        public Color ForeColor
        {
            get
            {
                return foreColor;
            }
        }

        public bool OverrideForeColor
        {
            get
            {
                return overrideForeColor;
            }
        }

        /// <summary>
        /// Marks the text segment as read-only.
        /// </summary>
        public bool IsReadOnly { get; set; }

        public string ToolTip
        {
            get
            {
                return toolTip;
            }
            set
            {
                toolTip = value;
            }
        }

        /// <summary>
        /// Gets the last offset that is inside the marker region.
        /// </summary>
        public int EndOffset
        {
            get
            {
                return Offset + Length - 1;
            }
        }

        public TextMarker(int offset, int length, TextMarkerType textMarkerType)
            : this(offset, length, textMarkerType, Color.Red)
        {
        }

        public TextMarker(int offset, int length, TextMarkerType textMarkerType, Color color)
        {
            if (length < 1)
                length = 1;
            this.offset = offset;
            this.length = length;
            this.textMarkerType = textMarkerType;
            this.color = color;
        }

        public TextMarker(int offset, int length, TextMarkerType textMarkerType, Color color, Color foreColor)
        {
            if (length < 1)
                length = 1;
            this.offset = offset;
            this.length = length;
            this.textMarkerType = textMarkerType;
            this.color = color;
            this.foreColor = foreColor;
            this.overrideForeColor = true;
        }
    }
}
