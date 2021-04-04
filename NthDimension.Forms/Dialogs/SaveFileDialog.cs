using System;
using System.Drawing;
using System.IO;
using System.Linq;
using NthDimension.Forms.Events;
using NthDimension.Forms.Layout;
using NthDimension.Forms.Widgets;


namespace NthDimension.Forms.Dialogs
{
    public delegate void SaveFileDialogResultHandler(SaveFileDialog sfd, EDialogResult dialogResult);
    public class SaveFileDialog : DialogBase
    {
        Widget                  panelTop;
        Widget                  panelBottom;
        TreeView                treeView;
        SplitterBox             iSplitterBox;
        readonly ListView       listView;
        Button                  saveFile;
        Button                  cancel;
        TextField               iTextBox;

        readonly string[]       filterExtensions;
        static string           lastFolder = String.Empty;

        public override ImageList ImgList
        {
            get
            {
                return treeView.Canvas.ImageList;
            }
            set
            {
                if (value == treeView.Canvas.ImageList)
                    return;
                treeView.Canvas.ImageList = value;
            }
        }

        public string FileName
        {
            get;
            private set;
        }

        //public SaveFileDialog(string caption, string[] filterExtesnsions = null)
        //{

        //}
        public SaveFileDialog(string caption, string filename = "", string[] filterExtensions = null)
        {
            if (!String.IsNullOrEmpty(filename))
                FileName = filename;

            string festr = string.Empty;

            if (filterExtensions != null && filterExtensions.Length > 0)
            {
                festr = filterExtensions.Length > 1 ? " (Filters: " : " (Filter: ";
                foreach (string fex in filterExtensions)
                    festr += "*." + fex + " | ";
                festr = festr.Substring(0, festr.Length - 3) + ")";
            }
            else
                festr = " (Filter: *.*)";

            Title = caption + festr;

            panelTop = new Widget();
            panelTop.Size = new Size(0, 40);
            panelTop.Dock = EDocking.Top;

            panelBottom = new Widget();
            panelBottom.Size = new Size(1000, 80);
            panelBottom.Dock = EDocking.Bottom;

            iTextBox = new TextField();
            iTextBox.FGColor = Color.WhiteSmoke;
            iTextBox.Font = new NanoFont(NanoFont.DefaultRegular, 12f);
            iTextBox.Margin = new Spacing(10, 10, 10, 0);
            iTextBox.Size = new Size(0, (int)System.Math.Ceiling(iTextBox.Font.Height + 6));
            iTextBox.Dock = EDocking.Top;

            panelBottom.Widgets.Add(iTextBox);

            SuspendLayout();
            {
                saveFile = new Button("Save");
                saveFile.FGColor = Color.FromArgb(0, 96, 128);
                saveFile.Anchor = EAnchorStyle.Bottom | EAnchorStyle.Right;
                cancel = new Button("Cancel");
                cancel.Anchor = EAnchorStyle.Bottom | EAnchorStyle.Right;

                saveFile.Bounds = new Rectangle(panelBottom.Width - 240, panelBottom.Height - 10 - 28, 100, 28);
                cancel.Bounds = new Rectangle(panelBottom.Width - 110, panelBottom.Height - 10 - 28, 100, 28);

                panelBottom.Widgets.Add(saveFile);
                panelBottom.Widgets.Add(cancel);
            }
            ResumeLayout();

            panelTop.BGColor = panelBottom.BGColor = Color.FromArgb(51, 51, 51);

            treeView = new TreeView();
            treeView.Dock = EDocking.Fill;
            treeView.FGColor = Color.LightGray;

            listView = new ListView();
            listView.Dock = EDocking.Fill;
            AddColumns();

            iSplitterBox = new SplitterBox(ESplitterType.HorizontalScroll);
            iSplitterBox.SplitterBarLocation = 0.3f;
            iSplitterBox.Dock = EDocking.Fill;
            iSplitterBox.Panel0.Widgets.Add(treeView);
            iSplitterBox.Panel1.Widgets.Add(listView);

            Widgets.Add(panelTop);
            Widgets.Add(panelBottom);
            Widgets.Add(iSplitterBox);

            // logic
            if (filterExtensions == null)
                this.filterExtensions = new string[1] { "*" };
            else
                this.filterExtensions = filterExtensions;

            //treeView.Canvas.NodeMouseClick              += NodeSelected;
            //listView.SelectedIndexChanged               += IListView_SelectedIndexChanged;
            //cancel.MouseClickEvent                      += Cancel_MouseClickEvent;
            //saveFile.MouseClickEvent                    += OpenFile_MouseClickEvent;
        }

        void AddColumns()
        {
            var column = new ListViewColumn();
            column.Width = 380;
            column.HeaderText = "File Name";
            column.ID = "file_name";
            column.SortMode = EListViewSortMode.AtoZ;
            listView.Columns.Add(column);

            column = new ListViewColumn();
            column.Width = 180;
            column.HeaderText = "Modification Date";
            column.ID = "modification_date";
            column.SortMode = EListViewSortMode.AtoZ;
            listView.Columns.Add(column);

            column = new ListViewColumn();
            column.Width = 120;
            column.HeaderText = "Size";
            column.ID = "file_size";
            column.SortMode = EListViewSortMode.AtoZ;
            listView.Columns.Add(column);

            listView.Invalidate();
        }



        public new void Show()
        {
            throw new Exception("For 'Dialogs' use 'Show(WHUD win, OpenFilesDialogResultHandler userReturnDialog)'");
        }

        public new void Show(WHUD win)
        {
            throw new Exception("For 'Dialogs' use 'Show(WHUD win, OpenFilesDialogResultHandler userReturnDialog)'");
        }

        SaveFileDialogResultHandler userReturnDialog;
        public void Show(WHUD win, SaveFileDialogResultHandler userReturnDialog)
        {
            if (userReturnDialog == null)
                throw new ArgumentNullException("userReturnDialog");

            this.userReturnDialog = userReturnDialog;

            base.Show(win);

            if (!IsVisible)
                return;

            treeView.Canvas.ShowMessageCentered = "Loading Directories...";
        }
        public override void OnShowDialog()
        {
            base.OnShowDialog();

            RunAfterNextPaintUpdate(LoadDirectories);
        }

        void LoadDirectories()
        {
            PopulateTree(treeView);
            treeView.Canvas.ShowMessageCentered = null;
        }

        void PopulateTree(TreeView pTreeView)
        {
            DirectoryInfo directory;
            TreeNode node;

            try
            {
                if (lastFolder.Length > 0)
                {
                    directory = new DirectoryInfo(lastFolder);
                    node = pTreeView.Nodes.Add(directory.Name);
                    node.Tag = directory;
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                    PopulateNode(node);
                }

                // get the information of the directory UserProfile
                string userProfilePath = Environment.GetEnvironmentVariable("USERPROFILE");
                if (userProfilePath != null)
                {
                    directory = new DirectoryInfo(userProfilePath);
                    if (string.Compare(directory.FullName, lastFolder,
                                      StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        node = pTreeView.Nodes.Add(directory.Name);
                        node.Tag = directory;
                        node.ImageIndex = 2;
                        node.SelectedImageIndex = 2;
                        PopulateNode(node);
                    }
                }
                // get the information of the directory MyDocuments
                directory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                if (string.Compare(directory.FullName, lastFolder,
                                  StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    node = pTreeView.Nodes.Add(directory.Name);
                    node.Tag = directory;
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                    PopulateNode(node);
                }
            }
            catch (UnauthorizedAccessException)
            {
                /*showMsgBox = true;
                winMsgBox = new MessageBox(GetCanvas(), "Unauthorized Access Exception");
                winMsgBox.SetPosition(rand.Next(700), rand.Next(400));
                winMsgBox.CloseButtonPressedEvent += new GwenEventHandler(winMsgBox_CloseButtonPressedEvent);*/
            }

            if (Environment.OSVersion.Platform != PlatformID.Unix && String.IsNullOrEmpty(FileName))
            {
                // drives
                DriveInfo[] drives = DriveInfo.GetDrives();
                for (int i = 0; i < drives.Length; i++)
                {
                    try
                    {
                        directory = new DirectoryInfo(drives[i].Name);
                        if (string.Compare(directory.FullName, lastFolder,
                                          StringComparison.InvariantCultureIgnoreCase) != 0)
                        {
                            DirectoryInfo[] diArray = directory.GetDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.ReparsePoint)).ToArray();
                            TreeNode dNode = pTreeView.Nodes.Add(drives[i].VolumeLabel +
                                                                 " (" + drives[i].Name.Remove(drives[i].Name.Length - 1) + ")");
                            dNode.Tag = directory;
                            dNode.ImageIndex = 2;
                            dNode.SelectedImageIndex = 2;

                            foreach (DirectoryInfo d in diArray)
                            {
                                TreeNode tn = dNode.Nodes.Add(d.Name);
                                tn.Tag = d;

                                PopulateNode(tn);
                            }
                        }
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                        /*winMsgBox = new MessageBox(GetCanvas(), "Unauthorized Access Exception");
                        winMsgBox.SetPosition(rand.Next(700), rand.Next(400));
                        winMsgBox.CloseButtonPressedEvent += new GwenEventHandler(winMsgBox_CloseButtonPressedEvent);*/
                    }
                }
            }
            else
            {
                try
                {
                    // get the information of the directory MyComputer
                    directory = new DirectoryInfo(String.IsNullOrEmpty(FileName) ? "/" : Path.GetDirectoryName(FileName));
                    if (string.Compare(directory.FullName, lastFolder,
                                      StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        node = pTreeView.Nodes.Add(directory.Name);
                        node.Tag = directory;
                        node.ImageIndex = 2;
                        node.SelectedImageIndex = 2;
                        PopulateNode(node);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        void PopulateNode(TreeNode node)
        {
            var di = node.Tag as DirectoryInfo;
            DirectoryInfo[] diArray;

            //diArray = di.GetDirectories();
            diArray = di.GetDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.ReparsePoint)).ToArray();

            foreach (DirectoryInfo d in diArray)
            {
                TreeNode tn = node.Nodes.Add(d.Name);
                tn.Tag = d;
                tn.ImageIndex = 2;
                tn.SelectedImageIndex = 2;
            }

            foreach (TreeNode tn2 in node.Nodes)
            {
                try
                {
                    PopulateNode2(tn2);
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        void PopulateNode2(TreeNode node)
        {
            var di = node.Tag as DirectoryInfo;
            //diArray = di.GetDirectories();
            DirectoryInfo[] diArray = di.GetDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.ReparsePoint)).ToArray();

            foreach (DirectoryInfo d in diArray)
            {
                TreeNode tn = node.Nodes.Add(d.Name);
                tn.Tag = d;
                tn.ImageIndex = 2;
                tn.SelectedImageIndex = 2;
            }
        }

        TreeNode lastNode;

        private void NodeSelected(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;

            if (lastNode == node)
                return;

            listView.Items.Clear();
            lastNode = node;

            foreach (TreeNode tn in node.Nodes)
            {
                try
                {
                    if (tn.Nodes.Count == 0)
                    {
                        PopulateNode(tn);
                    }
                    var di = tn.Tag as DirectoryInfo;

                    if (di != null)
                        PopulateFiles(di);
                }
                catch (UnauthorizedAccessException)
                {
                    /*showMsgBox = true;
                        winMsgBox = new MessageBox(GetCanvas(), "Unauthorized Access Exception");
                        winMsgBox.SetPosition(rand.Next(700), rand.Next(400));
                        winMsgBox.CloseButtonPressedEvent += new GwenEventHandler(winMsgBox_CloseButtonPressedEvent);*/
                }
            }
            //node.ExpandAll();
        }

        bool fileValid = false;

        void PopulateFiles(DirectoryInfo di)
        {
            //foreach (FileInfo fi in di.GetFiles())
            string[] files = DirectoryAlternative.GetFiles(di.FullName);

            for (int f = 0; f < files.Length; f++)
            {
                FileInfo fi = new FileInfo(files[f]); string ext = Path.GetExtension(fi.Name).ToLower();

                foreach (string fext in filterExtensions)
                {
                    string auxExt = "." + fext.ToLower();

                    if (fext == "*" || auxExt == ext)
                    {
                        fileValid = true;
                        break;
                    }
                }

                if (fileValid)
                {
                    float factor = 1024;
                    string sizeUnit = " KB";

                    if (fi.Length > 1048576)
                    {
                        factor = 1048576;
                        sizeUnit = " MB";
                    }
                    if (fi.Length > 1073741824)
                    {
                        factor = 1073741824;
                        sizeUnit = " GB";
                    }
                    float sizeKbytes = fi.Length / factor;

                    var item = new ListViewItem();
                    item.Tag = fi;

                    // File Name
                    var subitem = new ListViewSubItem();
                    // by this, the subitem will be shown and listed under the 'column'
                    subitem.ColumnID = listView.Columns[0].ID;
                    subitem.DrawMode = EListViewItemDrawMode.Text;
                    subitem.Text = fi.Name;
                    item.SubItems.Add(subitem);

                    // Modification Date
                    subitem = new ListViewSubItem();
                    // by this, the subitem will be shown and listed under the 'column'
                    subitem.ColumnID = listView.Columns[1].ID;
                    subitem.DrawMode = EListViewItemDrawMode.Text;
                    subitem.Text = fi.LastAccessTime.ToString();
                    item.SubItems.Add(subitem);

                    // File Type
                    subitem = new ListViewSubItem();
                    // by this, the subitem will be shown and listed under the 'column'
                    subitem.ColumnID = listView.Columns[2].ID;
                    subitem.DrawMode = EListViewItemDrawMode.Text;
                    subitem.Text = sizeKbytes.ToString("0.##") + sizeUnit;
                    item.SubItems.Add(subitem);

                    /*TableRow row = lb.AddRow(fi.LastAccessTime.ToString());
                    row.SetCellText(1, sizeKbytes.ToString("0.##") + sizeUnit);
                    row.SetCellText(2, fi.Name);
                    row.UserData = fi;*/

                    fileValid = false;

                    listView.Items.Add(item);
                }
            }

            if (di.Parent != null)
                lastFolder = di.Parent.FullName;

            listView.Invalidate();
        }
    }
}
