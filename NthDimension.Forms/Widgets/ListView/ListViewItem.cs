using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Widgets
{
    public class ListViewItem : IListViewItem
    {
        private List<ListViewSubItem> subItems = new List<ListViewSubItem>();
        private bool selected = false;
        private bool specialItem = false;

        /// <summary>
        /// Get subitem using column id
        /// </summary>
        /// <param name="id">The column id</param>
        /// <returns>The arget subitem if found otherwise null.</returns>
        public ListViewSubItem GetSubItemByID(string id)
        {
            foreach (ListViewSubItem subItem in this.subItems)
            {
                if (subItem.ColumnID == id)
                    return subItem;
            }
            return null;
        }
        /// <summary>
        /// Get or set the subitems collection.
        /// </summary>
        public List<ListViewSubItem> SubItems
        {
            get { return subItems; }
            set
            {
                subItems = value;
            }
        }
        /// <summary>
        /// Get or set a value indecate whether this item is selected.
        /// </summary>
        public bool Selected
        { get { return selected; } set { selected = value; } }
        /// <summary>
        /// Get or set a value indecate whether this item is special. Special items always colered with special color.
        /// </summary>
        public bool IsSpecialItem
        { get { return specialItem; } set { specialItem = value; } }
        /// <summary>
        /// Rises the "on mouse leave" event.
        /// </summary>
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();
            foreach (IListViewItem subitem in subItems)
                subitem.OnMouseLeave();
        }
    }
}
