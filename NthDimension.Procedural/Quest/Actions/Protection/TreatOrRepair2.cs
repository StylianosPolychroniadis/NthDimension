using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Protection
{
    public class Protection_TreatOrRepair2 : StartingActions
    {
        public Protection_TreatOrRepair2()
        {
            name = "TreatOrRepair2";
        }

        public override void GenerateActions()
        {
            WallsOrGates wall = new WallsOrGates();
            actions.Add(new GoTo(wall));
            actions.Add(new Repair(wall));
        }
    }
}
