using System;
using System.IO;
using NthDimension.Forms;
using NthDimension.Forms.Dialogs;
using NthDimension.Forms.Widgets;

namespace NthStudio.Gui.Widgets.Source.Tree
{
    public class ProjectNode : FileBasedNode
    {
        static private ContextMenuStrip ctxMenu;

        public ProjectNode(string fileName)
            : this(fileName, Path.GetFileNameWithoutExtension(fileName))
        {
        }

        public ProjectNode(string fileName, string proName)
            : base(fileName)
        {
            Text = Name = proName;

            // Create the ContextMenuStrip.
            ctxMenu = new ContextMenuStrip();

            //Create some menu items.
            var addMenu = new MenuStripItem("Add", true);
            var newFileMenu = new MenuStripItem("New Source-file", true);
            //newFileMenu.Click += AddNewFileClick;
            var existFileMenu = new MenuStripItem("Exist Source-file", true);
            //existFileMenu.Click += AddExitsFileClick;

            var newFolderMenu = new MenuStripItem("New Folder", true);
            //newFolderMenu.Click += AddNewFolderClick;

            addMenu.MenuItems.Add(newFileMenu);
            addMenu.MenuItems.Add(existFileMenu);
            addMenu.MenuItems.Add(newFolderMenu);

            var deleteLabel = new MenuStripItem("Delete", true);
            var renameLabel = new MenuStripItem("Rename", true);

            //Add the menu items to the menu.
            ctxMenu.Widgets.Add(addMenu);
            ctxMenu.Widgets.Add(deleteLabel);
            ctxMenu.Widgets.Add(renameLabel);

            // Set the ContextMenuStrip property to the ContextMenuStrip.
            this.ContextMenuStrip = ctxMenu;
        }

        void AddNewFileClick(object sender, EventArgs e)
        {
            //TreeViewSolution tvs = this.TreeView as TreeViewSolution;

            //tvs.SourceFileNew(this);
        }

        void AddExitsFileClick(object sender, EventArgs e)
        {
            var fd = new OpenFilesDialog("Open Pulxar source-file", new string[] { "px" });
            ////fd.Multiselect = true;
            ////fd.Filter = "Silang source-file(*.sil)|*.sil";
            ////fd.InitialDirectory = this.PathFile;

            //fd.Show(null, HandleOpenFile);
            throw new NotImplementedException("Handle OpenFilesDialog");
            
        }

        void HandleOpenFile(OpenFilesDialog ofd, EDialogResult dialogResult)
        {
            if (dialogResult == EDialogResult.Cancel)
                return;

            foreach (String fName in ofd.FilesNames)
            {
                FileNode filNode = new FileNode(fName);

                foreach (FileBasedNode fbn in Nodes)
                {
                    if (String.Compare(fbn.Name, filNode.Name, true) == 0)
                    {
                        /*MessageBox.Show("File '" + fName + "' already loaded...",
                                        "File exist in project.",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);*/
                        break;
                    }
                }

                filNode.ImageIndex = 0;
                filNode.SelectedImageIndex = 0;
                this.Nodes.Add(filNode);
            }
        }

        void AddNewFolderClick(object sender, EventArgs e)
        {
            //TreeViewSolution tvs = this.TreeView as TreeViewSolution;

            //tvs.FolderNew(this);
        }
    }
}
