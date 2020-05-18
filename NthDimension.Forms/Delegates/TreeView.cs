using NthDimension.Forms.Events;

namespace NthDimension.Forms.Delegates
{
    public delegate void DrawTreeNodeEventHandler(object sender, DrawTreeNodeEventArgs e);
    public delegate void NodeLabelEditEventHandler(object sender, NodeLabelEditEventArgs e);
    public delegate void TreeNodeMouseClickEventHandler(object sender, TreeNodeMouseClickEventArgs e);
    public delegate void TreeNodeMouseHoverEventHandler(object sender, TreeNodeMouseHoverEventArgs e);
    public delegate void TreeViewCancelEventHandler(object sender, TreeViewCancelEventArgs e);
    public delegate void TreeViewEventHandler(object sender, TreeViewEventArgs e);
    public delegate void PaintEventHandler(object sender, PaintEventArgs e);
}
