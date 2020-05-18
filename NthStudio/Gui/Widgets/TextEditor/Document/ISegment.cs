namespace NthStudio.Gui.Widgets.TextEditor.Document
{
    public interface ISegment
    {
        /// <value>
        /// The offset where the span begins
        /// </value>
        int Offset
        {
            get;
            set;
        }

        /// <value>
        /// The length of the span
        /// </value>
        int Length
        {
            get;
            set;
        }
    }

}
