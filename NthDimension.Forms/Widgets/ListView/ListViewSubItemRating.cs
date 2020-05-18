using System;
using System.Drawing;


namespace NthDimension.Forms.Widgets
{
    /// <summary>
    /// This is special subitem can be treated like normal subitem (added to item normaly) but 
    /// this subitem will be drawn differently. The parent advanced listview control will draw it as 5 stars debending on Rating 
    /// value, also the control will allow user to change rating using mouse. No text or image will be drawn and no draw event 
    /// will rise.
    /// </summary>
    public class ListViewSubItemRating : ListViewSubItem
    {
        private int rating = 0;
        private bool drawOverImage;
        private int overRating = 0;
        private int itemHeight;

        /// <summary>
        /// Rised when the user changes the rating via clicking on this subitem.
        /// </summary>
        public event EventHandler<ListViewRatingChangedArgs> RatingChanged;
        /// <summary>
        /// Rised when this sub item need to refresh rating (to get the original value of rating)
        /// </summary>
        public event EventHandler<ListViewRatingChangedArgs> UpdateRatingRequest;
        /// <summary>
        /// Get or set the rating value. (0-5, 0=none rating, 5=top rating or 5 stars)
        /// </summary>
        public int Rating
        {
            get { return rating; }
            set
            {
                rating = value;
            }
        }
        /// <summary>
        /// Get or set the overrating value (0-5, 0=none rating, 5=top rating or 5 stars). This value changed debending on the mouse 
        /// cursor overing the subitem and has no effect on original rating value.
        /// </summary>
        public int OverRating
        {
            get { return overRating; }
            set
            {
                overRating = value;
            }
        }
        /// <summary>
        /// Get or set if should draw the overrating image. The overimage draw when the overrating value changed debending on the mouse 
        /// cursor overing the subitem and has no effect on original rating value.
        /// </summary>
        public bool DrawOverImage
        {
            get { return drawOverImage; }
            set { drawOverImage = value; }
        }
        /// <summary>
        /// Rises the mouse click event
        /// </summary>
        /// <param name="mouseLocation">The mouse location within the viewport.</param>
        /// <param name="charFontSize">The default char size debending on parent listview control font.</param>
        /// <param name="itemIndex">The parent item index.</param>
        public override void OnMouseClick(Point mouseLocation, Size charFontSize, int itemIndex)
        {
            base.OnMouseClick(mouseLocation, charFontSize, itemIndex);
            int width = this.itemHeight * 4;
            int baseW = (width / 5);
            if (mouseLocation.X < baseW)
            { rating = 1; }
            else if (mouseLocation.X >= (baseW) && mouseLocation.X < (baseW * 2))
            { rating = 2; }
            else if (mouseLocation.X >= (baseW * 2) && mouseLocation.X < (baseW * 3))
            { rating = 3; }
            else if (mouseLocation.X >= (baseW * 3) && mouseLocation.X < (baseW * 4))
            { rating = 4; }
            else if (mouseLocation.X >= (baseW * 4) && mouseLocation.X < (baseW * 5))
            { rating = 5; }
            else if (mouseLocation.X >= baseW * 5)
            { rating = 0; }
            if (RatingChanged != null)
                RatingChanged(this, new ListViewRatingChangedArgs(base.ColumnID, itemIndex, rating));
        }
        /// <summary>
        /// Rises the mouse over event
        /// </summary>
        /// <param name="mouseLocation">The mouse location within the viewport.</param>
        /// <param name="charFontSize">The default char size debending on parent listview control font.</param>
        public override void OnMouseOver(Point mouseLocation, Size charFontSize)
        {
            base.OnMouseOver(mouseLocation, charFontSize);
            int width = this.itemHeight * 4;
            int baseW = (width / 5);
            if (mouseLocation.X < baseW)
            { overRating = 1; }
            else if (mouseLocation.X >= (baseW) && mouseLocation.X < (baseW * 2))
            { overRating = 2; }
            else if (mouseLocation.X >= (baseW * 2) && mouseLocation.X < (baseW * 3))
            { overRating = 3; }
            else if (mouseLocation.X >= (baseW * 3) && mouseLocation.X < (baseW * 4))
            { overRating = 4; }
            else if (mouseLocation.X >= (baseW * 4) && mouseLocation.X < (baseW * 5))
            { overRating = 5; }
            else if (mouseLocation.X >= baseW * 5)
            { overRating = 0; }
        }
        /// <summary>
        /// Rises the UpdateRatingRequest event.
        /// </summary>
        /// <param name="itemIndex">The parent item index</param>
        /// <param name="itemHeight">The item height value. Should be calculated in the parent control automaticly debending on draw status.</param>
        public void OnRefreshRating(int itemIndex, int itemHeight)
        {
            if (UpdateRatingRequest != null)
                UpdateRatingRequest(this, new ListViewRatingChangedArgs(base.ColumnID, itemIndex, 0));
            this.itemHeight = itemHeight;
        }
    }
}
