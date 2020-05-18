using NthDimension.Forms.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.NodeGraph
{
    public interface INodeGraphPlug
    {
        bool CanProcess();
        int PlugIndex
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }


        NodeGraphNode ParentNode
        {
            get;
            set;
        }
        NodeViewState View
        {
            get;
            set;
        }

        enuOPERATORS PlugType
        {
            get;
            set;
        }



        NodeGraphDataType DataType
        {
            get;
            set;
        }



        Rectangle GetPlugArea();
        Rectangle GetHitArea();
        Point GetPlugTextPosition(PaintEventArgs e);
    }
}
