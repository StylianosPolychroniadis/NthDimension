namespace NthDimension.CodeGen.Dom.Refactoring
{
    /// <summary>
    /// A document representing a source code file for refactoring.
    /// Line and column counting starts at 1.
    /// Offset counting starts at 0.
    /// </summary>
    public interface IDocument
    {
        int TextLength { get; }
        int TotalNumberOfLines { get; }
        IDocumentLine GetLine(int lineNumber);

        int PositionToOffset(int line, int column);

        void Insert(int offset, string text);
        void Remove(int offset, int length);
        char GetCharAt(int offset);

        /// <summary>
        /// Make the document combine the following actions into a single
        /// action for undo purposes.
        /// </summary>
        void StartUndoableAction();

        /// <summary>
        /// Ends the undoable action started with <see cref="StartUndoableAction"/>.
        /// </summary>
        void EndUndoableAction();

        void UpdateView();
    }

    /// <summary>
    /// A line inside a <see cref="IDocument"/>.
    /// </summary>
    public interface IDocumentLine
    {
        int Offset { get; }
        int Length { get; }
        string Text { get; }
    }
}
