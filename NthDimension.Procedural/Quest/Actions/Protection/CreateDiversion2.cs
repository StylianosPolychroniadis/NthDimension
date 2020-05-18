using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Protection
{
    public class Protection_CreateDiversion2 : StartingActions
    {
        public Protection_CreateDiversion2()
        {
            name = "CreateDiversion2";
        }

        public override void GenerateActions()
        {
            WallsOrGates wall = new WallsOrGates();
            actions.Add(new GoTo(wall));
            actions.Add(new Damage(wall));
        }
    }
}
