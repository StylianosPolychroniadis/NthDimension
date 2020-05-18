using NthDimension.Forms.Widgets;
using System;

namespace NthDimension.Forms.Events
{
    public class TreeNodeMouseHoverEventArgs : EventArgs
    {
        private TreeNode node;

        #region Public Constructors
        public TreeNodeMouseHoverEventArgs(TreeNode node) : base()
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
