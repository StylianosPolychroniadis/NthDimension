using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Widgets
{
    public class ListViewColumn
    {
        private string text = "";
        private string id = "";
        private EListViewSortMode sortMode = EListViewSortMode.None;
        private int width = 60;
        /// <summary>
        /// Get or set the header text of this column.
        /// </summary>
        public string HeaderText
        { get { return text; } set { text = value; } }
        /// <summary>
        /// Get or set the sortmode for this column that will be used to sort items that connected to this column using id.
        /// </summary>
        public EListViewSortMode SortMode
        { get { return sortMode; } set { sortMode = value; } }
        /// <summary>
        /// Get or set the id of this column. Use this to connect subitems to this column.
        /// </summary>
        public string ID
        { get { return id; } set { id = value; } }
        /// <summary>
        /// Get or set the width of this column.
        /// </summary>
        public int Width
        { get { return width; } set { width = value; } }
    }
}
