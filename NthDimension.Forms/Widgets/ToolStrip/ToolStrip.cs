using NthDimension.Forms.Delegates;
using System;
using System.Drawing;

namespace NthDimension.Forms.Widgets
{
    public abstract class ToolStrip : Widget
    {
        internal static ToolStrip mainMenu;

        protected ToolStrip(EItemsAlignment pItemsAlign = EItemsAlignment.Horizontal)
        {
            if (mainMenu == null)
                mainMenu = this;

            ItemsAlign = pItemsAlign;
        }

        public EItemsAlignment ItemsAlign
        {
            get;
            private set;
        }
       
        internal IToolStripItem ActiveItem
        {
            get;
            set;
        }
    }

    public interface IToolStripItem 
    {
        event ItemClickedHandler ItemClickedEvent;

        string Text
        {
            get;
        }

        Color TextColor
        {
            get;
            set;
        }

        bool IsContextMenuHide
        {
            get;
        }

        void ShowMenu();

        void HideMenu();
    }
}
