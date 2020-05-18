using NthDimension.Forms.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms.Events
{
    public class TreeNodeMouseClickEventArgs : MouseEventArgs
    {
        private TreeNode node;

        #region Public Constructors
        public TreeNodeMouseClickEventArgs(TreeNode node, MouseButton button, int clicks, int x, int y)
            : base(button, clicks, x, y, 0)
        {
            this.node = node;
        }
        #endregion // Public Constructors

        #region Public Instance Properties
        public TreeNode Node
        {
            get { return this.node; }
        }
        #endregion // Public Instance Properties
    }
}
