using System.Drawing;
using System.Xml.Serialization;

namespace NthDimension.Forms.Widgets.CalcGrid
{
    public class SpreadsheetStyleSettings
    {
        [XmlIgnore]
        public Color ForeColor { get; set; }

        [XmlElement("ForeColor")]
        public int ForeColorARGB
        {
            get { return this.ForeColor.ToArgb(); }
            set { this.ForeColor = Color.FromArgb(value); }
        }

        [XmlIgnore]
        public Color BackColor { get; set; }

        [XmlElement("BackColor")]
        public int BackColorARGB
        {
            get { return this.BackColor.ToArgb(); }
            set { this.BackColor = Color.FromArgb(value); }
        }

        [XmlElement]
        public string FontFamily { get; set; }
        [XmlElement]
        public int FontSize { get; set; }
        [XmlElement]
        public bool Bold { get; set; }
        [XmlElement]
        public bool Italic { get; set; }
        [XmlElement]
        public bool Underline { get; set; }

        [XmlElement]
        public string Format { get; set; }

        //public object Value { get; set; } // Store ONLY style and NOT value???
    }
}
