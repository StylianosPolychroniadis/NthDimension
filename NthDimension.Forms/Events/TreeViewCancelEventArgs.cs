using NthDimension.Forms.Widgets;
using System.ComponentModel;

namespace NthDimension.Forms.Events
{
    public class TreeViewCancelEventArgs : CancelEventArgs
    {

        private TreeNode node;
        private TreeViewAction action;

        public TreeViewCancelEventArgs(TreeNode node, bool cancel, TreeViewAction action) : base(cancel)
        {
            this.node = node;
            this.action = action;
        }

        public TreeNode Node
        {
            get { return node; }
        }

        public TreeViewAction Action
        {
            get { return action; }
        }
    }
}
