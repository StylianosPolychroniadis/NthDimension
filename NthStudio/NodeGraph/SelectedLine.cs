using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.NodeGraph
{
    public class SelectedLine
    {
        public SelectedLine(NodeGraphLink line, bool pointA, bool pointB)
        {
            this.Line = line;
            this.PointA = pointA;
            this.PointB = pointB;
        }

        public NodeGraphLink Line { get; set; }
        public bool PointA { get; set; }
        public bool PointB { get; set; }
    }
}
