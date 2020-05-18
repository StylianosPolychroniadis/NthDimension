namespace NthDimension.CalcEngine.Expressions
{
    /// <summary>
    /// Token ID (used when evaluating expressions)
    /// </summary>
    internal enum TokenId
    {
        AND, OR, // LOGICAL
        GT, LT, GE, LE, EQ, NE, // COMPARE
        ADD, SUB, // ADDSUB
        MUL, DIV, DIVINT, MOD, // MULDIV
        POWER, // POWER
        OPEN, CLOSE, END, COMMA, PERIOD, // GROUP
        ATOM, // LITERAL, IDENTIFIER
    }
}
