namespace NthDimension.Forms.Widgets
{
    public class ListViewSubItem : IListViewItem
    {
        string columnID = "";

        /// <summary>
        /// Get or set the column id that this subitem belongs to. This value is important and this subitem will 
        /// NOT GET DROWN until this value is set correctly. Use the same value of ListViewColumn.ID
        /// </summary>
        public string ColumnID
        { get { return columnID; } set { columnID = value; } }
    }
}
