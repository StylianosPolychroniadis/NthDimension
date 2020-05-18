using NthDimension.Forms.Widgets;
using System;

namespace NthDimension.Forms.Events
{
    public class NodeLabelEditEventArgs : EventArgs
    {
        private TreeNode node;
        private string label;
        private bool cancel;

        public NodeLabelEditEventArgs(TreeNode node)
        {
            this.node = node;
        }

        public NodeLabelEditEventArgs(TreeNode node, string label) : this(node)
        {
            this.label = label;
        }

        public bool CancelEdit
        {
            get { return cancel; }
            set
            {
                cancel = value;

                if (cancel)
                    node.EndEdit(true);
            }
        }

        public TreeNode Node
        {
            get { return node; }
        }

        public string Label
        {
            get { return label; }
        }

        internal void SetLabel(string label)
        {
            this.label = label;
        }
    }
}
