using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Equipment
{
    public class Equipment_Steal : StartingActions
    {
        public Equipment_Steal()
        {
            name = "Steal";
        }

        public override void GenerateActions()
        {
            Item i = new Item();
            actions.Add(new Steal(i));
        }
    }
}
