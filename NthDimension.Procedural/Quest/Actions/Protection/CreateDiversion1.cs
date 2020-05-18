using NthDimension.Procedural.Quest.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Procedural.Quest.Actions.Protection
{
    public class Protection_CreateDiversion1 : StartingActions
    {
        public Protection_CreateDiversion1()
        {
            name = "CreateDiversion1";
        }
        public override void GenerateActions()
        {
            Item i = new Item();
            WallsOrGates wall = new WallsOrGates();
            actions.Add(new Get(i));
            actions.Add(new GoTo(wall));
            actions.Add(new Use(i));
        }
    }
}
