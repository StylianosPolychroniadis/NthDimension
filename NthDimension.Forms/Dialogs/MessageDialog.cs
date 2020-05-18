using System.Drawing;

namespace NthDimension.Forms.Dialogs
{
    /// <summary>
    /// Description of MessageDialog.
    /// </summary>
    public class MessageDialog : DialogBase
    {
        public MessageDialog()
        {
            Size = new Size(400, 300);
        }

        public override ImageList ImgList
        {
            get;
            set;
        }
    }
}
