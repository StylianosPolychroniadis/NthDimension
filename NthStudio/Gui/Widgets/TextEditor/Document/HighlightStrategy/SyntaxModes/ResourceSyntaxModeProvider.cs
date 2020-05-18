using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace NthStudio.Gui.Widgets.TextEditor.Document.HighlightStrategy.SyntaxModes
{
    public class ResourceSyntaxModeProvider : ISyntaxModeFileProvider
    {
        List<SyntaxMode> syntaxModes = null;

        public ICollection<SyntaxMode> SyntaxModes
        {
            get
            {
                return syntaxModes;
            }
        }

        public ResourceSyntaxModeProvider()
        {
            Assembly assembly = typeof(SyntaxMode).Assembly;
            Stream syntaxModeStream = assembly.GetManifestResourceStream(StringConstants.TextEditorDefinitionsSyntaxModes);
            if (syntaxModeStream != null)
            {
                syntaxModes = SyntaxMode.GetSyntaxModes(syntaxModeStream);
            }
            else
            {
                syntaxModes = new List<SyntaxMode>();
            }
        }

        public XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode)
        {
            Assembly assembly = typeof(SyntaxMode).Assembly;
            try
            {
                Stream strm = assembly.GetManifestResourceStream(StringConstants.TextEditorDefinitions + syntaxMode.FileName);
                return new XmlTextReader(strm);
            }
            catch (System.NullReferenceException) // by Miki
            {
                return null;
            }
        }

        public void UpdateSyntaxModeList()
        {
            // resources don't change during runtime
        }
    }
}
