namespace NthDimension.CalcEngine.Expressions
{
    /// <summary>
    /// Represents a node in the expression tree.
    /// </summary>
    internal class Token
    {
        // ** fields
        public TokenId ID;
        public TokenType Type;
        public System.Object Value;

        // ** ctor
        public Token(System.Object value, TokenId id, TokenType type)
        {
            Value = value;
            ID = id;
            Type = type;
        }
    }
}
