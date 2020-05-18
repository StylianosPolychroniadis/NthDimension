using System.Collections.Generic;
using NthDimension.CalcEngine.Functions;

namespace NthDimension.CalcEngine.Expressions
{
    /// <summary>
    /// Function call expression, e.g. sin(0.5)
    /// </summary>
    class FunctionExpression : Expression
    {
        // ** fields
        FunctionDefinition _fn;
        List<Expression> _parms;

        // ** ctor
        internal FunctionExpression()
        {
        }
        public FunctionExpression(FunctionDefinition function, List<Expression> parms)
        {
            _fn = function;
            _parms = parms;
        }

        // ** object model
        override public object Evaluate()
        {
            return _fn.Function(_parms);
        }
        public override Expression Optimize()
        {
            bool allLits = true;
            if (_parms != null)
            {
                for (int i = 0; i < _parms.Count; i++)
                {
                    var p = _parms[i].Optimize();
                    _parms[i] = p;
                    if (p._token.Type != TokenType.LITERAL)
                    {
                        allLits = false;
                    }
                }
            }
            return allLits
                ? new Expression(this.Evaluate())
                : this;
        }
    }
}
