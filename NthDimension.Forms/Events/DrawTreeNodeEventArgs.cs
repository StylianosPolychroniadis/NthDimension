using NthDimension.Forms.Widgets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Events
{
    public class DrawTreeNodeEventArgs : EventArgs
    {
        private Rectangle bounds;
        private bool draw_default;
        private GContext graphics;
        private TreeNode node;
        private TreeNodeStates state;

        #region Public Constructors
        public DrawTreeNodeEventArgs(GContext graphics, TreeNode node,
                                      Rectangle bounds, TreeNodeStates state)
        {
            this.bounds = bounds;
            this.draw_default = false;
            this.graphics = graphics;
            this.node = node;
            this.state = state;
        }
        #endregion // Public Constructors

        #region Public Instance Properties
        public Rectangle Bounds
        {
            get { return bounds; }
        }

        public bool DrawDefault
        {
            get { return draw_default; }
            set { draw_default = value; }
        }

        public GContext Graphics
        {
            get { return graphics; }
        }

        public TreeNode Node
        {
            get { return node; }
        }

        public TreeNodeStates State
        {
            get { return state; }
        }
        #endregion // Public Instance Properties
    }
}
