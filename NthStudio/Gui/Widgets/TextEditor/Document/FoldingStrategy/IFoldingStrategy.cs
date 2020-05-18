using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.TextEditor.Document.FoldingStrategy
{
    public interface IFoldingStrategy
    {
        /// <remarks>
        /// Calculates the fold level of a specific line.
        /// </remarks>
        List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation);
    }

}
