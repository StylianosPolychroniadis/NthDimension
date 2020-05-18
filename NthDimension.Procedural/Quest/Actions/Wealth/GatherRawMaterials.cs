using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Wealth
{
    public class Wealth_GatherRawMaterials : StartingActions
    {
        public Wealth_GatherRawMaterials()
        {
            this.name = "GatherRawMaterials";
        }

        override public void GenerateActions()
        {
            Item i = new Item();
            actions.Add(new GoTo(i));
            actions.Add(new Get(i));
        }
    }
}
