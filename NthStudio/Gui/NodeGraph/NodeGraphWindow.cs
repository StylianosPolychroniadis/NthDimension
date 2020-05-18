using NthDimension.Forms;
using NthDimension.Forms.Widgets;
using NthStudio.Gui.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.NodeGraph
{
    public class NodeGraphWindow : NthDimension.Forms.Dialogs.DialogBase// NthDimension.Forms.Window
    {
        class NodeGraphToolbox : NthDimension.Forms.Panel
        {

            private bool m_init;

            private void InitializeComponent()
            {
                this.m_init = true;
            }
            protected override void DoPaint(NthDimension.Forms.GContext parentGContext)
            {
                if (!m_init)
                    this.InitializeComponent();

                base.DoPaint(parentGContext);
            }
        }
     
        class NodeGraphProperties : NthStudio.Gui.Widgets.PropertyGrid.PropertyGrid
        {
            private bool m_init;
            Widgets.PropertyGrid.DefaultObjectAdapter propertyAdapter;

            public NodeGraphProperties()
            {
                propertyAdapter = new Widgets.PropertyGrid.DefaultObjectAdapter();
                propertyAdapter.TargetPropertyGrid = this;
            }

            public void SelectNode(object node)
            {
                propertyAdapter.SelectedObject = (node);
                this.Repaint();
            }

            //private void InitializeComponent()
            //{
            //    m_init = true;
            //}
            //protected override void DoPaint(NthDimension.Forms.GContext parentGContext)
            //{
            //    if (!m_init)
            //        this.InitializeComponent();

            //    base.DoPaint(parentGContext);
            //}
        }

    
        private NodeGraphToolbox                            m_toolbox;
        private NodeGraphProperties                         m_properties;
        private NthStudio.NodeGraph.NodeGraphPanel          m_nodegraph;

        private RadioButton m_btnBool;
        private RadioButton m_btnIf;
        private RadioButton m_btnAnd;
        private RadioButton m_btnOr;
        private RadioButton m_btnXor;


        public override ImageList ImgList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public NodeGraphWindow()            
        {
            this.BorderStyle = enuBorderStyle.Sizable;
            this.IsDialog = false;
            //this.BGColor = System.Drawing.Color.Gray;
            this.PaintBackGround = true;
            this.ShowBoundsLines = true;
            
            this.InitializeComponent();

            
        }
        ~NodeGraphWindow()
        {
            //if (null != m_menu)
            //    if (!m_menu.IsHide)
            //    {
            //        m_menu.Hide();
            //        m_menu.Dispose();
            //    }
        }



        //public override void OnShowDialog()
        //{
        //    base.OnShowDialog();

        //    RunAfterNextPaintUpdate(InitializeComponent);
        //}

        private void InitializeComponent()
        {
            SuspendLayout();

            // TODO:: node icons in data\icons\nodes
            m_btnBool = new RadioButton("Boolean", string.Empty, Picture.enuRectangle.Normal) { Size = new System.Drawing.Size(80, 25), Dock = EDocking.Top, Checkable = true };
            m_btnIf = new RadioButton("If", string.Empty, Picture.enuRectangle.Normal) { Size = new System.Drawing.Size(80, 25), Dock = EDocking.Top, Checkable = true }; ;
            m_btnAnd = new RadioButton("And", string.Empty, Picture.enuRectangle.Normal) { Size = new System.Drawing.Size(80, 25), Dock = EDocking.Top, Checkable = true }; ;
            m_btnOr = new RadioButton("Or", string.Empty, Picture.enuRectangle.Normal) { Size = new System.Drawing.Size(80, 25), Dock = EDocking.Top, Checkable = true }; ;
            m_btnXor = new RadioButton("Xor", string.Empty, Picture.enuRectangle.Normal) { Size = new System.Drawing.Size(80, 25), Dock = EDocking.Top, Checkable = true }; ;

            this.m_toolbox = new NodeGraphToolbox();
            this.m_toolbox.Size = new System.Drawing.Size(200, this.Height);
            this.m_toolbox.Dock = NthDimension.Forms.EDocking.Fill;
            this.m_toolbox.PaintBackGround = true;
            this.m_toolbox.Widgets.AddRange(
                new RadioButton[] 
                {
                    m_btnBool,
                    m_btnIf,
                    m_btnAnd,
                    m_btnOr,
                    m_btnXor
                });
            //this.Widgets.Add(this.m_toolbox);

            //this.m_toolbox.Widgets.Add(new TimeLine() { Dock = EDocking.Fill });

            this.m_properties = new NodeGraphProperties();
            this.m_properties.Size = new System.Drawing.Size(300, this.Height);
            this.m_properties.Dock = NthDimension.Forms.EDocking.Fill;
            //this.Widgets.Add(this.m_properties);

            SplitterBox splitTools = new SplitterBox(ESplitterType.VerticalScroll);
            splitTools.Dock = EDocking.Fill;
            splitTools.Panel0.Widgets.Add(this.m_toolbox);
            splitTools.Panel1.Widgets.Add(this.m_properties);


            this.m_nodegraph = new NthStudio.NodeGraph.NodeGraphPanel();
            this.m_nodegraph.Size = new System.Drawing.Size(this.Width, this.Height - DEF_HEADER_HEIGHT);
            this.m_nodegraph.Location = new System.Drawing.Point(0, DEF_HEADER_HEIGHT);
            this.m_nodegraph.Dock = NthDimension.Forms.EDocking.Fill;            
            //this.Widgets.Add(this.m_nodegraph);
            //this.Widgets.Add(new Widgets.Slider());

            SplitterBox splitEditor = new SplitterBox(ESplitterType.HorizontalScroll);
            splitEditor.Dock = EDocking.Fill;
            splitEditor.Panel0.Widgets.Add(m_nodegraph);
            //splitEditor.Panel0.Size = new System.Drawing.Size((int)(splitEditor.Width * 0.4f), splitEditor.Panel1.Height);
            splitEditor.Panel1.Widgets.Add(splitTools);
            //splitEditor.Panel1.Size = new System.Drawing.Size((int)(splitEditor.Width * 0.4f), splitEditor.Panel1.Height);
            splitEditor.SplitterBarLocation = 0.80f;

            this.Widgets.Add(splitEditor);

            #region NodeGraph Configuration
            m_nodegraph.ConnectorHitZoneBleed   = 10;
            m_nodegraph.DisplayLayer            = NthStudio.NodeGraph.NodeGraphPanel.enuFloatingTextLayers.None;
            m_nodegraph.DrawShadow              = true;
            m_nodegraph.Dock                    = NthDimension.Forms.EDocking.Fill;
            m_nodegraph.EditMode                = NthStudio.NodeGraph.enuNodeGraphEditMode.Idle;
            m_nodegraph.GridAlpha               = 32;
            m_nodegraph.GridPadding             = 32;
            m_nodegraph.LinkHardness            = 2f;       // was 2f
            m_nodegraph.LinkPenWidth            = 3F;
            m_nodegraph.NodeHeaderSize          = 20;
            m_nodegraph.ShowGrid                = true;
            m_nodegraph.SmoothBehavior          = false;
            m_nodegraph.EnableDrawDebug         = true;
            m_nodegraph.ToolbarMode             = NthStudio.NodeGraph.enuToolbarEditMode.Idle;
            m_nodegraph.UseLinkColoring         = true;
            m_nodegraph.ShowBoundsLines         = true;



            #endregion NodeGraph Configuration


            ResumeLayout();

            this.registerGraphNodeTypes();

            m_nodegraph.onSelectionChanged    += delegate 
            { 
                if(m_nodegraph.View.SelectedItems.Count > 0)
                    this.m_properties.SelectNode(m_nodegraph.View.SelectedItems[0]); 
            };
            m_nodegraph.MouseDownEvent      += delegate (Widget sender, NthDimension.Forms.Events.MouseDownEventArgs mea)
            {
                m_nodegraph.NodeGraphPanel_MouseDown(sender, mea);

                if (mea.Button == NthDimension.MouseButton.Left
                    && m_nodegraph.EditMode == NthStudio.NodeGraph.enuNodeGraphEditMode.Idle ||
                       m_nodegraph.EditMode == NthStudio.NodeGraph.enuNodeGraphEditMode.Selecting)                   
                {
                    ////MaterialNode mat = new MaterialNode(mea.X, mea.Y, m_nodeGraph.View, true);
                    //Model.BoolNode mat = new Model.BoolNode(mea.X, mea.Y, m_nodegraph.View, true);
                    //m_nodegraph.AddNode(mat);
                    if(m_btnBool.Checked)
                    {
                        System.Drawing.Point wp = WHUD.LibContext.CursorPos;
                        //Model.BoolNode mat = new Model.BoolNode(mea.X, mea.Y, m_nodegraph.View, true);
                        Model.BoolNode mat = new Model.BoolNode(wp.X, wp.Y, m_nodegraph.View, true);
                        m_nodegraph.AddNode(mat);
                        m_btnBool.Checked = false;
                    }


                }
            };
            m_nodegraph.MouseUpEvent        += delegate (object sender, NthDimension.Forms.Events.MouseEventArgs mea)
            { m_nodegraph.NodeGraphPanel_MouseUp(sender, mea); };
            m_nodegraph.MouseMoveEvent      += delegate (object sender, NthDimension.Forms.Events.MouseEventArgs mea)
            { m_nodegraph.NodeGraphPanel_MouseMove(sender, mea); };
            m_nodegraph.MouseWheelEvent     += delegate (object sender, NthDimension.Forms.Events.MouseEventArgs e)
            { m_nodegraph.NodeGraphPanel_MouseWheel(sender, e); };
            m_nodegraph.KeyUpEvent          += delegate (object sender, NthDimension.Forms.Events.KeyEventArgs e)
            { m_nodegraph.NodeGraphPanel_KeyUp(sender, e); };
            m_nodegraph.KeyDownEvent        += delegate (object sender, NthDimension.Forms.Events.KeyEventArgs e)
            { m_nodegraph.NodeGraphPanel_KeyDown(sender, e); };
        
        }

        private void registerGraphNodeTypes()
        {
            this.m_nodegraph.View.UnRegisterAllDataTypes();
            this.m_nodegraph.View.RegisterDataType(new Type.NodeGraphDataTypeBool());
            this.m_nodegraph.View.RegisterDataType(new Type.NodeGraphDataTypeFloat());
            this.m_nodegraph.View.RegisterDataType(new Type.NodeGraphDataTypeMaterial());
        }
    }
}
