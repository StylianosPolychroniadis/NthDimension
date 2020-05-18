namespace NthDimension.CodeGen.Dom
{
    public interface IExpressionFinder
    {
        /// <summary>
        /// Finds an expression before the current offset.
        /// </summary>
        ExpressionResult FindExpression(string text, int offset);

        /// <summary>
        /// Finds an expression around the current offset.
        /// </summary>
        ExpressionResult FindFullExpression(string text, int offset);

        /// <summary>
        /// Removed the last part of the expression.
        /// </summary>
        /// <example>
        /// "arr[i]" => "arr"
        /// "obj.Field" => "obj"
        /// "obj.Method(args,...)" => "obj.Method"
        /// </example>
        string RemoveLastPart(string expression);
    }
}
