using NthStudio.Gui.Widgets.TextEditor.Document;


namespace NthStudio.Gui.Widgets.TextEditor.Actions
{
    public class ShiftCaretRight : CaretRight
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation oldCaretPos = pTextCanvas.Caret.Position;
            base.Execute(pTextCanvas);
            pTextCanvas.AutoClearSelection = false;
            pTextCanvas.SelectionManager.ExtendSelection(oldCaretPos, pTextCanvas.Caret.Position);
        }
    }

    public class ShiftCaretLeft : CaretLeft
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation oldCaretPos = pTextCanvas.Caret.Position;
            base.Execute(pTextCanvas);
            pTextCanvas.AutoClearSelection = false;
            pTextCanvas.SelectionManager.ExtendSelection(oldCaretPos, pTextCanvas.Caret.Position);
        }
    }

    public class ShiftCaretUp : CaretUp
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation oldCaretPos = pTextCanvas.Caret.Position;
            base.Execute(pTextCanvas);
            pTextCanvas.AutoClearSelection = false;
            pTextCanvas.SelectionManager.ExtendSelection(oldCaretPos, pTextCanvas.Caret.Position);
        }
    }

    public class ShiftCaretDown : CaretDown
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation oldCaretPos = pTextCanvas.Caret.Position;
            base.Execute(pTextCanvas);
            pTextCanvas.AutoClearSelection = false;
            pTextCanvas.SelectionManager.ExtendSelection(oldCaretPos, pTextCanvas.Caret.Position);
        }
    }

    public class ShiftWordRight : WordRight
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation oldCaretPos = pTextCanvas.Caret.Position;
            base.Execute(pTextCanvas);
            pTextCanvas.AutoClearSelection = false;
            pTextCanvas.SelectionManager.ExtendSelection(oldCaretPos, pTextCanvas.Caret.Position);
        }
    }

    public class ShiftWordLeft : WordLeft
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation oldCaretPos = pTextCanvas.Caret.Position;
            base.Execute(pTextCanvas);
            pTextCanvas.AutoClearSelection = false;
            pTextCanvas.SelectionManager.ExtendSelection(oldCaretPos, pTextCanvas.Caret.Position);
        }
    }

    public class ShiftHome : Home
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation oldCaretPos = pTextCanvas.Caret.Position;
            base.Execute(pTextCanvas);
            pTextCanvas.AutoClearSelection = false;
            pTextCanvas.SelectionManager.ExtendSelection(oldCaretPos, pTextCanvas.Caret.Position);
        }
    }

    public class ShiftEnd : End
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation oldCaretPos = pTextCanvas.Caret.Position;
            base.Execute(pTextCanvas);
            pTextCanvas.AutoClearSelection = false;
            pTextCanvas.SelectionManager.ExtendSelection(oldCaretPos, pTextCanvas.Caret.Position);
        }
    }

    public class ShiftMoveToStart : MoveToStart
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation oldCaretPos = pTextCanvas.Caret.Position;
            base.Execute(pTextCanvas);
            pTextCanvas.AutoClearSelection = false;
            pTextCanvas.SelectionManager.ExtendSelection(oldCaretPos, pTextCanvas.Caret.Position);
        }
    }

    public class ShiftMoveToEnd : MoveToEnd
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation oldCaretPos = pTextCanvas.Caret.Position;
            base.Execute(pTextCanvas);
            pTextCanvas.AutoClearSelection = false;
            pTextCanvas.SelectionManager.ExtendSelection(oldCaretPos, pTextCanvas.Caret.Position);
        }
    }

    public class ShiftMovePageUp : MovePageUp
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation oldCaretPos = pTextCanvas.Caret.Position;
            base.Execute(pTextCanvas);
            pTextCanvas.AutoClearSelection = false;
            pTextCanvas.SelectionManager.ExtendSelection(oldCaretPos, pTextCanvas.Caret.Position);
        }
    }

    public class ShiftMovePageDown : MovePageDown
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            TextLocation oldCaretPos = pTextCanvas.Caret.Position;
            base.Execute(pTextCanvas);
            pTextCanvas.AutoClearSelection = false;
            pTextCanvas.SelectionManager.ExtendSelection(oldCaretPos, pTextCanvas.Caret.Position);
        }
    }

    public class SelectWholeDocument : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            pTextCanvas.AutoClearSelection = false;
            TextLocation startPoint = new TextLocation(0, 0);
            TextLocation endPoint = pTextCanvas.Document.OffsetToPosition(pTextCanvas.Document.TextLength);
            if (pTextCanvas.SelectionManager.HasSomethingSelected)
            {
                if (pTextCanvas.SelectionManager.SelectionCollection[0].StartPosition == startPoint &&
                    pTextCanvas.SelectionManager.SelectionCollection[0].EndPosition == endPoint)
                {
                    return;
                }
            }
            pTextCanvas.Caret.Position = pTextCanvas.SelectionManager.NextValidPosition(endPoint.Y);
            pTextCanvas.SelectionManager.ExtendSelection(startPoint, endPoint);
            // after a SelectWholeDocument selection, the caret is placed correctly,
            // but it is not positioned internally.  The effect is when the cursor
            // is moved up or down a line, the caret will take on the column that
            // it was in before the SelectWholeDocument
            pTextCanvas.SetDesiredColumn();
        }
    }

    public class ClearAllSelections : AbstractEditAction
    {
        public override void Execute(TextCanvas pTextCanvas)
        {
            pTextCanvas.SelectionManager.ClearSelection();
        }
    }
}
