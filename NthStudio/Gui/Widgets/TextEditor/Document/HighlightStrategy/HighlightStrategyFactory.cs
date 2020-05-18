using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor.Document.HighlightStrategy
{
    public static class HighlightingStrategyFactory
    {
        public static IHighlightingStrategy CreateHighlightingStrategy()
        {
            return (IHighlightingStrategy)HighlightingManager.Manager.HighlightingDefinitions["Default"];
        }

        public static IHighlightingStrategy CreateHighlightingStrategy(string name)
        {
            IHighlightingStrategy highlightingStrategy = HighlightingManager.Manager.FindHighlighter(name);

            if (highlightingStrategy == null)
            {
                return CreateHighlightingStrategy();
            }
            return highlightingStrategy;
        }

        public static IHighlightingStrategy CreateHighlightingStrategyForFile(string fileName)
        {
            IHighlightingStrategy highlightingStrategy = HighlightingManager.Manager.FindHighlighterForFile(fileName);
            if (highlightingStrategy == null)
            {
                return CreateHighlightingStrategy();
            }
            return highlightingStrategy;
        }
    }
}
