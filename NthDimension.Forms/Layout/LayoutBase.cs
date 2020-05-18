using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Layout
{
    public abstract class LayoutBase
    {
        protected LayoutBase(Widget owner)
        {
            this.Owner = owner;
        }

        public Widget Owner
        {
            get;
            protected set;
        }

        public abstract void DoLayout();

        public override string ToString()
        {
            return string.Format("[LayoutBase: owner widget is {0}]", Owner);
        }
    }
}
