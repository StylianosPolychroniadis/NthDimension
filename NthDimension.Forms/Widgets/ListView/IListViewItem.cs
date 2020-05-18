using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Widgets
{
    /// <summary>
    /// The listview mode.
    /// </summary>
    public enum EListViewViewMode
    {
        /// <summary>
        /// Draw items in details mode. This will draw columns and subitems.
        /// </summary>
        Details,
        /// <summary>
        /// Draw items in thumbnails mode. This will draw items only.
        /// </summary>
        Thumbnails
    }

    /// <summary>
    /// The item draw mode.
    /// </summary>
    public enum EListViewItemDrawMode
    {
        /// <summary>
        /// Draw text only. The text will be taken from Text property of item or subitem.
        /// </summary>
        Text,
        /// <summary>
        /// Draw image only. The image will be used from ImagesList at given index of ImageIndex property of item or subitem.
        /// </summary>
        Image,
        /// <summary>
        /// Draw both text and image. The text will be taken from Text property of item or subitem,
        /// The image will be used from ImagesList at given index of ImageIndex property of item or subitem.
        /// </summary>
        TextAndImage,
        /// <summary>
        /// Use user resources to draw both text and image. You must use the draw events to draw once this mode chosen.
        /// </summary>
        UserDraw
    }

    /// <summary>
    /// The sort mode.
    /// </summary>
    public enum EListViewSortMode
    {
        /// <summary>
        /// Sort items A to Z.
        /// </summary>
        AtoZ,
        /// <summary>
        /// Sort items Z to A.
        /// </summary>
        ZtoA,
        /// <summary>
        /// No sort.
        /// </summary>
        None
    }

    public abstract class IListViewItem
    {
        private string text = "";
        private Color color = Color.Black;
        private int imageIndex;
        private bool customFontEnabled = false;
        private NanoFont font = new NanoFont(NanoFont.DefaultRegular, 8f);
        private EListViewItemDrawMode drawMode = EListViewItemDrawMode.Text;
        private object tag;

        /// <summary>
        /// Get or set the item text.
        /// </summary>
        public virtual string Text
        { get { return text; } set { text = value; } }
        /// <summary>
        /// Get or set this item text's color.
        /// </summary>
        public virtual Color TextColor
        { get { return color; } set { color = value; } }
        /// <summary>
        /// Get or set the image index for this item within the ImagesList collection of the parent control.
        /// </summary>
        public virtual int ImageIndex
        { get { return imageIndex; } set { imageIndex = value; } }
        /// <summary>
        /// Get or set the draw mode for this item.
        /// </summary>
        public EListViewItemDrawMode DrawMode
        { get { return this.drawMode; } set { this.drawMode = value; } }
        /// <summary>
        /// Get or set if the custom font enabled for this item. Normally the control draw item texts debending on font value 
        /// of that control, but if this value enabled the control will use the font that specified in CustomFont property 
        /// of thos item.
        /// </summary>
        public bool CustomFontEnabled
        { get { return customFontEnabled; } set { customFontEnabled = value; } }
        /// <summary>
        /// Get or set the custom font which will be used to draw text if this item when the CustomFontEnabled property is true.
        /// </summary>
        public NanoFont CustomFont
        { get { return font; } set { font = value; } }
        /// <summary>
        /// Get or set the tag for this item.
        /// </summary>
        public object Tag
        { get { return this.tag; } set { this.tag = value; } }
        /// <summary>
        /// Rises the mouse over event.
        /// </summary>
        /// <param name="mouseLocation">The mouse location within the viewport of the parent listview control.</param>
        /// <param name="charFontSize">The char font size</param>
        public virtual void OnMouseOver(Point mouseLocation, Size charFontSize) { }
        /// <summary>
        /// Rises the mouse click event
        /// </summary>
        /// <param name="mouseLocation">The mouse location within the viewport of the parent listview control.</param>
        /// <param name="charFontSize">The char font size</param>
        /// <param name="itemIndex">The item index or the part item index if this item is a subitem.</param>
        public virtual void OnMouseClick(Point mouseLocation, Size charFontSize, int itemIndex) { }
        /// <summary>
        /// Rises the mouse leave event
        /// </summary>
        public virtual void OnMouseLeave() { }
    }
}
