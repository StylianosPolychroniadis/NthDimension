
namespace NthStudio.Gui.Widgets.TextEditor.Actions
{
    public class Cut : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            if (pTextCanvas.Document.ReadOnly)
            {
                return;
            }
            //pTextCanvas.ClipboardHandler.Cut(null, null);
        }
    }

    public class Copy : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            pTextCanvas.AutoClearSelection = false;
            //pTextCanvas.ClipboardHandler.Copy(null, null);
        }
    }

    public class Paste : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            if (pTextCanvas.Document.ReadOnly)
            {
                return;
            }
            //pTextCanvas.ClipboardHandler.Paste(null, null);
        }
    }
}
