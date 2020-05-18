using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Widgets
{
    public class ListViewItemParameter : EventArgs
    {
        public ListViewItem Item
        {
            get;
            set;
        }

        public ListViewItemParameter(ListViewItem pItem)
        {
            Item = pItem;
        }
    }

    public class ListViewColumnParameter : EventArgs
    {
        public ListViewColumn Column
        {
            get;
            set;
        }

        public ListViewColumnParameter(ListViewColumn pColumn)
        {
            Column = pColumn;
        }
    }

    /// <summary>
    /// Arguments for column click events
    /// </summary>
    public class ListViewColumnClickArgs : EventArgs
    {
        /// <summary>
        /// Arguments for column click events
        /// </summary>
        /// <param name="id">The target column id</param>
        public ListViewColumnClickArgs(string id)
        {
            this.id = id;
        }

        private string id = "";

        /// <summary>
        /// Get the column id
        /// </summary>
        public string ColumnID
        {
            get { return id; }
        }
    }

    /// <summary>
    /// Arguments for column draw events
    /// </summary>
    public class ListViewColumnDrawArgs : EventArgs
    {
        /// <summary>
        /// Arguments for column draw events
        /// </summary>
        /// <param name="id">The target column id</param>
        /// <param name="gr">The graphics class used to draw the column</param>
        /// <param name="rectangle">The rectangle area of the column in the draw panel</param>
        public ListViewColumnDrawArgs(string id, GContext gr, Rectangle rectangle)
        {
            this.id = id;
            this.rectangle = rectangle;
            this.gr = gr;
        }

        private string id = "";
        private Rectangle rectangle;
        private GContext gr;

        /// <summary>
        /// Get the column id
        /// </summary>
        public string ColumnID
        {
            get { return id; }
        }
        /// <summary>
        /// Get the rectangle area of the column in the draw panel
        /// </summary>
        public Rectangle ColumnRectangle
        {
            get { return rectangle; }
        }
        /// <summary>
        /// Get the graphics class used to draw the column
        /// </summary>
        public GContext Graphics
        { get { return gr; } }
    }

    /// <summary>
    /// Arguments for item double click events
    /// </summary>
    public class ListViewItemDoubleClickArgs : EventArgs
    {
        /// <summary>
        /// Arguments for item double click events
        /// </summary>
        /// <param name="itemIndex">The clicked item index</param>
        public ListViewItemDoubleClickArgs(int itemIndex)
        {
            this.index = itemIndex;
        }

        private int index = 0;
        /// <summary>
        /// Get the clicked item index
        /// </summary>
        public int ClickedItemIndex
        {
            get { return index; }
        }
    }

    /// <summary>
    /// Arguments for item draw vents
    /// </summary>
    public class ListViewItemDrawArgs : EventArgs
    {
        /// <summary>
        /// Arguments for item draw vents
        /// </summary>
        /// <param name="itemIndex">The target item index</param>
        public ListViewItemDrawArgs(int itemIndex)
        {
            this.itemIndex = itemIndex;
        }

        private int itemIndex;
        private Image image;
        private string text;

        /// <summary>
        /// Get or set the text to draw
        /// </summary>
        public string TextToDraw
        { get { return text; } set { text = value; } }
        /// <summary>
        /// Get or set the image to draw
        /// </summary>
        public Image ImageToDraw
        { get { return image; } set { image = value; } }
        /// <summary>
        /// Get the target item index
        /// </summary>
        public int ItemIndex
        { get { return itemIndex; } }
    }
    /// <summary>
    /// Arguments for item select events.
    /// </summary>
    public class ListViewItemSelectArgs : EventArgs
    {
        /// <summary>
        /// Arguments for item select events.
        /// </summary>
        /// <param name="itemIndex">The target item index</param>
        public ListViewItemSelectArgs(int itemIndex)
        {
            this.itemIndex = itemIndex;
        }
        private int itemIndex;
        /// <summary>
        /// The item index.
        /// </summary>
        public int ItemIndex
        { get { return itemIndex; } }
    }
    /// <summary>
    /// Arguments for mouse over subitem events.
    /// </summary>
    public class ListViewMouseOverSubItemArgs : EventArgs
    {
        /// <summary>
        /// Arguments for mouse over subitem events.
        /// </summary>
        /// <param name="itemIndex">The target item index.</param>
        /// <param name="columnID">The column id which the subitem belong to.</param>
        /// <param name="mouseX">The mouse x coordinate value in the panel (not the view port).</param>
        public ListViewMouseOverSubItemArgs(int itemIndex, string columnID, int mouseX)
        {
            this.itemIndex = itemIndex;
            this.columnID = columnID;
            this.mouseX = mouseX;
        }

        private int itemIndex = -1;
        private string columnID = "";
        private int mouseX = 0;

        /// <summary>
        /// Get the column id which the subitem belong to.
        /// </summary>
        public string ColumnID
        { get { return columnID; } }
        /// <summary>
        /// Get the parent item index.
        /// </summary>
        public int ItemIndex
        { get { return itemIndex; } }
        /// <summary>
        /// The mouse x coordinate value in the panel (not the view port).
        /// </summary>
        public int MouseX
        { get { return mouseX; } }
    }
    /// <summary>
    /// Argumnets for rating events.
    /// </summary>
    public class ListViewRatingChangedArgs : EventArgs
    {

        /// <summary>
        /// Argumnets for rating events.
        /// </summary>
        /// <param name="id">The column id which the subitem belongs to.</param>
        /// <param name="itemIndex">The parent item index</param>
        /// <param name="rating">The rating value (0-5)</param>
        public ListViewRatingChangedArgs(string id, int itemIndex, int rating)
        {
            this.itemIndex = itemIndex;
            this.id = id;
            this.rating = rating;
        }

        private string id = "";
        private int itemIndex;
        private int rating = 0;
        /// <summary>
        /// Get the column id which the subitem belongs to.
        /// </summary>
        public string ColumnID
        { get { return id; } }
        /// <summary>
        /// Get The parent item index
        /// </summary>
        public int ItemIndex
        { get { return itemIndex; } }
        /// <summary>
        /// Get the rating value (0-5; 0=none rating, 5=top rating or 5 stars)
        /// </summary>
        public int Rating
        { get { return rating; } }
    }
    /// <summary>
    /// Arguments for subitem draw events.
    /// </summary>
    public class ListViewSubItemDrawArgs : EventArgs
    {
        /// <summary>
        /// Arguments for subitem draw events.
        /// </summary>
        /// <param name="id">The column id which this subitem belongs to.</param>
        /// <param name="itemIndex">The parent item index.</param>
        public ListViewSubItemDrawArgs(string id, int itemIndex)
        {
            this.itemIndex = itemIndex;
            this.id = id;
        }

        private string id = "";
        private int itemIndex;
        private Image image;
        private string text;

        /// <summary>
        /// Get the column id which this subitem belongs to.
        /// </summary>
        public string ColumnID
        { get { return id; } }
        /// <summary>
        /// Get or set the text to draw.
        /// </summary>
        public string TextToDraw
        { get { return text; } set { text = value; } }
        /// <summary>
        /// Get or set the image to draw.
        /// </summary>
        public Image ImageToDraw
        { get { return image; } set { image = value; } }
        /// <summary>
        /// Get the parent item index.
        /// </summary>
        public int ItemIndex
        { get { return itemIndex; } }
    }
}
