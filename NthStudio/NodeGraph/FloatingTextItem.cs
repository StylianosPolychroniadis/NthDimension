using System;
using System.Drawing;

namespace NthStudio.NodeGraph
{
    public class FloatingTextItem
    {
        public enum NodeAlignment
        {
            Input,
            Output
        }
        public string Text
        {
            get;
            set;
        }
        public Point Point
        {
            get;
            set;
        }
        public FloatingTextItem.NodeAlignment TextType
        {
            get;
            set;
        }
        public FloatingTextItem(string text, FloatingTextItem.NodeAlignment textType)
        {
            this.Text = text;
            this.TextType = textType;
        }
    }
}
