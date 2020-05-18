using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Widgets.PropertyGrid
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IsDefaultPropertyEditorOfAttribute : Attribute
    {
        public readonly Type TargetType;

        public IsDefaultPropertyEditorOfAttribute(Type type)
        {
            TargetType = type;
        }
    }
}
