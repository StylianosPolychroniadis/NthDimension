using System.Collections.Generic;
using System.Reflection;

namespace NthDimension.CalcEngine.Expressions
{
    /// <summary>
    /// Helper used for building BindingExpression objects.
    /// </summary>
    class BindingInfo
    {
        public BindingInfo(string member, List<Expression> parms)
        {
            Name = member;
            Parms = parms;
        }
        public string Name { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public PropertyInfo PropertyInfoItem { get; set; }
        public List<Expression> Parms { get; set; }
    }
}
