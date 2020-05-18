using NthDimension.Forms;
using NthDimension.Forms.Dialogs;
using NthStudio.Gui.NodeGraph.Model;
using NthStudio.Gui.NodeGraph.Type;
using NthStudio.NodeGraph;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Dialogs
{
    public class NodeGraphDialog : DialogBase
    {
        private bool m_init;

        private NodeGraphPanel m_toolbox;
        private NodeGraphPanel m_nodeGraph;
        private NodeGraphPanel m_propertyGrid;

        public override ImageList ImgList
        {
            get { return null; }
            set { }
        }

        public NodeGraphDialog()
        {
            this.Size = new System.Drawing.Size(StudioWindow.Instance.ClientRectangle.Width - 50,
                ClientRect.Height - 50);//new System.Drawing.Size(800, 600);

            

        }


        private void InitializeComponent()
        {

            m_toolbox                           = new NodeGraphPanel(this);
            m_toolbox.Size                      = new System.Drawing.Size(200, this.Height);
            m_toolbox.Dock                      = EDocking.Left;

            this.Widgets.Add(this.m_toolbox);

            m_nodeGraph                         = new NodeGraphPanel(this);
            m_nodeGraph.Location                = new System.Drawing.Point(0, 0);
            m_nodeGraph.Size                    = this.Size;

            m_nodeGraph.ConnectorHitZoneBleed   = 10;

            m_nodeGraph.DisplayLayer            = NodeGraphPanel.enuFloatingTextLayers.AllPaths;
            m_nodeGraph.DrawShadow              = true;
            m_nodeGraph.Dock                    = EDocking.Fill;
            m_nodeGraph.EditMode                = enuNodeGraphEditMode.Idle;
            m_nodeGraph.GridAlpha               = 32;
            m_nodeGraph.GridPadding             = 32;
            m_nodeGraph.LinkHardness            = 2f;       // was 2f
            m_nodeGraph.LinkPenWidth            = 3F;
            m_nodeGraph.NodeHeaderSize          = 20;
            m_nodeGraph.ShowGrid                = true;
            m_nodeGraph.SmoothBehavior          = false;
            m_nodeGraph.EnableDrawDebug         = true;
            m_nodeGraph.ToolbarMode             = enuToolbarEditMode.Idle;
            m_nodeGraph.UseLinkColoring         = true;
            m_nodeGraph.ShowBoundsLines         = true;

            this.Widgets.Add(m_nodeGraph);

            this.registerGraphNodeTypes();

            m_nodeGraph.MouseDownEvent  += delegate (Widget sender, NthDimension.Forms.Events.MouseDownEventArgs mea)
            { m_nodeGraph.NodeGraphPanel_MouseDown(sender, mea);

                if (mea.Button == NthDimension.MouseButton.Left)
                {
                    //MaterialNode mat = new MaterialNode(mea.X, mea.Y, m_nodeGraph.View, true);
                    BoolNode mat = new BoolNode(mea.X, mea.Y, m_nodeGraph.View, true);
                    m_nodeGraph.AddNode(mat);
                }
            };
            m_nodeGraph.MouseUpEvent    += delegate (object sender, NthDimension.Forms.Events.MouseEventArgs mea)
            { m_nodeGraph.NodeGraphPanel_MouseUp(sender, mea); };
            m_nodeGraph.MouseMoveEvent  += delegate(object sender, NthDimension.Forms.Events.MouseEventArgs mea)
            { m_nodeGraph.NodeGraphPanel_MouseMove(sender, mea); };
            m_nodeGraph.MouseWheelEvent += delegate(object sender, NthDimension.Forms.Events.MouseEventArgs e)
            { m_nodeGraph.NodeGraphPanel_MouseWheel(sender, e); };
            m_nodeGraph.KeyUpEvent      += delegate(object sender, NthDimension.Forms.Events.KeyEventArgs e)
            { m_nodeGraph.NodeGraphPanel_KeyUp(sender, e); };
            m_nodeGraph.KeyDownEvent    += delegate (object sender, NthDimension.Forms.Events.KeyEventArgs e)
            { m_nodeGraph.NodeGraphPanel_KeyDown(sender, e); };

            m_init = true;
        }

        private void registerGraphNodeTypes()
        {
            this.m_nodeGraph.View.UnRegisterAllDataTypes();
            this.m_nodeGraph.View.RegisterDataType(new NodeGraphDataTypeBool());
            this.m_nodeGraph.View.RegisterDataType(new NodeGraphDataTypeFloat());
            this.m_nodeGraph.View.RegisterDataType(new NodeGraphDataTypeMaterial());
        }

        protected override void DoPaint(GContext parentGContext)
        {
            if (!m_init)
                this.InitializeComponent();

            base.DoPaint(parentGContext);
        }
    }
}
