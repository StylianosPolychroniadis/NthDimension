
using System.Drawing;
using NthDimension;
using NthDimension.Forms;
using NthStudio.Gui.Widgets.TextEditor.Margins;

namespace NthStudio.Gui.Widgets.TextEditor
{
    /// <summary>
    /// This delegate is used for document events.
    /// </summary>
    public delegate void DocumentEventHandler(object sender, DocumentEventArgs e);
    public delegate bool KeyEventHandler(char ch);
    public delegate bool DialogKeyProcessor(Keys keyData);

    public delegate void MarginMouseEventHandler(AbstractMargin sender, Point mousepos, MouseButton mouseButtons);
    public delegate void MarginPaintEventHandler(AbstractMargin sender, GContext gc, Rectangle rect);
}
