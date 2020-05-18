using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace WinFormsUI
{
    [System.ComponentModel.DesignerCategory("code")]
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.All)]
    public class ToolStripFontComboBox : ToolStripComboBox
    {
        protected int iDropWidth = 0;

        public ToolStripFontComboBox()
        {
            // Set the draw mode so we can take over item drawing
            this.ComboBox.DrawMode = DrawMode.OwnerDrawVariable;
            this.AutoCompleteMode = AutoCompleteMode.Append;

            // Handle the events
            this.ComboBox.DropDown += new EventHandler(DropDown);
            this.ComboBox.MeasureItem += new MeasureItemEventHandler(MeasureItem);
            this.ComboBox.DrawItem += new DrawItemEventHandler(DrawItem);

            // Create the list of fonts, and populate the ComboBox with that list
            PopulateFonts();
        }

        protected new void DropDown(object sender, EventArgs e)
        {
            this.ComboBox.DropDownWidth = iDropWidth;
        }

        protected void MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index > -1)
            {
                string szFont = Items[e.Index].ToString();
                Graphics g = this.ComboBox.CreateGraphics();
                SizeF size = g.MeasureString(szFont, new Font(szFont, this.Font.Size));

                // Set the Item's Width, and iDropWidth if the item has a greater width
                e.ItemWidth = (int)size.Width;
                if (e.ItemWidth > iDropWidth) { iDropWidth = e.ItemWidth; }

                // If .NET gives you problems drawing fonts with different heights, set a maximum height
                e.ItemHeight = (int)size.Height;
                if (e.ItemHeight > 20) { e.ItemHeight = 20; }
            }
        }

        protected void DrawItem(object sender, DrawItemEventArgs e)
        {
            // DrawBackground handles drawing the background (i.e,. hot-tracked v. not)
            // It uses the system colors (Bluish, and and white, by default)
            // same as calling e.Graphics.FillRectangle ( SystemBrushes.Highlight, e.Bounds );
            e.DrawBackground();

            if (e.Index > -1)
            {
                string szFont = Items[e.Index].ToString();
                Font fFont = new Font(szFont, this.Font.Size);

                Rectangle rectDraw = e.Bounds;

                if (e.State != DrawItemState.Selected)
                {
                    e.Graphics.DrawString(szFont, fFont, SystemBrushes.WindowText, rectDraw);

                }
                else
                {
                    e.Graphics.DrawString(szFont, fFont, SystemBrushes.HighlightText, rectDraw);
                }
            }
            // Uncomment this if you actually like the way the focus rectangle looks
            e.DrawFocusRectangle();
        }

        public void PopulateFonts()
        {
            foreach (FontFamily ff in FontFamily.Families)
            {
                if (ff.IsStyleAvailable(FontStyle.Regular))
                {
                    this.Items.Add(ff.Name);
                }
            }
            if (Items.Count > 0)
                this.SelectedIndex = this.FindString("Times");
        }



    }
}
