using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using NthDimension.Forms.Events;
using NthDimension.Forms.Layout;
using NthDimension.Forms.Widgets;

namespace NthDimension.Forms.Dialogs
{
    public delegate void OpenFilesDialogResultHandler(OpenFilesDialog ofd, EDialogResult dialogResult);

    /// <summary>
    /// Description of OpenFilesDialog.
    /// </summary>
    public class OpenFilesDialog : DialogBase
    {
        Widget panelTop;
        Widget panelBottom;
        TreeView treeView;
        SplitterBox iSplitterBox;
        readonly ListView listView;
        Button openFile;
        Button cancel;
        TextField iTextBox;

        readonly string[] filterExtensions;
        public string StartupPath = String.Empty;

        public OpenFilesDialog(string caption, string[] filterExtensions = null)
        {
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
            iTextBox.Size = new Size(0, (int)Math.Ceiling(iTextBox.Font.Height + 6));
            iTextBox.Dock = EDocking.Top;

            panelBottom.Widgets.Add(iTextBox);

            SuspendLayout();
            {
                openFile = new Button("Open");
                openFile.FGColor = Color.FromArgb(0, 96, 128);
                openFile.Anchor = EAnchorStyle.Bottom | EAnchorStyle.Right;
                cancel = new Button("Cancel");
                cancel.Anchor = EAnchorStyle.Bottom | EAnchorStyle.Right;

                openFile.Bounds = new Rectangle(panelBottom.Width - 240, panelBottom.Height - 10 - 28, 100, 28);
                cancel.Bounds = new Rectangle(panelBottom.Width - 110, panelBottom.Height - 10 - 28, 100, 28);

                panelBottom.Widgets.Add(openFile);
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

            treeView.Canvas.NodeMouseClick += NodeSelected;
            listView.SelectedIndexChanged += IListView_SelectedIndexChanged;
            cancel.MouseClickEvent += Cancel_MouseClickEvent;
            openFile.MouseClickEvent += OpenFile_MouseClickEvent;
        }

        private void IListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 1)
            {
                iTextBox.Text = "¡¡¡ OPEN ALL SELECTED FILES !!!";
            }
            else
            {
                var si = listView.SelectedItems[0];
                var fi = si.Tag as FileInfo;

                string str = Ellipsis.Compact(fi.FullName, iTextBox.Font, iTextBox.Width - 4, EllipsisFormat.Middle);
                iTextBox.Text = str;
            }
        }

        public string[] FilesNames
        {
            get;
            private set;
        }

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

        #region Test

        int cont;

        // add column

        private bool IsColumnExist(string headerText)
        {
            foreach (ListViewColumn cl in listView.Columns)
            {
                if (cl.HeaderText == headerText)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddColumn()
        {
            var column = new ListViewColumn();
            column.Width = 120;
            string name = "Column" + cont++;
            // let's see if this column exists
            if (IsColumnExist(name))
            {
                int i = 0;
                name = name + i;
                while (IsColumnExist(name))
                {
                    i++;
                    name = name + i;
                }
            }
            // add the column
            column.HeaderText = name;// the name that will apears in the control
            column.ID = name.ToLower();// you can set the id as you like, we set here the easiest thing to remember.
            column.SortMode = EListViewSortMode.None;
            listView.Columns.Add(column);

            listView.Invalidate();
        }

        // add item
        private void AddItem()
        {
            if (listView.Columns.Count == 0)
            {
                //MessageBox.Show("Please add column(s) first. In details view mode, adding items without adding columns will show nothing and the items will be ignored.");
                return;
            }
            var item = new ListViewItem();
            // In details view mode, the item text will not get shown, the subitems do instead
            // so here we need to setup subitems.
            // 1 Loop through columns to see what we need to add
            foreach (ListViewColumn column in listView.Columns)
            {
                // 2 Create the subitem and give it the column id. This is how the mlv control works, the subitems
                // must has the column id that should be listed for.
                var subitem = new ListViewSubItem();
                subitem.ColumnID = column.ID;// by this, the subitem will be shown and listed under the 'column'
                subitem.DrawMode = EListViewItemDrawMode.Text;// this demo is only for testing items and columns so let's keep it that way
                subitem.Text = "item-added" + ", id=" + column.ID;
                // 3 Add this subitem to the item
                item.SubItems.Add(subitem);
            }
            // 4 Now the item is ready to be added
            listView.Items.Add(item);

            listView.Invalidate();
        }
        #endregion Test

        private void Cancel_MouseClickEvent(object sender, EventArgs e)
        {
            Hide();
            userReturnDialog(this, EDialogResult.Cancel);
        }

        private void OpenFile_MouseClickEvent(object sender, MouseEventArgs mea)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            if (listView.SelectedItems.Count > 1)
            {
                FilesNames = new string[listView.SelectedItems.Count];
                int cont1 = 0;
                foreach (ListViewItem mlvi in listView.SelectedItems)
                {
                    var fi = mlvi.Tag as FileInfo;
                    FilesNames[cont1++] = fi.FullName;
                }
            }
            else
            {
                FilesNames = new string[1];
                var fi = listView.SelectedItems[0].Tag as FileInfo;
                FilesNames[0] = fi.FullName;
            }

            Hide();
            userReturnDialog(this, EDialogResult.Accept);
        }

        public override void Close()
        {
            base.Close();
            userReturnDialog(this, EDialogResult.Cancel);
        }

        OpenFilesDialogResultHandler userReturnDialog;

        public new void Show()
        {
            throw new Exception("For 'Dialogs' use 'Show(WHUD win, OpenFilesDialogResultHandler userReturnDialog)'");
        }

        public new void Show(WHUD win)
        {
            throw new Exception("For 'Dialogs' use 'Show(WHUD win, OpenFilesDialogResultHandler userReturnDialog)'");
        }

        public void Show(WHUD win, OpenFilesDialogResultHandler userReturnDialog)
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
                if (StartupPath.Length > 0)
                {
                    directory = new DirectoryInfo(StartupPath);
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
                    if (string.Compare(directory.FullName, StartupPath,
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
                if (string.Compare(directory.FullName, StartupPath,
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

            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                // drives
                DriveInfo[] drives = DriveInfo.GetDrives();
                for (int i = 0; i < drives.Length; i++)
                {
                    try
                    {
                        directory = new DirectoryInfo(drives[i].Name);
                        if (string.Compare(directory.FullName, StartupPath,
                                          StringComparison.InvariantCultureIgnoreCase) != 0)
                        {
                            DirectoryInfo[] diArray = directory.GetDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.ReparsePoint)).ToArray();;
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
                    catch(Exception)
                    {

                    }
                }
            }
            else
            {
                try
                {
                    // get the information of the directory MyComputer
                    directory = new DirectoryInfo("/");
                    if (string.Compare(directory.FullName, StartupPath,
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
                catch(Exception)
                {
                }
            }
        }

        void PopulateNode(TreeNode node)
        {
            var di = node.Tag as DirectoryInfo;

            if (di.FullName.ToLower().Contains("recycle.bin")) return;
#if OPTIMIZEDIO
            //DirectoryInfo[] diArray;
            List<DirectoryInfo> diArray = new List<DirectoryInfo>();
            List<DirectoryInfo> dArr = new List<DirectoryInfo>();
            string[] sdir = DirectoryAlternative.GetDirectories(di.FullName);

            foreach (string s in sdir)
            {
                var dd = new DirectoryInfo(s);
                if (!dd.Attributes.HasFlag(FileAttributes.ReparsePoint))
                    dArr.Add(new DirectoryInfo(s));
            }

            diArray.AddRange(dArr);
#else
            DirectoryInfo[] diArray = di.GetDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.ReparsePoint)).ToArray();
#endif

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
                catch (UnauthorizedAccessException aE)
                {
                    Rendering.Utilities.ConsoleUtil.log(string.Format("Warning: {0} Access Denied", di.Name));
                }
            }
        }

        void PopulateNode2(TreeNode node)
        {
            var di = node.Tag as DirectoryInfo;
#if OPTIMIZEDIO
            //DirectoryInfo[] diArray;
            List<DirectoryInfo> diArray = new List<DirectoryInfo>();
            List<DirectoryInfo> dArr = new List<DirectoryInfo>();
            string[] sdir = DirectoryAlternative.GetDirectories(di.FullName);

            foreach (string s in sdir)
            {
                var dd = new DirectoryInfo(s);
                if (!dd.Attributes.HasFlag(FileAttributes.ReparsePoint))
                    dArr.Add(new DirectoryInfo(s));
            }

            diArray.AddRange(dArr);
#else
            DirectoryInfo[] diArray = di.GetDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.ReparsePoint)).ToArray();
#endif

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

                    if (di != null && Directory.Exists(di.FullName))
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
            //foreach (FileInfo fi in DirectoryAlternative.GetFiles(di.FullName))
            if (!Directory.Exists(di.FullName)) return;
            string[] files = DirectoryAlternative.GetFiles(di.FullName);

            for (int f = 0; f < files.Length; f++)
            {
                FileInfo fi = new FileInfo(files[f]);
                string ext = Path.GetExtension(fi.Name).ToLower();

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
                StartupPath = di.Parent.FullName;

            listView.Invalidate();
        }

    }
}
