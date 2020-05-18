
using System;
using System.Collections.Generic;

using NthStudio.Gui.Widgets.TextEditor.Document;
using NthStudio.Gui.Widgets.TextEditor.Document.FoldingStrategy;

namespace NthStudio.Gui.Widgets.TextEditor.Actions
{
    public class ToggleFolding : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            List<FoldMarker> foldMarkers = pTextCanvas.Document.FoldingManager.GetFoldingsWithStart(pTextCanvas.Caret.Line);
            if (foldMarkers.Count != 0)
            {
                foreach (FoldMarker fm in foldMarkers)
                    fm.IsFolded = !fm.IsFolded;
            }
            else
            {
                foldMarkers = pTextCanvas.Document.FoldingManager.GetFoldingsContainsLineNumber(pTextCanvas.Caret.Line);
                if (foldMarkers.Count != 0)
                {
                    FoldMarker innerMost = foldMarkers[0];
                    for (int i = 1; i < foldMarkers.Count; i++)
                    {
                        if (new TextLocation(foldMarkers[i].StartColumn, foldMarkers[i].StartLine) >
                            new TextLocation(innerMost.StartColumn, innerMost.StartLine))
                        {
                            innerMost = foldMarkers[i];
                        }
                    }
                    innerMost.IsFolded = !innerMost.IsFolded;
                }
            }
            pTextCanvas.Document.FoldingManager.NotifyFoldingsChanged(EventArgs.Empty);
        }
    }

    public class ToggleAllFoldings : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            bool doFold = true;
            foreach (FoldMarker fm in pTextCanvas.Document.FoldingManager.FoldMarker)
            {
                if (fm.IsFolded)
                {
                    doFold = false;
                    break;
                }
            }
            foreach (FoldMarker fm in pTextCanvas.Document.FoldingManager.FoldMarker)
            {
                fm.IsFolded = doFold;
            }
            pTextCanvas.Document.FoldingManager.NotifyFoldingsChanged(EventArgs.Empty);
        }
    }

    public class ShowDefinitionsOnly : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            foreach (FoldMarker fm in pTextCanvas.Document.FoldingManager.FoldMarker)
            {
                fm.IsFolded = fm.FoldType == FoldType.MemberBody || fm.FoldType == FoldType.Region;
            }
            pTextCanvas.Document.FoldingManager.NotifyFoldingsChanged(EventArgs.Empty);
        }
    }
}
