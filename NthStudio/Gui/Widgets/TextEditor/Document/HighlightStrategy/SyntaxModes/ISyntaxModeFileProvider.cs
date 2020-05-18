
using System.Collections.Generic;
using System.Xml;

namespace NthStudio.Gui.Widgets.TextEditor.Document.HighlightStrategy.SyntaxModes
{
    public interface ISyntaxModeFileProvider
    {
        ICollection<SyntaxMode> SyntaxModes
        {
            get;
        }

        XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode);
        void UpdateSyntaxModeList();
    }
}
