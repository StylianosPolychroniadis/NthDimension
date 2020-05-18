using System.Drawing;

namespace NthDimension.Forms.Layout
{

    // *** TODO *** Refactor rename to match file name

    public class DockAnchorLayout : LayoutBase
    {
        public DockAnchorLayout(Widget owner)
            : base(owner)
        {
        }

        //private void DoAnchoringLayout(Widget.WidgetList wlist)
        private void DoAnchoringLayout(WidgetList wlist)
        {
            if (null != wlist)
                foreach (Widget child in wlist)
                {
                    if (child.IsHide || child.Anchor == EAnchorStyle.None)
                        continue;

                    int x = child.AnchorSpaces.Left;
                    int y = child.AnchorSpaces.Top;
                    int width = child.Width;
                    int height = child.Height;

                    if (child.Anchor.HasFlag(EAnchorStyle.Bottom) && !child.Anchor.HasFlag(EAnchorStyle.Top))
                    {
                        y = Owner.Height - (child.AnchorSpaces.Bottom + child.Height);
                    }
                    if (child.Anchor.HasFlag(EAnchorStyle.Right) && !child.Anchor.HasFlag(EAnchorStyle.Left))
                    {
                        x = Owner.Width - (child.AnchorSpaces.Right + child.Width);
                    }
                    if (child.Anchor.HasFlag(EAnchorStyle.Top) && child.Anchor.HasFlag(EAnchorStyle.Bottom))
                    {
                        height = Owner.Height - (child.AnchorSpaces.Top + child.AnchorSpaces.Bottom);
                    }
                    if (child.Anchor.HasFlag(EAnchorStyle.Left) && child.Anchor.HasFlag(EAnchorStyle.Right))
                    {
                        width = Owner.Width - (child.AnchorSpaces.Left + child.AnchorSpaces.Right);
                    }

                    child.Location = new Point(x, y);
                    child.Size = new Size(width, height);
                }
        }

        //private void DoDockLayout(Widget.WidgetList wlist)
        private void DoDockLayout(WidgetList wlist)
        {
            Rectangle dockRect = Owner.ClientRect;
            var childBounds = new Rectangle();

            foreach (Widget child in wlist)
            {
                if (child.IsHide || child.Dock == EDocking.None)
                    continue;

                Spacing childMargin = child.Margin;

                switch (child.Dock)
                {
                    case EDocking.Top:
                        childBounds = new Rectangle(dockRect.X + childMargin.Left,
                                                    dockRect.Y + childMargin.Top,
                                                    dockRect.Width - childMargin.Left - childMargin.Right,
                                                    child.Height);
                        dockRect.Y += childBounds.Height + childMargin.Top + childMargin.Bottom;
                        dockRect.Height -= childBounds.Height + childMargin.Top + childMargin.Bottom;
                        break;

                    case EDocking.Left:
                        childBounds = new Rectangle(dockRect.X + childMargin.Left,
                                                    dockRect.Y + childMargin.Top,
                                                    child.Width,
                                                    dockRect.Height - childMargin.Top - childMargin.Bottom);
                        dockRect.X += childBounds.Width + childMargin.Left + childMargin.Right;
                        dockRect.Width -= childBounds.Width + childMargin.Left + childMargin.Right;
                        break;

                    case EDocking.Bottom:
                        childBounds = new Rectangle(dockRect.X + childMargin.Left,
                                                    dockRect.Y + (dockRect.Height - child.Height - childMargin.Bottom),
                                                    dockRect.Width - childMargin.Left - childMargin.Right,
                                                    child.Height);
                        dockRect.Height -= childBounds.Height + childMargin.Top + childMargin.Bottom;
                        break;

                    case EDocking.Right:
                        childBounds = new Rectangle(dockRect.X + (dockRect.Width - child.Width - childMargin.Right),
                                                    dockRect.Y + childMargin.Top,
                                                    child.Width,
                                                    dockRect.Height - childMargin.Top - childMargin.Bottom);
                        dockRect.Width -= childBounds.Width + childMargin.Left + childMargin.Right;
                        break;

                    case EDocking.Fill:
                        childBounds = new Rectangle(dockRect.X + childMargin.Left,
                                                    dockRect.Y + childMargin.Top,
                                                    dockRect.Width - childMargin.Left - childMargin.Right,
                                                    dockRect.Height - childMargin.Top - childMargin.Bottom);
                        break;
                }

                child.Location = childBounds.Location;
                child.Size = childBounds.Size;
            }
        }

        public override void DoLayout()
        {
            var winHUD = Owner as Widget.WHUD;

            DoDockLayout(Owner.Widgets);
            if (winHUD != null)
                DoDockLayout(winHUD.Overlays);

            DoAnchoringLayout(Owner.Widgets);
            if (winHUD != null)
                DoAnchoringLayout(winHUD.Overlays);
        }
    }
}
