namespace NthDimension.CodeGen.Dom
{
    using System.Collections;

    public interface IResolver
    {
        /// <summary>
        /// Resolves an expression.
        /// The caretLineNumber and caretColumn is 1 based.
        /// </summary>
        ResolveResult Resolve(ExpressionResult expressionResult,
                              int caretLineNumber,
                              int caretColumn,
                              string fileName,
                              string fileContent);

        ArrayList CtrlSpace(int caretLine, int caretColumn, string fileName, string fileContent, ExpressionContext context);
    }
}
