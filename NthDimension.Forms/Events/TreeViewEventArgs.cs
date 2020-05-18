using NthDimension.Forms.Widgets;
using System;

namespace NthDimension.Forms.Events
{
    public class TreeViewEventArgs : EventArgs
    {

        private TreeNode node;
        private TreeViewAction action;

        public TreeViewEventArgs(TreeNode node)
        {
            this.node = node;
        }

        public TreeViewEventArgs(TreeNode node, TreeViewAction action) : this(node)
        {
            this.action = action;
        }

        public TreeViewAction Action
        {
            get { return action; }
        }

        public TreeNode Node
        {
            get { return node; }
        }
    }
}
