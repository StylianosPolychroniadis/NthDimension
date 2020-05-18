using System;
using System.IO;
using NthDimension.Forms.Widgets;

namespace NthStudio.Gui.Widgets.Source.Tree
{
    public class SolutionNode : FileBasedNode
    {
        static private ContextMenuStrip ctxMenu;

        public SolutionNode(string fileName)
            : this(fileName, Path.GetFileNameWithoutExtension(fileName))
        {
        }

        public SolutionNode(string fileName, string solName)
            : base(fileName)
        {
            Name = solName;
            Text = "Solution: " + Name;

            // Create the ContextMenuStrip.
            ctxMenu = new ContextMenuStrip();

            //Create some menu items.
            var addMenu = new MenuStripItem("Add", true);
            var newProjectMenu = new MenuStripItem("New Project", true);
            //newProjectMenu.Click += NewProjectClick;
            //addMenu.DropDownItems.AddRange(new MenuStripItem[] { newProjectMenu });

            var deleteLabel = new MenuStripItem("Delete", true);
            var renameLabel = new MenuStripItem("Rename", true);

            //Add the menu items to the menu.
            ctxMenu.Widgets.Add(addMenu);
            ctxMenu.Widgets.Add(deleteLabel);
            ctxMenu.Widgets.Add(renameLabel);

            // Set the ContextMenuStrip property to the ContextMenuStrip.
            this.ContextMenuStrip = ctxMenu;
        }

        void NewProjectClick(object sender, EventArgs e)
        {
            //TreeViewSolution tvs = this.TreeView as TreeViewSolution;

            //tvs.ProjectFileNew();
        }
    }
}
