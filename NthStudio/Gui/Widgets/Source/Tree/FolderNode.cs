using System;
using System.IO;
using NthDimension.Forms.Widgets;

namespace NthStudio.Gui.Widgets.Source.Tree
{
    public class FolderNode : FileBasedNode
    {
        static private ContextMenuStrip ctxMenu;

        public FolderNode(string fileName)
            : this(fileName, Path.GetFileName(fileName))
        {
        }

        public FolderNode(string fileName, string folName)
            : base(fileName)
        {
            Text = Name = folName;

            // Create the ContextMenuStrip.
            ctxMenu = new ContextMenuStrip();

            //Create some menu items.
            var openLabel = new MenuStripItem("Open", true);
            //openLabel.Click += Open_Click;
            var deleteLabel = new MenuStripItem("Delete", true);
            //deleteLabel.Click += Delete;
            var renameLabel = new MenuStripItem("Rename", true);
            //renameLabel.Click += Rename;

            //Add the menu items to the menu.
            ctxMenu.Widgets.Add(openLabel);
            ctxMenu.Widgets.Add(deleteLabel);
            ctxMenu.Widgets.Add(renameLabel);

            // Set the ContextMenuStrip property to the ContextMenuStrip.
            this.ContextMenuStrip = ctxMenu;
        }

        void Open_Click(object sender, EventArgs e)
        {
            /*MessageBox.Show("Folder '" + Name + "' Clicked.",
                            "Folder, open clik.",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);*/
        }

        protected void Rename(object sender, EventArgs e)
        {
            //TreeView.LabelEdit = true;
            //BeginEdit();
        }

        protected void Delete(object sender, EventArgs e)
        {
            //TreeView.Nodes.Remove(this);
        }
    }
}
