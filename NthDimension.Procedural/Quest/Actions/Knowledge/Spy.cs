using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Knowledge
{
    public class Knowledge_Spy : StartingActions
    {
        public Knowledge_Spy()
        {
            name = "Spy";
        }
        public override void GenerateActions()
        {
            actions.Add(new Spy());
        }
    }
}
