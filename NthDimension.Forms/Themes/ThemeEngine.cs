using NthDimension.Forms.Widgets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Themes
{
    public static class ThemeEngine
    {
        static ThemeEngine()
        {
            BackColor = Color.White;
            BackColor2 = Color.FromArgb(50, 72, 99); //(72, 75, 85);
            ColorControl = Color.WhiteSmoke;////Color.FromArgb(32, 42, 52); //Color.Gray;
            ColorHighlight = Color.FromArgb(70, 92, 119);
            ColorHighlightText = Color.Gray;
        }

        public static Color BackColor
        {
            get;
            set;
        }

        public static Color BackColor2
        {
            get;
            set;
        }

        public static Color ColorControl
        {
            get;
            set;
        }

        public static Color ColorHighlight
        {
            get;
            set;
        }

        public static Color ColorHighlightText
        {
            get;
            set;
        }

        public static void TreeViewDrawNodePlusMinus(TreeViewCanvas treeView, TreeNode node, GContext dc,
                                                     int x, int middle, bool fillBackColor)
        {
            Rectangle rect = Rectangle.Empty;

            int nodeY = node.GetY();
            int height = treeView.ActualItemHeight - 2;

            if (fillBackColor)
                dc.FillRectangle(new SolidBrush(treeView.BackColor), (x + 4) - (height / 2), nodeY + 1, height, height);

            if (!fillBackColor)
            {
                rect = new Rectangle(x, middle - 4, 10, 10);
                int w4 = (int)System.Math.Ceiling((float)rect.Width / 4);
                int w2 = (int)System.Math.Ceiling((float)rect.Width / 2);
                int h4 = (int)System.Math.Ceiling((float)rect.Height / 4);
                int h2 = (int)System.Math.Ceiling((float)rect.Height / 2);

                if (node.IsExpanded)
                {
                    // p1-------------p2
                    //  ·             ·
                    //    ·			·
                    //       ·   ·
                    //        p3
                    dc.FillPolygon(new NanoSolidBrush(treeView.FGColor), new Point[]{
                                       new Point(rect.Left, rect.Top + h4 - 4),		// p1
					               	new Point(rect.Right, rect.Top + h4 - 4),  		// p2
					               	new Point(rect.Left + w2, rect.Bottom - 4)});   // p3
                }
                else
                {
                    // p1
                    // |  ·
                    // |	p2
                    // |  .
                    // p3
                    dc.FillPolygon(new NanoSolidBrush(treeView.FGColor), new Point[]{
                                       new Point(rect.Left + w4, rect.Top - 4),		// p1
					               	new Point(rect.Right, rect.Top + h2 - 4),		// p2
					               	new Point(rect.Left + w4, rect.Bottom - 4)});   // p3
                }
            }
            else
            {
                dc.DrawRectangle(new NanoPen(Color.LightGray), x, middle - 4, 9, 9);
                dc.DrawLine(new NanoPen(Color.LightGray), x + 2, middle, x + 7, middle);

                if (!node.IsExpanded)
                {
                    dc.DrawLine(new NanoPen(Color.LightGray), x + 4, middle - 2, x + 4, middle + 3);
                }
            }

            //dc.DrawRectangle(new NanoPen(TextColor.Red), rect);
        }
    }
}
