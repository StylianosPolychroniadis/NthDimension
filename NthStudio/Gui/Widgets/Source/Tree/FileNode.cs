using System;
using System.IO;
using NthDimension.Forms.Widgets;

namespace NthStudio.Gui.Widgets.Source.Tree
{
    public class FileNode : FileBasedNode
    {
        static private ContextMenuStrip ctxMenu;

        public FileNode(string fileName)
            : this(fileName, Path.GetFileName(fileName))
        {
        }

        public FileNode(string fileName, string fileId)
            : base(fileName)
        {
            Text = Name = fileId;

            // Create the ContextMenuStrip.
            ctxMenu = new ContextMenuStrip();

            //Create some menu items.
            var openLabel = new MenuStripItem("Open", true);
            //openLabel.Click += Open_Click;
            var deleteLabel = new MenuStripItem("Delete", true);
            var renameLabel = new MenuStripItem("Rename", true);

            //Add the menu items to the menu.
            ctxMenu.Widgets.Add(openLabel);
            ctxMenu.Widgets.Add(deleteLabel);
            ctxMenu.Widgets.Add(renameLabel);

            // Set the ContextMenuStrip property to the ContextMenuStrip.
            this.ContextMenuStrip = ctxMenu;
        }

        void Open_Click(object sender, EventArgs e)
        {
            /*MessageBox.Show("File '" + Name + "' Clicked.",
                            "File, open clik.",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);*/
        }
    }
}
