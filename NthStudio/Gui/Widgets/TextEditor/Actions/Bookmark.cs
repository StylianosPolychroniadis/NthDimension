
using System;

using NthStudio.Gui.Widgets.TextEditor.Document.BookmarkManagement;

namespace NthStudio.Gui.Widgets.TextEditor.Actions
{
    public class ToggleBookmark : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            pTextCanvas.Document.BookmarkManager.ToggleMarkAt(pTextCanvas.Caret.Position);
            pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, pTextCanvas.Caret.Line));
            pTextCanvas.Document.CommitUpdate();

        }
    }

    public class GotoPrevBookmark : AbstractEditAction
    {
        Predicate<Bookmark> predicate = null;

        public GotoPrevBookmark(Predicate<Bookmark> predicate)
        {
            this.predicate = predicate;
        }

        public override void Execute(TextCanvas pTextCanvas)
        {
            Bookmark mark = pTextCanvas.Document.BookmarkManager.GetPrevMark(pTextCanvas.Caret.Line, predicate);
            if (mark != null)
            {
                pTextCanvas.Caret.Position = mark.Location;
                pTextCanvas.SelectionManager.ClearSelection();
                pTextCanvas.SetDesiredColumn();
            }
        }
    }

    public class GotoNextBookmark : AbstractEditAction
    {
        Predicate<Bookmark> predicate = null;

        public GotoNextBookmark(Predicate<Bookmark> predicate)
        {
            this.predicate = predicate;
        }

        public override void Execute(TextCanvas pTextCanvas)
        {
            Bookmark mark = pTextCanvas.Document.BookmarkManager.GetNextMark(pTextCanvas.Caret.Line, predicate);
            if (mark != null)
            {
                pTextCanvas.Caret.Position = mark.Location;
                pTextCanvas.SelectionManager.ClearSelection();
                pTextCanvas.SetDesiredColumn();
            }
        }
    }

    public class ClearAllBookmarks : AbstractEditAction
    {
        Predicate<Bookmark> predicate = null;

        public ClearAllBookmarks(Predicate<Bookmark> predicate)
        {
            this.predicate = predicate;
        }

        public override void Execute(TextCanvas pTextCanvas)
        {
            pTextCanvas.Document.BookmarkManager.RemoveMarks(predicate);
            pTextCanvas.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
            pTextCanvas.Document.CommitUpdate();
        }
    }
}
