namespace NthDimension.CodeGen.Dom
{
    /// <summary>
    /// Structure containing the result of a call to an expression finder.
    /// </summary>
    public struct ExpressionResult
    {
        /// <summary>The expression that has been found at the specified offset.</summary>
        public string Expression;
        /// <summary>Specifies the context in which the expression was found.</summary>
        public ExpressionContext Context;
        /// <summary>An object carrying additional language-dependend data.</summary>
        public object Tag;

        public ExpressionResult(string expression) : this(expression, ExpressionContext.Default, null) { }
        public ExpressionResult(string expression, ExpressionContext context) : this(expression, context, null) { }
        public ExpressionResult(string expression, object tag) : this(expression, ExpressionContext.Default, tag) { }

        public ExpressionResult(string expression, ExpressionContext context, object tag)
        {
            this.Expression = expression;
            this.Context = context;
            this.Tag = tag;
        }

        public override string ToString()
        {
            if (Context == ExpressionContext.Default)
                return "<" + Expression + ">";
            else
                return "<" + Expression + "> (" + Context.ToString() + ")";
        }
    }
}
