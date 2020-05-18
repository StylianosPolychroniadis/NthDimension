using NthDimension.Forms;
using NthDimension.Forms.Dialogs;
using NthDimension.Forms.Widgets;
using NthDimension.Rendering.Scenegraph;
using NthStudio.Gui.Widgets;
using NthStudio.Gui.Widgets.Source.Tree;
using NthStudio.Gui.Widgets.TabStrip;
using NthStudio.Gui.Widgets.TextEditor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NthStudio.Gui.Displays
{
    public class WorkspaceDisplayController : Widget    // TODO:: populate from xml with xaml like-syntax
    {
        SolutionNode solNode;
        XmlDocument solutionXmlDoc;
        ImageList imgList;

        //Scene scene;

        //public Scene Scene
        //{
        //    get { return scene; }
        //}

        public WorkspaceDisplayController()
        {
            

            Size    = new Size(0, 45);
            BGColor = Color.FromArgb(32, 32, 66);
            Dock    = EDocking.Top;

            imgList = new ImageList();
            imgList.TransparentColor = Color.Transparent;

            //Utilities.ResourceUtil.GetImageResourceByName(imgList, 0, "solution16dark.png");
            //Utilities.ResourceUtil.GetImageResourceByName(imgList, 1, "project16dark.png");
            //Utilities.ResourceUtil.GetImageResourceByName(imgList, 2, "folder16dark.png");
            //Utilities.ResourceUtil.GetImageResourceByName(imgList, 3, "file16dark.png");

            TabStrip tabStrip = new TabStrip();
            tabStrip.Size = new Size(400, 400);
            tabStrip.Location = new Point(0,0);
            tabStrip.Dock = EDocking.Fill;
            tabStrip.BGColor = Color.Gray;

            TabStripItem tabCreate = new TabStripItem(new Label("Create"));
            tabCreate.Size = new Size(400,400);
            tabCreate.Location = new Point(0,0);

            #region primitives
            ToolButton btnPrimitive0 = new ToolButton(@".\data\icons\primitives\plane.png");
            btnPrimitive0.Size = new Size(22, 22);
            btnPrimitive0.Location = new Point(5, 1);
            btnPrimitive0.Font = new NanoFont(NanoFont.DefaultRegular, 6f);
            

            ToolButton btnPrimitive1 = new ToolButton(@".\data\icons\primitives\triangle.png");
            btnPrimitive1.Size = new Size(22, 22);
            btnPrimitive1.Location = new Point(5 + 22 + 1, 1);
            btnPrimitive1.Font = new NanoFont(NanoFont.DefaultRegular, 6f);

            ToolButton btnPrimitive2 = new ToolButton(@".\data\icons\primitives\cube.png");
            btnPrimitive2.Size = new Size(22, 22);
            btnPrimitive2.Location = new Point(5 + 44 + 2, 1);
            btnPrimitive2.Font = new NanoFont(NanoFont.DefaultRegular, 6f);

            ToolButton btnPrimitive3 = new ToolButton(@".\data\icons\primitives\cylinder.png");
            btnPrimitive3.Size = new Size(22, 22);
            btnPrimitive3.Location = new Point(5 + 66 + 3, 1);
            btnPrimitive3.Font = new NanoFont(NanoFont.DefaultRegular, 6f);

            ToolButton btnPrimitive4 = new ToolButton(@".\data\icons\primitives\capsule.png");
            btnPrimitive4.Size = new Size(22, 22);
            btnPrimitive4.Location = new Point(5 + 88 + 4, 1);
            btnPrimitive4.Font = new NanoFont(NanoFont.DefaultRegular, 6f);

            ToolButton btnPrimitive5 = new ToolButton(@".\data\icons\primitives\tank.png");
            btnPrimitive5.Size = new Size(22, 22);
            btnPrimitive5.Location = new Point(5 + 110 + 5, 1);
            btnPrimitive5.Font = new NanoFont(NanoFont.DefaultRegular, 6f);

            ToolButton btnPrimitive6 = new ToolButton(@".\data\icons\primitives\cone.png");
            btnPrimitive6.Size = new Size(22, 22);
            btnPrimitive6.Location = new Point(5 + 132 + 6, 1);
            btnPrimitive6.Font = new NanoFont(NanoFont.DefaultRegular, 6f);

            ToolButton btnPrimitive7 = new ToolButton(@".\data\icons\primitives\prism.png");
            btnPrimitive7.Size = new Size(22, 22);
            btnPrimitive7.Location = new Point(5 + 154 + 7, 1);
            btnPrimitive7.Font = new NanoFont(NanoFont.DefaultRegular, 6f);

            ToolButton btnPrimitive8 = new ToolButton(@".\data\icons\primitives\tube.png");
            btnPrimitive8.Size = new Size(22, 22);
            btnPrimitive8.Location = new Point(5 + 178 + 8, 1);
            btnPrimitive8.Font = new NanoFont(NanoFont.DefaultRegular, 6f);

            ToolButton btnPrimitive9 = new ToolButton(@".\data\icons\primitives\torus.png");
            btnPrimitive9.Size = new Size(22, 22);
            btnPrimitive9.Location = new Point(5 + 200 + 9, 1);
            btnPrimitive9.Font = new NanoFont(NanoFont.DefaultRegular, 6f);

            ToolButton btnPrimitive10 = new ToolButton(@".\data\icons\primitives\icosahedron.png");
            btnPrimitive10.Size = new Size(22, 22);
            btnPrimitive10.Location = new Point(5 + 222 + 10, 1);
            btnPrimitive10.Font = new NanoFont(NanoFont.DefaultRegular, 6f);

            tabCreate.Widgets.Add(btnPrimitive0);
            tabCreate.Widgets.Add(btnPrimitive1);
            tabCreate.Widgets.Add(btnPrimitive2);
            tabCreate.Widgets.Add(btnPrimitive3);
            tabCreate.Widgets.Add(btnPrimitive4);
            tabCreate.Widgets.Add(btnPrimitive5);
            tabCreate.Widgets.Add(btnPrimitive6);
            tabCreate.Widgets.Add(btnPrimitive7);
            tabCreate.Widgets.Add(btnPrimitive8);
            tabCreate.Widgets.Add(btnPrimitive9);
            tabCreate.Widgets.Add(btnPrimitive10);
            #endregion primitives

            TabStripItem tabTransform = new TabStripItem(new Label("Transform"));
            tabTransform.Size = new Size(400, 400);
            tabTransform.Location = new Point(0, 0);

            #region transformations
            ToolButton btnSelect = new ToolButton("Select", @".\data\icons\transforms\select.png");
            btnSelect.Size = new Size(80, 22);
            btnSelect.Location = new Point(5 + 0, 1);
            btnSelect.Font = new NanoFont(NanoFont.DefaultRegular, 6f);
           


            RadioButton btnWorld = new RadioButton("World", @".\data\icons\transforms\world.png");
            btnWorld.Size = new Size(80, 22);
            btnWorld.Location = new Point(5 + 80 + 1, 1);
            btnWorld.Font = new NanoFont(NanoFont.DefaultRegular, 6f);
            btnWorld.Checkable = true;
            btnWorld.Checked = true;

            RadioButton btnLocal = new RadioButton("Local", @".\data\icons\transforms\local.png");
            btnLocal.Size = new Size(80, 22);
            btnLocal.Location = new Point(5 + 80 + 1, 1);
            //btnLocal.Location = new Point(5 + 80 + 70 + 4, 1);
            btnLocal.Font = new NanoFont(NanoFont.DefaultRegular, 6f);
            btnLocal.Checkable = true;
            btnLocal.Checked = false;
            btnLocal.Hide();
            

            btnWorld.OnCheckedChanged += delegate
            {
                if (btnWorld.Checked)
                {
                    btnLocal.Hide();
                    btnLocal.Checked = false;
                    btnLocal.Invalidate();
                    btnWorld.Show();
                }
                else
                    btnLocal.Checked = true;
            };

            btnLocal.OnCheckedChanged += delegate
            {
                if (btnLocal.Checked)
                {
                    btnWorld.Hide();
                    btnWorld.Checked = false;
                    btnWorld.Invalidate();
                    btnLocal.Show();
                }
                else
                    btnWorld.Checked = true;
            };

            ToolButton btnTranslate = new ToolButton(@".\data\icons\transforms\translate.png");
            btnTranslate.Size = new Size(22, 22);
            btnTranslate.Location = new Point(5 + 80 + 80 + 2);
            btnTranslate.Font = new NanoFont(NanoFont.DefaultRegular, 6f);

            tabTransform.Widgets.Add(btnSelect);
            tabTransform.Widgets.Add(btnWorld);
            tabTransform.Widgets.Add(btnLocal);
            tabTransform.Widgets.Add(btnTranslate);

            #endregion transformations

            tabStrip.Items.Add(tabCreate);
            tabStrip.Items.Add(tabTransform);

            this.Widgets.Add(tabStrip);

            //scene = new Scene();
           

        }

        #region Menu-Items-Slots

        MenuStripItem MsiSaveFile_;
        public MenuStripItem MsiSaveFile
        {
            get { return MsiSaveFile_; }
            set
            {
                if (value == MsiSaveFile_)
                    return;
                MsiSaveFile_ = value;
                MsiSaveFile_.IsItemEnable = false;
                MsiSaveFile_.ItemClickedEvent += delegate (object sender, EventArgs ea)
                {
                    if (TabbedTextEditor == null)
                        return;
                    if (TabbedTextEditor.SelectedItem != null && TabbedTextEditor.SelectedItem.Selected == true)
                    {
                        TextEditor te = TabbedTextEditor.ActiveEditor;

                        if (te != null && TabbedTextEditor.IsModified(te))
                            DoSave(te);
                    }
                    //saveAll.Enabled = AnyItemModified();
                };
            }
        }

        MenuStripItem MsiCloseFile_;
        public MenuStripItem MsiCloseFile
        {
            get { return MsiCloseFile_; }
            set
            {
                if (value == MsiCloseFile_)
                    return;
                MsiCloseFile_ = value;
                MsiCloseFile_.IsItemEnable = false;
                MsiCloseFile_.ItemClickedEvent += delegate (object sender, EventArgs ea)
                {
                    //var ofd = new OpenFilesDialog("Open Files"); //, new string[] { "cs", "pxpro" });
                    //ofd.Show(WindowHUD, ProcessOpenFile);
                };

            }
        }

        MenuStripItem MsiOpenFile_;
        public MenuStripItem MsiOpenFile
        {
            get { return MsiOpenFile_; }
            set
            {
                if (value == MsiOpenFile_)
                    return;
                MsiOpenFile_ = value;
                MsiOpenFile.ItemClickedEvent += delegate (object sender, EventArgs ea)
                {
                    var ofd = new OpenFilesDialog("Open Files"); //, new string[] { "cs", "pxpro" });
                    ofd.Show(WindowHUD, HandleOpenFile);
                };
            }
        }

        void HandleOpenFile(OpenFilesDialog ofd, EDialogResult dialogResult)
        {
            if (dialogResult == EDialogResult.Accept)
            {
                TabbedTextEditor.OpenFiles(ofd.FilesNames);
            }
        }

        MenuStripItem MsiOpenProSolFile_;
        public MenuStripItem MsiOpenProSolFile
        {
            get { return MsiOpenProSolFile_; }
            set
            {
                if (value == MsiOpenProSolFile_)
                    return;
                MsiOpenProSolFile_ = value;
                MsiOpenProSolFile_.ItemClickedEvent += delegate (object sender, EventArgs ea)
                {
                    var ofd = new OpenFilesDialog("Open Project or Solution File", new string[] { "pxsol", "pxpro" });
                    ofd.ImgList = imgList;
                    ofd.Show(WindowHUD, HandleOpenProSolFile);
                };
            }
        }

        void HandleOpenProSolFile(OpenFilesDialog ofd, EDialogResult dialogResult)
        {
            if (dialogResult != EDialogResult.Accept)
                return;

            string ext = Path.GetExtension(ofd.FilesNames[0]);

            if (ext == ".pxsol")
            {
                SolutionFileOpen(ofd.FilesNames[0]);
            }
            else if (ext == ".pxpro")
            {
                ProjectFileOpen(ofd.FilesNames[0]);
            }
            else
            {
                /*MessageBox.Show("Invalid Solution or Project File.",
								"Open Solution/Project Problem",
								MessageBoxButtons.OK,
								MessageBoxIcon.Information);*/
            }
        }

        void SolutionFileOpen(string fileName)
        {
            try
            {
                solutionXmlDoc = new XmlDocument();
                solutionXmlDoc.Load(fileName);
                if (solutionXmlDoc.DocumentElement == null)
                    return;
                string solName = solutionXmlDoc.DocumentElement.GetAttribute("name");
                solNode = new SolutionNode(fileName, solName);

                solNode.ImageIndex = 0;
                solNode.SelectedImageIndex = 0;
                SolutionTreeView.Nodes.Add(solNode);

                if (solutionXmlDoc.DocumentElement != null)
                    foreach (XmlElement node in solutionXmlDoc.DocumentElement.GetElementsByTagName("project"))
                    {
                        XmlElement xe = node["file"];
                        string proFileName = xe.GetAttribute("relpath")
                                             + xe.GetAttribute("name");
                        proFileName = Utilities.PathUtil.AdaptPathSeparator(proFileName);
                        ProjectFileOpen(Utilities.PathUtil.MergePaths(solNode.PathFile, proFileName));
                    }

                solNode.Expand();
            }
            catch/* (Exception ex)*/
            {
                /*MessageBox.Show(ex.Message,
								"Error open file solution '" + Path.GetFileName(fileName) + "'",
								MessageBoxButtons.OK,
								MessageBoxIcon.Information);*/
            }
            finally
            {
            }
            //closeSolution.Enabled = true;
            //newProject.Enabled = true;
        }

        private void PopulateXmlElement(FileBasedNode pNode, XmlElement xmlElem)
        {
            foreach (XmlElement nodeElement in xmlElem.GetElementsByTagName("folder"))
            {
                string fileId = nodeElement.GetAttribute("relpath")
                                + nodeElement.GetAttribute("name");
                fileId = Utilities.PathUtil.AdaptPathSeparator(fileId);
                FolderNode fiNode = new FolderNode(Utilities.PathUtil.MergePaths(pNode.PathFile, fileId));
                fiNode.ImageIndex = 2;
                fiNode.SelectedImageIndex = 2;
                pNode.Nodes.Add(fiNode);

                PopulateXmlElement(fiNode, nodeElement);
            }

            foreach (XmlElement nodeElement in xmlElem.GetElementsByTagName("file"))
            {
                string fileId = nodeElement.GetAttribute("relpath")
                                + nodeElement.GetAttribute("name");
                fileId = Utilities.PathUtil.AdaptPathSeparator(fileId);
                FileNode fiNode = new FileNode(Utilities.PathUtil.MergePaths(pNode.PathFile, fileId));
                fiNode.ImageIndex = 3;
                fiNode.SelectedImageIndex = 3;
                pNode.Nodes.Add(fiNode);
            }
        }

        private void ProjectFileOpen(string fileName)
        {
            if (!File.Exists(fileName))
            {
                /*MessageBox.Show("File '" + Path.GetFileName(fileName) + "' not exist.",
								"Error open file",
								MessageBoxButtons.OK,
								MessageBoxIcon.Information);*/

                return;
            }
            var projectXmlDoc = new XmlDocument();
            projectXmlDoc.Load(fileName);
            var proNode = new ProjectNode(fileName)
            {
                ImageIndex = 1,
                SelectedImageIndex = 1
            };
            solNode.Nodes.Add(proNode);

            XmlElement proEle = projectXmlDoc["project"];
            bool proExpand = proEle != null && bool.Parse(proEle.Attributes["expand"].Value);

            if (projectXmlDoc.DocumentElement != null)
            {
                PopulateXmlElement(proNode, projectXmlDoc.DocumentElement);
            }
            if (proExpand)
                proNode.Expand();
            else
                proNode.Collapse();

            //BeginUpdate();
            //Sort();
            //EndUpdate();
        }


        #endregion Menu-Items-Slots

        #region TabbedTextEditor-Slot

        TabbedDocs TabbedTextEditor_;
        public TabbedDocs TabbedTextEditor
        {
            get { return TabbedTextEditor_; }
            set
            {
                if (value == TabbedTextEditor_)
                    return;
                TabbedTextEditor_ = value;
                TabbedTextEditor_.TabStripItemSelectionChanged += TabbedTextEditor__TabStripItemSelectionChanged;
            }
        }

        void TabbedTextEditor__TabStripItemSelectionChanged(TabStripItemChangedEventArgs e)
        {
            if (e.Item.Title.EndsWith("*", StringComparison.InvariantCulture))
                MsiSaveFile.IsItemEnable = true;
            else
                MsiSaveFile.IsItemEnable = false;
        }
        #endregion TabbedTextEditor-Slot

        #region TreeView-Solution-Slot

        TreeView SolutionTreeView_;
        public TreeView SolutionTreeView
        {
            get { return SolutionTreeView_; }
            set
            {
                if (value == SolutionTreeView_)
                    return;
                SolutionTreeView_ = value;
                SolutionTreeView.Nodes.Clear();

                SolutionTreeView.Canvas.ImageList = imgList;
            }
        }
        #endregion TreeView-Solution-Slot

        #region Private-Methods

        private void SetModifiedFlag(TextEditor editor, bool flag)
        {
            if (TabbedTextEditor != null && TabbedTextEditor.IsModified(editor) != flag)
            {
                var si = TabbedTextEditor.SelectedItem;
                if (TabbedTextEditor.IsModified(editor))
                {
                    si.Title = si.Title.Substring(0, si.Title.Length - 1);
                    //MsiSaveFile.IsItemEnable = false;
                }
                else
                {
                    si.Title += "*";
                    //MsiSaveFile.IsItemEnable = true;
                }
            }
        }

        private bool DoSave(TextEditor editor)
        {
            if (string.IsNullOrEmpty(editor.FileName))
                return DoSaveAs(editor);
            else
            {
                try
                {
                    editor.SaveFile(editor.FileName);
                    SetModifiedFlag(editor, false);
                    return true;
                }
                catch /*(Exception ex)*/
                {
                    //MessageBox.Show(ex.Message, ex.GetType().Name);
                    return false;
                }
            }
        }

        private bool DoSaveAs(TextEditor editor)
        {
            /*SaveFileDialog saveFileDialog = new SaveFileDialog();

			saveFileDialog.FileName = editor.FileName;
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					editor.SaveFile(saveFileDialog.FileName);
					editor.Parent.Text = Path.GetFileName(editor.FileName);
					SetModifiedFlag(editor, false);

					// The syntax highlighting strategy doesn't change
					// automatically, so do it manually.
					editor.Document.HighlightingStrategy =
						HighlightingStrategyFactory.CreateHighlightingStrategyForFile(editor.FileName);
					return true;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, ex.GetType().Name);
				}
			}*/
            return false;
        }
        #endregion Private-Methods

        protected override void OnPaintBackground(GContext gc)
        {
            var nsb1 = new NanoSolidBrush(Color.FromArgb(66, 66, 66));
            var nsb2 = new NanoSolidBrush(Color.FromArgb(220, 220, 220)); //(Color.FromArgb(48, 48, 58));

            gc.PaintLinearGradientRect(nsb1, nsb2, ClientRect);
        }
    }
}
