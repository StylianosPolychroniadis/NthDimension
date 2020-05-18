using System.Drawing;
using System.Linq;

using NthDimension.Forms;
using NthDimension.Forms.Dialogs;
using NthDimension.Forms.Widgets;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering;
using NthDimension.Rendering.Shaders;
using NthDimension.Rendering.Drawables.Framebuffers;
using NthDimension.Rendering.Utilities;
using System.Collections.Generic;
using NthDimension.Forms.Events;
using NthStudio.Gui.Widgets;
using NthStudio.Gui.Widgets.ObjectExplorer;
using NthStudio.Gui.Widgets.PropertyGrid;

namespace NthStudio.Gui.Dialogs
{
    public class CodeViewer : DialogBase
    {

        private TabbedDocs          m_editor;
        private ProjectExplorer     m_tree;
        private PropertyGrid m_properies;
        public override ImageList ImgList { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public CodeViewer()
        {
            Title = "Project1";
            Size = new Size(900, 600);
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BGColor = Color.DarkGray;

            m_editor = new TabbedDocs();
            m_editor.Name = "TextEditor";
            m_editor.Dock = EDocking.Fill;
            m_editor.BGColor = Color.FromArgb(10, 72, 75, 85);
            //m_editor.OpenFiles()

            var m_splitBarFromEditor = new SplitterBox(ESplitterType.HorizontalScroll);
            m_splitBarFromEditor.Name = "SplitBoxH";
            m_splitBarFromEditor.SplitterBarLocation = 0.2f;
            m_splitBarFromEditor.Size = new Size(400, 400);
            m_splitBarFromEditor.Dock = EDocking.Fill;
            m_splitBarFromEditor.PaintBackGround = false;

            //var m_splitBar = new SplitterBox(ESplitterType.VerticalScroll);
            //m_splitBar.Name = "SplitBoxV";
            //m_splitBar.SplitterBarLocation = 0f;
            //m_splitBar.Size = new Size(400, 400);
            //m_splitBar.Dock = EDocking.Fill;
            //m_splitBar.BGColor = Color.FromArgb(10, 72, 75, 85);
            //m_splitBar.PaintBackGround = false;

            m_tree = new ProjectExplorer();

            m_tree.Name = "TreeView";
            m_tree.BGColor = Color.FromArgb(10, 72, 75, 85);
            m_tree.FGColor = Color.LightGray;
            m_tree.Dock = EDocking.Fill;
            m_tree.ShowMarginLines = false;
            m_tree.Size = new Size(800, 600);
            m_tree.PaintBackGround = false;

            //m_properies = new PropertyGrid();
            //m_properies.Name = "Property Grid";
            //m_properies.Dock = EDocking.Fill;
            //m_properies.Size = new Size(800, 600);
            //m_properies.BGColor = new Color(); // Transparent

            ////m_properies.ShowBoundsLines = true;

            //m_splitBar.Panel0.Widgets.Add(m_tree);
            //m_splitBar.Panel1.Widgets.Add(m_properies);

            //m_splitBar.Panel0.BGColor = new Color();
            //m_splitBar.Panel1.BGColor = new Color();

            //m_splitBarFromEditor.Panel0.Widgets.Add(m_splitBar);
            m_splitBarFromEditor.Panel0.Widgets.Add(m_tree);
            m_splitBarFromEditor.Panel1.Widgets.Add(m_editor);

            this.Widgets.Add(m_splitBarFromEditor);

            string csTemplatePath = System.IO.Path.Combine(NthDimension.Utilities.DirectoryUtil.AssemblyDirectory, "data\\templates\\CSharpApplicationTemplate.cs");

            m_editor.OpenFiles(new string[]
            {
                csTemplatePath
            });

        }
    }
}
