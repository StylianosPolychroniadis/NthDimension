using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Wealth
{
    public class Wealth_MakeValuablesForResale : StartingActions
    {
        public Wealth_MakeValuablesForResale()
        {
            name = "MakeValuablesForResale";
        }

        public override void GenerateActions()
        {
            Item i = new Item();
            actions.Add(new Repair(i));
        }
    }
}
