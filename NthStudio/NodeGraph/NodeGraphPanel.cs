using NthDimension;
using NthDimension.Forms;
using NthDimension.Forms.Delegates;
using NthDimension.Forms.Events;
using NthDimension.Forms.Widgets;
using NthStudio.NodeGraph.Xml;
using System;
using System.Collections.Generic;
            using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NthStudio.NodeGraph
{
    [Flags]
    public enum enuToolbarEditMode
    {
        Idle = 0,
        Scroll = 2,
        ZoomIn = 4,
        ZoomOut = 8,
        Select = 16,
        //SelectMulti = 32,
        Translate = 64,
        Link = 128

    }

    public enum enuNodeGraphEditMode
    {
        Idle,
        Scrolling,
        Zooming,
        Selecting,
        SelectingBox,
        MovingSelection,
        MovingPoint,
        Linking
    }

    public enum HitType
    {
        Nothing,
        Node,
        Plug,
        Line,
        Point
    }

    public class NodeGraphPanelSelectionEventArgs : EventArgs
    {
        public int NewSelectionCount;
        public NodeGraphPanelSelectionEventArgs(int count)
        {
            this.NewSelectionCount = count;
        }
    }

    public delegate void NodeGraphPanelSelectionEventHandler(object sender, NodeGraphPanelSelectionEventArgs args);
    public delegate void NodeGraphPanelLinkEventHandler(object sender, NodeGraphLinkEventArgs args);

    public class NodeGraphPanel : Panel//Widget
    {
        public delegate void PropertiesFor(NodeGraphNode node);

        public enum enuFloatingTextLayers
        {
            None,
            AllPaths,
            GasPath,
            FluidPath,
            AirPath

        }

        #region Designer

        private bool m_init = false; 

        [Browsable(false)]
        private NodeViewState m_oView;
        public Rectangle ViewSpaceRectangle;
        private int m_iScrollLastX = 0;
        private int m_iScrollLastY = 0;
        private NanoFont m_oDebugFont;
        private Point m_SelectBoxOrigin;
        private Point m_SelectBoxCurrent;
        private Point m_ViewSpaceCursorLocation;
        private int m_ScrollMargins = 32;
        private int m_ScrollMarginsValue = 10;
        private Point m_MoveLastPosition;
        private Point m_MoveLastPositionPoint;
        private int m_NodeHeaderSize = 24;
        private float m_LinkHardness = 2f;
        private bool m_bDrawDebug;
        private bool m_bUseLinkColoring = true;
        private Color m_NodeTextColor = Color.FromArgb(255, 255, 255, 255);
        private Color m_NodeTextShadowColor = Color.Black;//Color.FromArgb(128, 0, 0, 0);
        private Color m_NodeFillColor = Color.FromArgb(255, 128, 128, 128);
        private Color m_NodeFillSelectedColor = Color.FromArgb(255, 160, 128, 100);
        private Color m_NodeOutlineColor = Color.FromArgb(255, 180, 180, 180);
        private Color m_NodeOutlineSelectedColor = Color.FromArgb(255, 192, 160, 128);
        private Color m_ConnectorTextColor = Color.FromArgb(255, 64, 64, 64);
        private Color m_ConnectorFillColor = Color.FromArgb(255, 0, 0, 0);
        private Color m_NodeHeaderColor = Color.FromArgb(128, 0, 0, 0);
        private Color m_ConnectorOutlineColor = Color.FromArgb(255, 32, 32, 32);
        private Color m_ConnectorSelectedFillColor = Color.FromArgb(255, 32, 32, 32);
        private Color m_ConnectorOutlineSelectedColor = Color.FromArgb(255, 64, 64, 64);
        private Color m_SelectionFillColor = Color.FromArgb(64, 128, 90, 30);
        private Color m_SelectionOutlineColor = Color.FromArgb(192, 255, 180, 60);
        private Color m_LinkColor = Color.FromArgb(255, 180, 180, 180);
        private Color m_LinkEditableColor = Color.FromArgb(255, 64, 255, 0);
        private Color m_NodeSignalValidColor = Color.FromArgb(255, 0, 255, 0);
        private Color m_NodeSignalInvalidColor = Color.FromArgb(255, 255, 0, 0);
        private NanoFont m_NodeTitleFont;
        private NanoFont m_NodeConnectorFont;
        private NanoFont m_NodeScaledTitleFont;
        private NanoFont m_NodeScaledConnectorFont;
        private SolidBrush m_NodeText;
        private SolidBrush m_NodeTextShadow;
        private Pen m_NodeOutline;
        private Pen m_NodeOutlineSelected;
        private SolidBrush m_NodeFill;
        private SolidBrush m_NodeFillSelected;
        private SolidBrush m_NodeSignalValid;
        private SolidBrush m_NodeSignalInvalid;
        private bool m_bDrawShadow = true;
        private SolidBrush m_ConnectorText;
        private Pen m_ConnectorOutline;
        private SolidBrush m_ConnectorFill;
        private Pen m_ConnectorOutlineSelected;
        private SolidBrush m_ConnectorFillSelected;
        private int m_ConnectorHitZoneBleed = 10;
        private float m_fNodeConnectorTextZoomTreshold = 0.8f;
        private float m_fNodeTitleZoomThreshold = .5f;
        private Pen m_SelectionOutline;
        private SolidBrush m_SelectionFill;
        private Pen m_Link;
        private Pen m_LinkEditable;
        private Brush m_LinkArrow;
        private SolidBrush m_NodeHeaderFill;

        private NodeGraphPlug m_InputLink;
        private NodeGraphPlug m_OutputLink;
        private bool m_bAltPressed;
        private bool m_bShiftPressed;
        private bool m_bCtrlPressed;
        private int m_iGridPadding = 32;
        private bool m_bShowGrid = true;
        private byte m_iGridAlpha = (byte)32;
        private bool m_bSmoothBehavior = false;
        private float m_linkPenWidth = 1f;
        private IContainer components = null;

        private enuFloatingTextLayers m_displayLayer = enuFloatingTextLayers.GasPath;
        #endregion

        #region Members
        private enuNodeGraphEditMode m_eEditMode = enuNodeGraphEditMode.Idle;
        private enuToolbarEditMode m_toolbarMode = enuToolbarEditMode.Idle;

        public event NodeGraphPanelSelectionEventHandler onSelectionChanged;
        public event NodeGraphPanelSelectionEventHandler onSelectionCleared;
        public event NodeGraphPanelLinkEventHandler onLinkCreated;
        public event NodeGraphPanelLinkEventHandler onLinkDestroyed;
        public event PaintEventHandler onDrawBackground;
        public event PropertiesFor SetPropertiesFor;

        private MenuStripItem mProperties;
        private MenuStripItem mDelete;
        private ContextMenuStrip rMenu;

       

        


        #endregion

        #region Properties
        public NodeViewState View
        {
            get
            {
                return this.m_oView;
            }
            set
            {
                this.m_oView = value;
            }
        }
        [Category("NodeGraph Panel")]
        public float NodeTitleZoomThreshold
        {
            get
            {
                return this.m_fNodeTitleZoomThreshold;
            }
            set
            {
                this.m_fNodeTitleZoomThreshold = value;
            }
        }
        [Category("NodeGraph Panel")]
        public float NodeConnectorTextZoomTreshold
        {
            get
            {
                return this.m_fNodeConnectorTextZoomTreshold;
            }
            set
            {
                this.m_fNodeConnectorTextZoomTreshold = value;
            }
        }
        [Category("NodeGraph Panel")]
        public bool EnableDrawDebug
        {
            get
            {
                return this.m_bDrawDebug;
            }
            set
            {
                this.m_bDrawDebug = value;
            }
        }
        [Category("NodeGraph Panel")]
        public int NodeHeaderSize
        {
            get
            {
                return this.m_NodeHeaderSize;
            }
            set
            {
                this.m_NodeHeaderSize = value;
            }
        }
        [Category("NodeGraph Panel")]
        public int ConnectorHitZoneBleed
        {
            get
            {
                return this.m_ConnectorHitZoneBleed;
            }
            set
            {
                this.m_ConnectorHitZoneBleed = value;
            }
        }
        [Category("NodeGraph Panel")]
        public float LinkHardness
        {
            get
            {
                return this.m_LinkHardness;
            }
            set
            {
                if ((double)value < 1.0)
                {
                    this.m_LinkHardness = 1f;
                }
                else
                {
                    this.m_LinkHardness = value;
                }
            }
        }
        [Category("NodeGraph Panel")]
        public int GridPadding
        {
            get
            {
                return this.m_iGridPadding;
            }
            set
            {
                this.m_iGridPadding = value;
            }
        }
        [Category("NodeGraph Panel")]
        public bool ShowGrid
        {
            get
            {
                return this.m_bShowGrid;
            }
            set
            {
                this.m_bShowGrid = value;
            }
        }
        [Category("NodeGraph Panel")]
        public byte GridAlpha
        {
            get
            {
                return this.m_iGridAlpha;
            }
            set
            {
                this.m_iGridAlpha = value;
            }
        }
        [Category("NodeGraph Panel")]
        public bool DrawShadow
        {
            get
            {
                return this.m_bDrawShadow;
            }
            set
            {
                this.m_bDrawShadow = value;
            }
        }
        [Category("NodeGraph Panel")]
        public bool SmoothBehavior
        {
            get
            {
                return this.m_bSmoothBehavior;
            }
            set
            {
                this.m_bSmoothBehavior = value;
            }
        }
        [Category("NodeGraph Panel")]
        public bool UseLinkColoring
        {
            get
            {
                return this.m_bUseLinkColoring;
            }
            set
            {
                this.m_bUseLinkColoring = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color NodeTextColor
        {
            get
            {
                return this.m_NodeTextColor;
            }
            set
            {
                this.m_NodeText = new SolidBrush(value);
                this.m_NodeTextColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color NodeTextShadowColor
        {
            get
            {
                return this.m_NodeTextShadowColor;
            }
            set
            {
                this.m_NodeTextShadow = new SolidBrush(value);
                this.m_NodeTextShadowColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color NodeFillColor
        {
            get
            {
                return this.m_NodeFillColor;
            }
            set
            {
                this.m_NodeFill = new SolidBrush(value);
                this.m_NodeFillColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color NodeFillSelectedColor
        {
            get
            {
                return this.m_NodeFillSelectedColor;
            }
            set
            {
                this.m_NodeFillSelected = new SolidBrush(value);
                this.m_NodeFillSelectedColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color NodeOutlineColor
        {
            get
            {
                return this.m_NodeOutlineColor;
            }
            set
            {
                this.m_NodeOutline = new Pen(value);
                this.m_NodeOutlineColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color NodeOutlineSelectedColor
        {
            get
            {
                return this.m_NodeOutlineSelectedColor;
            }
            set
            {
                this.m_NodeOutlineSelected = new Pen(value);
                this.m_NodeOutlineSelectedColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color NodeSignalValidColor
        {
            get
            {
                return this.m_NodeSignalValidColor;
            }
            set
            {
                this.m_NodeSignalValid = new SolidBrush(value);
                this.m_NodeSignalValidColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color NodeSignalInvalidColor
        {
            get
            {
                return this.m_NodeSignalInvalidColor;
            }
            set
            {
                this.m_NodeSignalInvalid = new SolidBrush(value);
                this.m_NodeSignalInvalidColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color ConnectorTextColor
        {
            get
            {
                return this.m_ConnectorTextColor;
            }
            set
            {
                this.m_ConnectorText = new SolidBrush(value);
                this.m_ConnectorTextColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color ConnectorFillColor
        {
            get
            {
                return this.m_ConnectorFillColor;
            }
            set
            {
                this.m_ConnectorFill = new SolidBrush(value);
                this.m_ConnectorFillColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color ConnectorOutlineColor
        {
            get
            {
                return this.m_ConnectorOutlineColor;
            }
            set
            {
                this.m_ConnectorOutline = new Pen(value);
                this.m_ConnectorOutlineColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color ConnectorFillSelectedColor
        {
            get
            {
                return this.m_ConnectorSelectedFillColor;
            }
            set
            {
                this.m_ConnectorFillSelected = new SolidBrush(value);
                this.m_ConnectorSelectedFillColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color ConnectorOutlineSelectedColor
        {
            get
            {
                return this.m_ConnectorOutlineSelectedColor;
            }
            set
            {
                this.m_ConnectorOutlineSelected = new Pen(value);
                this.m_ConnectorOutlineSelectedColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color SelectionFillColor
        {
            get
            {
                return this.m_SelectionFillColor;
            }
            set
            {
                this.m_SelectionFill = new SolidBrush(value);
                this.m_SelectionFillColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color SelectionOutlineColor
        {
            get
            {
                return this.m_SelectionOutlineColor;
            }
            set
            {
                this.m_SelectionOutline = new Pen(value);
                this.m_SelectionOutlineColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color NodeHeaderColor
        {
            get
            {
                return this.m_NodeHeaderColor;
            }
            set
            {
                this.m_NodeHeaderFill = new SolidBrush(value);
                this.m_NodeHeaderColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color LinkColor
        {
            get
            {
                return this.m_LinkColor;
            }
            set
            {
                this.m_Link = new Pen(value, 0.5f);
                this.m_LinkArrow = new SolidBrush(value);
                this.m_LinkColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color LinkEditableColor
        {
            get
            {
                return this.m_LinkEditableColor;
            }
            set
            {
                this.m_LinkEditable = new Pen(value, 1.5f);
                this.m_LinkEditableColor = value;
            }
        }
        [Browsable(false)]
        public SolidBrush NodeText
        {
            get
            {
                return this.m_NodeText;
            }
        }
        [Browsable(false)]
        public SolidBrush NodeTextShadow
        {
            get
            {
                return this.m_NodeTextShadow;
            }
        }
        [Browsable(false)]
        public SolidBrush NodeHeaderFill
        {
            get
            {
                this.m_NodeHeaderFill = new SolidBrush(this.m_NodeHeaderColor);
               

                return this.m_NodeHeaderFill;
            }
        }
        [Browsable(false)]
        public Pen NodeOutline
        {
            get
            {
                return this.m_NodeOutline = new Pen(this.m_NodeOutlineColor);
                
                //return this.m_NodeOutline;
            }
        }
        [Browsable(false)]
        public Pen NodeOutlineSelected
        {
            get
            {
                return this.m_NodeOutlineSelected = new Pen(m_NodeOutlineSelectedColor);
            }
        }
        [Browsable(false)]
        public SolidBrush NodeFill
        {
            get
            {
                //return this.m_NodeFill;
                return this.m_NodeFill = new SolidBrush(m_NodeFillColor);
                
            }
        }
        [Browsable(false)]
        public SolidBrush NodeFillSelected
        {
            get
            {
                return this.m_NodeFillSelected = new SolidBrush(m_NodeFillSelectedColor);
            }
        }
        [Browsable(false)]
        public SolidBrush NodeSignalValid
        {
            get
            {
                return this.m_NodeSignalValid = new SolidBrush(m_NodeSignalValidColor);
            }
        }
        [Browsable(false)]
        public SolidBrush NodeSignalInvalid
        {
            get
            {
                return this.m_NodeSignalInvalid = new SolidBrush(this.m_NodeSignalInvalidColor);
            }
        }
        [Browsable(false)]
        public SolidBrush ConnectorText
        {
            get
            {
                return this.m_ConnectorText = new SolidBrush(this.m_ConnectorTextColor); 
            }
        }
        [Browsable(false)]
        public Pen ConnectorOutline
        {
            get
            {
                return this.m_ConnectorOutline = new Pen(this.m_ConnectorOutlineColor);
            }
        }
        [Browsable(false)]
        public SolidBrush ConnectorFill
        {
            get
            {
                return this.m_ConnectorFill = new SolidBrush(this.m_ConnectorFillColor);
            }
        }
        [Browsable(false)]
        public Pen ConnectorOutlineSelected
        {
            get
            {
                return this.m_ConnectorOutlineSelected = new Pen(this.m_ConnectorOutlineSelectedColor);
            }
        }
        [Browsable(false)]
        public SolidBrush ConnectorFillSelected
        {
            get
            {
                return this.m_ConnectorFillSelected = new SolidBrush(this.m_ConnectorFillSelected.Color);
            }
        }
        [Category("NodeGraph Panel Fonts")]
        public NanoFont NodeTitleFont
        {
            get
            {
                return this.m_NodeTitleFont;
            }
            set
            {
                this.m_NodeTitleFont = value;
            }
        }
        [Category("NodeGraph Panel Fonts")]
        public NanoFont NodeConnectorFont
        {
            get
            {
                return this.m_NodeConnectorFont;
            }
            set
            {
                this.m_NodeConnectorFont = value;
            }
        }
        [Browsable(false)]
        public NanoFont NodeScaledTitleFont
        {
            get
            {
                return this.m_NodeScaledTitleFont;
            }
            set
            {
                this.m_NodeScaledTitleFont = value;
            }
        }
        [Browsable(false)]
        public NanoFont NodeScaledConnectorFont
        {
            get
            {
                return this.m_NodeScaledConnectorFont;
            }
            set
            {
                this.m_NodeScaledConnectorFont = value;
            }
        }
        public float LinkPenWidth
        {
            get
            {
                return this.m_linkPenWidth;
            }
            set
            {
                this.m_linkPenWidth = value;
            }
        }

        [Browsable(false), XmlIgnore]
        public enuFloatingTextLayers DisplayLayer
        {
            get { return m_displayLayer; }

            set { m_displayLayer = value; }
        }
        [Browsable(false), XmlIgnore]
        public enuToolbarEditMode ToolbarMode { get { return m_toolbarMode; } set { m_toolbarMode = value; } }
        [Browsable(false), XmlIgnore]
        public enuNodeGraphEditMode EditMode { get { return m_eEditMode; } set { m_eEditMode = value; } }
        #endregion



        #region Ctor
        public NodeGraphPanel()            
        {
            this.Size = new Size(100, 100);

            this.m_Link         = new Pen(m_LinkColor);
            this.m_LinkEditable = new Pen(m_LinkEditableColor);

            this.InitializeComponent();
           
            
            //mProperties = new MenuStripItem("Properties");
            //mProperties.MouseClickEvent += onProperties_Click;

            //mDelete = new MenuStripItem("Delete");
            //mDelete.MouseClickEvent += onDelete_Click;

            //rMenu = new ContextMenuStrip();
            //rMenu.Widgets.Add(mProperties);
            //rMenu.Widgets.Add(mDelete);

        }
        public NodeGraphPanel(Widget parent):this()
        {
            Parent = parent;
        }

        

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            //base.SuspendLayout();
            //base.AutoScaleDimensions = new SizeF(6f, 13f);
            ////base.AutoScaleMode = AutoScaleMode.Font;
            //BGColor = Color.FromArgb(67, 65, 64);
            Name = "NodeGraphPanel";
            //Size = new Size(100,100);// 536, 448);
            //Location = new Point(0, 0);

            this.InitializeNodegraphPanel(true);

            //base.Paint += new PaintEventHandler(this.NodeGraphPanel_Paint);
            //base.KeyDown += new KeyEventHandler(this.NodeGraphPanel_KeyDown);
            //base.KeyUp += new KeyEventHandler(this.NodeGraphPanel_KeyUp);
            //base.MouseDown += new MouseEventHandler(this.NodeGraphPanel_MouseDown);
            //base.MouseMove += new MouseEventHandler(this.NodeGraphPanel_MouseMove);
            //base.MouseUp += new MouseEventHandler(this.NodeGraphPanel_MouseUp);
            //base.MouseWheel += new MouseEventHandler(this.NodeGraphPanel_MouseWheel);
            //base.Resize += new EventHandler(this.NodeGraphPanel_Resize);
            //base.ResumeLayout(false);

            //this.MouseWheelEvent += delegate (object sender, MouseEventArgs e)
            //{
            //    NthDimension.Rendering.Utilities.ConsoleUtil.errorlog("Nodegraph", new Point(e.X, e.Y).ToString());
            //};

           
        }


        private void InitializeNodegraphPanel(bool resetView)
        {
            if (resetView || null == this.m_oView)
                this.m_oView = new NodeViewState(this);     // Used for De-serialization


            // Moved instanciation to class definition
            //this.m_ScrollMargins = 32; 
            //this.m_ScrollMarginsValue = 10;
            //this.NodeTitleZoomThreshold = 0.5f;
            //this.NodeConnectorTextZoomTreshold = 0.8f;

            //this.m_bShowGrid = false;
            //this.m_iGridPadding = 32;       // 256;
            //this.m_iGridAlpha = 32;        // 32;
            //this.m_bUseLinkColoring = true;


            //this.m_iScrollLastX = 0;
            //this.m_iScrollLastY = 0;

            //this.m_eEditMode = enuNodeGraphEditMode.Idle;
            //this.ToolbarMode = enuToolbarEditMode.Idle;

            //this.ConnectorHitZoneBleed = 10;        // was 2 -> Value Resets in FormUnitGraph.InititalizeComponent()
            //this.LinkHardness = 2f;       // was 2f
            //this.NodeHeaderSize = 0;//24;

            //this.NodeTextColor = Color.FromArgb(255, 255, 255, 255);
            //this.NodeTextShadowColor = Color.Black;//Color.FromArgb(128, 0, 0, 0);
            //this.NodeFillColor = Color.FromArgb(255, 128, 128, 128);
            //this.NodeFillSelectedColor = Color.FromArgb(255, 160, 128, 100);
            //this.NodeOutlineColor = Color.FromArgb(255, 180, 180, 180);
            //this.NodeOutlineSelectedColor = Color.FromArgb(255, 192, 160, 128);
            //this.ConnectorTextColor = Color.FromArgb(255, 64, 64, 64);
            //this.ConnectorFillColor = Color.FromArgb(255, 0, 0, 0);
            //this.ConnectorFillSelectedColor = Color.FromArgb(255, 32, 32, 32);
            //this.ConnectorOutlineColor = Color.FromArgb(255, 32, 32, 32);
            //this.ConnectorOutlineSelectedColor = Color.FromArgb(255, 64, 64, 64);
            //this.SelectionFillColor = Color.FromArgb(64, 128, 90, 30);
            //this.SelectionOutlineColor = Color.FromArgb(192, 255, 180, 60);
            //this.NodeHeaderColor = Color.FromArgb(128, 0, 0, 0);
            //this.LinkColor = Color.FromArgb(255, 180, 180, 180);
            //this.LinkEditableColor = Color.FromArgb(255, 64, 255, 0);
            //this.NodeSignalValidColor = Color.FromArgb(255, 0, 255, 0);
            //this.NodeSignalInvalidColor = Color.FromArgb(255, 255, 0, 0);


            this.m_oDebugFont = new NanoFont(NanoFont.DefaultRegular, 8f);
                       
            this.m_ViewSpaceCursorLocation = default(Point);
            this.m_bAltPressed = false;
            this.m_bCtrlPressed = false;
            this.m_NodeText = new SolidBrush(Color.White);
            this.m_NodeTextShadow = new SolidBrush(Color.Black);
           
            this.m_NodeTitleFont = new NanoFont(NanoFont.DefaultRegular, 10f);
            this.m_NodeConnectorFont = new NanoFont(NanoFont.DefaultRegular, 7f);
            this.m_NodeScaledTitleFont = new NanoFont(this.m_NodeTitleFont, this.m_NodeTitleFont.FontSize);
            this.m_NodeScaledConnectorFont = new NanoFont(this.m_NodeConnectorFont, this.m_NodeConnectorFont.FontSize);

            this.m_SelectBoxOrigin = default(Point);
            this.m_SelectBoxCurrent = default(Point);                   


            this.m_InputLink = null;
            this.m_OutputLink = null;

            this.EnableDrawDebug = false;
          
            this.m_oView.CurrentViewZoom = 1;

            m_init = true;

            //this.Dock = EDocking.Fill;
        }
        #endregion

        #region Context Menu Handlers
        private void onProperties_Click(object sender, MouseEventArgs mea)
        {
            throw new NotImplementedException();
            //if (this.View.SelectedItems.Count > 0 &&
            //    this.View.SelectedItems[0] is IHeatExchangerNode)
            //{
            //    HxsDialog hxs = new HxsDialog(this, (IHeatExchangerNode)this.View.SelectedItems[0]);
            //    DialogResult p = hxs.ShowDialog();

            //    if (p == DialogResult.OK && null != SetPropertiesFor)
            //        this.SetPropertiesFor(this.View.SelectedItems[0]);


            //}

            //if (this.View.SelectedItems.Count > 0 &&
            //    this.View.SelectedItems[0] is SourceNode)
            //{
            //    SourceDialog sdg = new SourceDialog(this, (SourceNode)this.View.SelectedItems[0]);
            //    DialogResult p = sdg.ShowDialog();


            //}

        }
        private void onDelete_Click(object sender, EventArgs eventArgs)
        {
            this.DeleteSelected();
        }
        #endregion Context Menu Handlers

        #region Mouse/Keyboard Event Handlers
        public void NodeGraphPanel_MouseDown(object sender, MouseEventArgs e)
        {
            enuNodeGraphEditMode eEditMode = this.m_eEditMode;

            if (eEditMode == enuNodeGraphEditMode.Idle)
            {
                
                MouseButton button = e.Button;
                HitType hitType = this.HitAll(new Point(e.X, e.Y));

                if (hitType == HitType.Node && button == MouseButton.Right)
                {
                    //this.rMenu.Show(this, new Point(e.X, e.Y));
                    //throw new NotImplementedException("Show(WHUD)");
                }

                if (hitType == HitType.Point)
                {
                    this.m_eEditMode = enuNodeGraphEditMode.MovingPoint;
                    this.m_MoveLastPositionPoint = this.ControlToView(e.Location);
                    return;
                }

                #region Scroll
                if (/*ToolbarMode == enuToolbarEditMode.Scroll ||*/ //Added ! to m_bShiftPressed
                    !m_bShiftPressed)
                    if (button == MouseButton.Right && hitType == HitType.Nothing)
                    {
                        #region Scroll

                        this.m_eEditMode = enuNodeGraphEditMode.Scrolling;
                        this.m_iScrollLastX = e.Location.X;
                        this.m_iScrollLastY = e.Location.Y;

                        #endregion
                    }
                #endregion

                #region Link
                if (/*ToolbarMode == enuToolbarEditMode.Link &&*/ button == MouseButton.Left)
                    //if (this.HitAll(e.Location) == HitType.Plug)
                    if (hitType == HitType.Plug)
                    {
                        if (!this.m_bAltPressed)
                        {


                            #region Start Linking
                            this.m_eEditMode = enuNodeGraphEditMode.Linking;
                            this.m_InputLink = this.GetHitPlug(e.Location);
                            this.m_OutputLink = null;
                            #endregion
                        }
                        else
                        {

                            #region Delete Link
                            NodeGraphPlug hitConnector = this.GetHitPlug(e.Location);
                            this.DeleteLinkConnectors(hitConnector);
                            #endregion
                        }
                    }
                #endregion

                #region Translate
                if (/*ToolbarMode == enuToolbarEditMode.Translate && */
                      button == MouseButton.Left &&
                    !m_bShiftPressed 
                    && false // TODO Remove 2020
                    )
                {
                    //if (this.View.SelectedItems.Count > 0 && this.HitSelected(e.Location) == HitType.Node)
                    if (this.View.SelectedItems.Count > 0 && hitType == HitType.Node)
                    {
                        #region Move Nodes
                        this.m_eEditMode = enuNodeGraphEditMode.MovingSelection;
                        this.m_MoveLastPosition = this.ControlToView(e.Location);
                        #endregion

                        return;
                    }




                }
                #endregion

                #region Select
                if (/*ToolbarMode == enuToolbarEditMode.Select &&*/ button == MouseButton.Left)
                {
                    if (System.Windows.Forms.Control.ModifierKeys != System.Windows.Forms.Keys.Shift)
                    {

                        #region Select

                        if (m_eEditMode == enuNodeGraphEditMode.Idle /*&& this.View.SelectedItems.Count == 0*/)
                        {
                            this.m_eEditMode = enuNodeGraphEditMode.Selecting;
                            this.m_SelectBoxCurrent = this.ControlToView(new Point(e.X, e.Y));
                            this.m_SelectBoxOrigin = this.ControlToView(new Point(e.X, e.Y));
                            this.UpdateHighlights();
                            this.CreateSelection();
                            if (button == MouseButton.Left && this.View.SelectedItems.Count > 0)
                            {
                                this.m_eEditMode = enuNodeGraphEditMode.MovingSelection;
                                this.m_MoveLastPosition = this.ControlToView(e.Location);
                            }
                        }

                        #endregion
                    }
                }
                #endregion
            }
            this.Invalidate();
        }
        public void NodeGraphPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.DeltaWheel != 0)
            {
                float num = this.View.ViewZoom + (float)e.DeltaWheel * 0.01f;
                if (num > 0.1f && num < 2f)
                {
                    this.View.ViewZoom = num;
                }
            }
            if (this.m_eEditMode == enuNodeGraphEditMode.SelectingBox)
            {
                this.m_SelectBoxCurrent = this.ControlToView(new Point(e.X, e.Y));
            }
            this.UpdateFontSize();
            this.Invalidate();
        }
        public void NodeGraphPanel_MouseUp(object sender, MouseEventArgs e)
        {
//this.Cursor = Cursors.Default;
            switch (this.m_eEditMode)
            {
                case enuNodeGraphEditMode.Scrolling:
                    if (e.Button == MouseButton.Right)
                    {
                        this.m_eEditMode = enuNodeGraphEditMode.Idle;
                    }
                    break;
                case enuNodeGraphEditMode.Selecting:
                case enuNodeGraphEditMode.SelectingBox:
                    if (e.Button == MouseButton.Left)
                    {
                        this.CreateSelection();
                        this.m_eEditMode = enuNodeGraphEditMode.Idle;
                    }
                    break;
                case enuNodeGraphEditMode.MovingSelection:
                    if (e.Button == MouseButton.Left)
                    {
                        this.m_eEditMode = enuNodeGraphEditMode.Idle;
                    }
                    break;
                case enuNodeGraphEditMode.MovingPoint:
                    if (e.Button == MouseButton.Left)
                        this.m_eEditMode = enuNodeGraphEditMode.Idle;
                    break;
                case enuNodeGraphEditMode.Linking:
                    this.m_OutputLink = this.GetHitPlug(e.Location);
                    //this.ValidateLink();
                    this.m_eEditMode = enuNodeGraphEditMode.Idle;
                    break;
            }
        }
        public void NodeGraphPanel_MouseMove(object sender, MouseEventArgs e)
        {
            this.m_ViewSpaceCursorLocation = this.ControlToView(new Point(e.X, e.Y));

            //if(e.Button == MouseButtons.Left)
            #region Left Mouse Button
            switch (this.m_eEditMode)
            {
                case enuNodeGraphEditMode.Scrolling:
//this.Cursor = Cursors.Hand;
                    this.View.ViewX += (int)((float)(e.Location.X - this.m_iScrollLastX) / this.View.ViewZoom);
                    this.View.ViewY += (int)((float)(e.Location.Y - this.m_iScrollLastY) / this.View.ViewZoom);
                    this.m_iScrollLastX = e.Location.X;
                    this.m_iScrollLastY = e.Location.Y;
                    this.Invalidate();
                    return;
                case enuNodeGraphEditMode.Selecting:
                    this.m_eEditMode = enuNodeGraphEditMode.SelectingBox;
                    this.m_SelectBoxCurrent = this.ControlToView(new Point(e.X, e.Y));
                    this.UpdateHighlights();
                    this.Invalidate();
                    return;
                case enuNodeGraphEditMode.SelectingBox:
//this.Cursor = Cursors.Cross;
                    if (this.IsInScrollArea(e.Location))
                    {
                        this.UpdateScroll(e.Location);
                    }
                    this.m_SelectBoxCurrent = this.ControlToView(new Point(e.X, e.Y));
                    this.UpdateHighlights();
                    this.Invalidate();
                    return;
                case enuNodeGraphEditMode.MovingSelection:
                    {
//this.Cursor = Cursors.SizeAll;
                        if (this.IsInScrollArea(e.Location))
                        {
                            this.UpdateScroll(e.Location);
                        }
                        Point moveLastPosition = this.ControlToView(e.Location);
                        int x = this.m_MoveLastPosition.X - moveLastPosition.X;
                        int y = this.m_MoveLastPosition.Y - moveLastPosition.Y;
                        this.MoveSelection(new Point(x, y));
                        this.Invalidate();
                        this.m_MoveLastPosition = moveLastPosition;
                        return;
                        break;
                    }
                case enuNodeGraphEditMode.MovingPoint:
                    {
//this.Cursor = Cursors.SizeAll;
                        if (this.IsInScrollArea(e.Location))
                        {
                            this.UpdateScroll(e.Location);
                        }
                        Point moveLastPosition = this.ControlToView(e.Location);
                        int x = this.m_MoveLastPositionPoint.X - moveLastPosition.X;
                        int y = this.m_MoveLastPositionPoint.Y - moveLastPosition.Y;
                        this.MovePoint(new Point(x, y));
                       // //this.Invalidate();
                        this.m_MoveLastPositionPoint = moveLastPosition;
                        return;
                        break;
                    }
                case enuNodeGraphEditMode.Linking:
                    if (this.IsInScrollArea(e.Location))
                    {
                        this.UpdateScroll(e.Location);
                    }
                    this.Invalidate();
                    return;
            }
            #endregion



            //this.Invalidate();
        }
        private void NodeGraphPanel_Resize(object sender, EventArgs e)
        {
            //this.Invalidate();
        }
        public void NodeGraphPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift)
            {
                this.m_bShiftPressed = true;
            }
            if (e.Alt)
            {
                this.m_bAltPressed = true;
            }
            if (e.Control)
            {
                this.m_bCtrlPressed = true;
            }
            if (e.KeyCode == Keys.Delete)
            {
                this.DeleteSelected();
            }
        }
        public void NodeGraphPanel_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Shift)
                this.m_bShiftPressed = false;
            if (!e.Alt)
                this.m_bAltPressed = false;
            if (!e.Control)
                this.m_bCtrlPressed = false;
        }
        #endregion

        #region Add / Delete
        public void AddNode(NodeGraphNode p_Node)
        {
            //if (p_Node is IHeatExchangerNode)
            //{
            //    int hxsId = this.GetNextAvailableHxsIndex();

            //    if (hxsId == -1)
            //        return;


            //    ((IHeatExchangerNode)p_Node).HxsId = hxsId;

            //}

            this.View.NodeCollection.Add(p_Node);
            //this.Invalidate();
        }

        public void DeleteSelected()
        {
            foreach (NodeGraphNode current in this.View.SelectedItems)
            {
                foreach (NodeGraphPlug current2 in current.Plugs)
                {
                    this.DeleteLinkConnectors(current2);
                }
                this.View.NodeCollection.Remove(current);
            }
            //this.Invalidate();
        }
        private void DeleteLinkConnectors(NodeGraphPlug pPlug)
        {
            List<NodeGraphLink> list = new List<NodeGraphLink>();
            foreach (NodeGraphLink current in this.View.Links)
            {
                if (current.Input == pPlug || current.Output == pPlug)
                {
                    list.Add(current);
                }
            }
            foreach (NodeGraphLink current2 in list)
            {
                this.View.Links.Remove(current2);
            }
            //this.Invalidate();
        }
        #endregion

        #region Drawing
        private void DrawSelectionBox(PaintEventArgs e)
        {
            if (this.m_eEditMode == enuNodeGraphEditMode.SelectingBox)
            {
                Rectangle p_Rectangle = default(Rectangle);
                if (this.m_SelectBoxOrigin.X > this.m_SelectBoxCurrent.X)
                {
                    p_Rectangle.X = this.m_SelectBoxCurrent.X;
                    p_Rectangle.Width = this.m_SelectBoxOrigin.X - this.m_SelectBoxCurrent.X;
                }
                else
                {
                    p_Rectangle.X = this.m_SelectBoxOrigin.X;
                    p_Rectangle.Width = this.m_SelectBoxCurrent.X - this.m_SelectBoxOrigin.X;
                }
                if (this.m_SelectBoxOrigin.Y > this.m_SelectBoxCurrent.Y)
                {
                    p_Rectangle.Y = this.m_SelectBoxCurrent.Y;
                    p_Rectangle.Height = this.m_SelectBoxOrigin.Y - this.m_SelectBoxCurrent.Y;
                }
                else
                {
                    p_Rectangle.Y = this.m_SelectBoxOrigin.Y;
                    p_Rectangle.Height = this.m_SelectBoxCurrent.Y - this.m_SelectBoxOrigin.Y;
                }

                this.m_SelectionOutline = new Pen(m_SelectionOutlineColor);
               
                e.GC.FillRectangle(this.m_SelectionFill = new SolidBrush(m_SelectionFillColor), this.ViewToControl(p_Rectangle));
                e.GC.DrawRectangle(new NanoPen(this.m_SelectionOutline.Color), this.ViewToControl(p_Rectangle));
            }
        }
        private void DrawNewLink(PaintEventArgs e)
        {
            if (this.m_eEditMode == enuNodeGraphEditMode.Linking)
            {
                Rectangle area = this.m_InputLink.GetPlugArea();
                Point pt = new Point(area.X + (int)(6f * this.View.CurrentViewZoom), area.Y + (int)(4f * this.View.CurrentViewZoom));
                Point point = this.ViewToControl(new Point(this.m_ViewSpaceCursorLocation.X, this.m_ViewSpaceCursorLocation.Y));
                Point point2 = new Point(pt.X + (int)((float)(point.X - pt.X) / this.LinkHardness), pt.Y);
                Point point3 = new Point(point.X - (int)((float)(point.X - pt.X) / this.LinkHardness), point.Y);

                switch (this.m_InputLink.LinkVisualStyle)
                {
                    case LinkVisualStyle.Direct:
                        e.GC.DrawLine(new NanoPen(this.m_LinkEditable.Color), pt, point);
                        break;
                    case LinkVisualStyle.RectangleHorizontal:
                        e.GC.DrawLine(new NanoPen(this.m_LinkEditable.Color), pt, point2);
                        e.GC.DrawLine(new NanoPen(this.m_LinkEditable.Color), point2, point3);
                        e.GC.DrawLine(new NanoPen(this.m_LinkEditable.Color), point3, point);

                        break;
                    case LinkVisualStyle.RectangleVertical:
                        Point point4 = new Point(point.X, point3.Y);

                        //if(point3.Y >= point2.Y)
                        //    point4 = 
                        //if(point3.Y < point2.Y)

                        e.GC.DrawLine(new NanoPen(this.m_LinkEditable.Color), pt, point2);
                        e.GC.DrawLine(new NanoPen(this.m_LinkEditable.Color), point2, point3);
                        e.GC.DrawLine(new NanoPen(this.m_LinkEditable.Color), point3, point4);
                        e.GC.DrawLine(new NanoPen(this.m_LinkEditable.Color), point4, point);
                        break;
                    case LinkVisualStyle.BezierCurve:
                        e.GC.DrawBezier(new NanoPen(this.m_LinkEditable.Color, (int)this.m_LinkEditable.Width), pt, point2, point3, point);
                        break;
                }
            }
        }
        private void DrawAllLinks(PaintEventArgs e)
        {
            foreach (NodeGraphLink current in this.View.Links)
            {
                Rectangle inputArea = current.Input.GetPlugArea();
                Rectangle outputArea = current.Output.GetPlugArea();

                Point pt0 = new Point();
                Point pt3 = new Point();
                Point pt1 = new Point();
                Point pt2 = new Point();
                //Point pt5 = new Point();

                Pen pen;
                Brush brush;

                if (this.m_bUseLinkColoring)
                {
                    pen = current.NodeGraphDataType.LinkPen;
                    brush = current.NodeGraphDataType.LinkArrowBrush;
                }
                else
                {
                    pen = this.m_Link;
                    brush = this.m_LinkArrow;
                }

                pen.Width = this.m_linkPenWidth;

                switch (current.Input.LinkVisualStyle)
                {
                    #region Direct
                    case LinkVisualStyle.Direct:

                        pt0 = new Point(inputArea.X + (int)(6f * this.View.CurrentViewZoom), inputArea.Y + (int)(4f * this.View.CurrentViewZoom));
                        pt3 = new Point(outputArea.X + (int)(-4f * this.View.CurrentViewZoom), outputArea.Y + (int)(4f * this.View.CurrentViewZoom));
                        //pt2 = new Point(pt0.X + (int)((float)(pt1.X - pt0.X) / this.LinkHardness), pt0.Y);

                        pt3 = new Point(outputArea.X + (int)(-4f * this.View.CurrentViewZoom),
                            outputArea.Y + (int)(4f * this.View.CurrentViewZoom));
                        e.GC.DrawLine(new NanoPen(pen.Color), pt0, pt3);

                        //if (current.Output.ParentNode.Highlighted || current.Input.ParentNode.Highlighted)
                        //{
                        //    e.Graphics.DrawEllipse(new Pen(Color.Orange), pt0.X, pt0.Y, 3f, 3f);
                        //    e.Graphics.DrawEllipse(new Pen(Color.Orange), pt3.X, pt3.Y, 3f, 3f);
                        //}

                        break;
                    #endregion

                    case LinkVisualStyle.RectangleHorizontal:

                        // Horizontal Points
                        pt3 = new Point(outputArea.X + (int)(-4f * this.View.CurrentViewZoom), outputArea.Y + (int)(4f * this.View.CurrentViewZoom));
                        pt0 = new Point(inputArea.X + (int)(6f * this.View.CurrentViewZoom), inputArea.Y + (int)(4f * this.View.CurrentViewZoom));


                        pt1 = new Point(pt0.X + (int)((float)(pt3.X - pt0.X) / this.LinkHardness), pt0.Y);
                        pt2 = new Point(pt3.X - (int)((float)(pt3.X - pt0.X) / this.LinkHardness), pt3.Y);

                        pt1.Offset(current.p1Offset);
                        pt2.Offset(current.p2Offset);

                        current.p1Collision = pt1;
                        current.p2Collision = pt2;

                        e.GC.DrawLine(new NanoPen(pen.Color), pt0, pt1);
                        e.GC.DrawLine(new NanoPen(pen.Color), pt1, pt2);
                        e.GC.DrawLine(new NanoPen(pen.Color), pt2, pt3);

                        if (current.Output.ParentNode.Highlighted || current.Input.ParentNode.Highlighted)
                        {
                            //e.Graphics.DrawEllipse(new Pen(Color.Orange), pt0.X, pt0.Y, 3f, 3f);
                            e.GC.DrawEllipse(new Pen(Color.Orange), pt1.X, pt1.Y, 5f, 5f);
                            e.GC.DrawEllipse(new Pen(Color.Orange), pt2.X, pt2.Y, 5f, 5f);
                            //e.Graphics.DrawEllipse(new Pen(Color.Orange), pt3.X, pt3.Y, 3f, 3f);
                        }
                        //e.Graphics.DrawLine(new Pen(Color.Blue), pt2, pt3);         // Horizontal                       
                        //e.Graphics.DrawLine(new Pen(Color.Green), pt1, pt2);        // Vertical
                        //e.Graphics.DrawLine(new Pen(Color.Red), pt0, pt1);          // Horizontal



                        break;
                    case LinkVisualStyle.RectangleVertical:
                        pt3 = new Point(outputArea.X + (int)(-4f * this.View.CurrentViewZoom), outputArea.Y + (int)(4f * this.View.CurrentViewZoom));
                        pt0 = new Point(inputArea.X + (int)(6f * this.View.CurrentViewZoom), inputArea.Y + (int)(4f * this.View.CurrentViewZoom));
                        pt1 = new Point(pt0.X + (int)((float)(pt3.X - pt0.X) / this.LinkHardness), pt0.Y);
                        pt2 = new Point(pt3.X - (int)((float)(pt3.X - pt0.X) / this.LinkHardness), pt3.Y);

                        e.GC.DrawLine(new NanoPen(pen.Color), pt0, pt1);
                        e.GC.DrawLine(new NanoPen(pen.Color), pt1, pt2);
                        e.GC.DrawLine(new NanoPen(pen.Color), pt2, pt3);

                        if (current.Output.ParentNode.Highlighted || current.Input.ParentNode.Highlighted)
                        {
                            e.GC.DrawEllipse(new Pen(Color.Orange), pt0.X, pt0.Y, 5f, 5f);
                            e.GC.DrawEllipse(new Pen(Color.Orange), pt1.X, pt1.Y, 5f, 5f);
                            e.GC.DrawEllipse(new Pen(Color.Orange), pt2.X, pt2.Y, 5f, 5f);
                            e.GC.DrawEllipse(new Pen(Color.Orange), pt3.X, pt3.Y, 5f, 5f);
                        }
                        //e.Graphics.DrawLine(this.m_LinkEditable, pt3, pt4);
                        break;
                    case LinkVisualStyle.BezierCurve:
                        pt3 = new Point(outputArea.X + (int)(-4f * this.View.CurrentViewZoom), outputArea.Y + (int)(4f * this.View.CurrentViewZoom));
                        pt0 = new Point(inputArea.X + (int)(6f * this.View.CurrentViewZoom), inputArea.Y + (int)(4f * this.View.CurrentViewZoom));
                        pt1 = new Point(pt0.X + (int)((float)(pt3.X - pt0.X) / this.LinkHardness), pt0.Y);
                        pt2 = new Point(pt3.X - (int)((float)(pt3.X - pt0.X) / this.LinkHardness), pt3.Y);

                        pt1.Offset(current.p1Offset);
                        pt2.Offset(current.p2Offset);

                        current.p1Collision = pt1;
                        current.p2Collision = pt2;

                        e.GC.DrawBezier(new NanoPen(pen.Color), pt0, pt1, pt2, pt3);

                        if (current.Output.ParentNode.Highlighted || current.Input.ParentNode.Highlighted)
                        {
                            //e.Graphics.DrawEllipse(new Pen(Color.Orange), pt0.X, pt0.Y, 3f, 3f);
                            e.GC.DrawEllipse(new Pen(Color.Orange), pt1.X, pt1.Y, 5f, 5f);
                            e.GC.DrawEllipse(new Pen(Color.Orange), pt2.X, pt2.Y, 5f, 5f);
                            //e.Graphics.DrawEllipse(new Pen(Color.Orange), pt3.X, pt3.Y, 3f, 3f);
                        }
                        break;
                }

                #region Arrow
               Point[] arrowPoints = new Point[]
               {
                    new Point(pt3.X + (int)(10f * this.View.CurrentViewZoom), pt3.Y),
                    new Point(pt3.X, pt3.Y - (int)(4f * this.View.CurrentViewZoom)),
                    new Point(pt3.X, pt3.Y + (int)(4f * this.View.CurrentViewZoom))
               };

                throw new NotImplementedException("Switch to NanoSolidBrush");

                //e.GC.FillPolygon(new NanoSolidBrush(brush), arrowPoints);
                #endregion
            }
        }
        private void NodeGraphPanel_Paint(/*object sender,*/ PaintEventArgs e)
        {
            #region Draw  Background (grid?)
            if (this.onDrawBackground != null)
            {
                this.onDrawBackground(this, e);
            }
            #endregion

            #region Update Font Sizes
            if (this.m_bSmoothBehavior)
            {
                this.View.CurrentViewZoom += (this.View.ViewZoom - this.View.CurrentViewZoom) * 0.08f;
                if ((double)Math.Abs(this.View.CurrentViewZoom - this.View.ViewZoom) < 0.005)
                {
                    this.View.CurrentViewZoom = this.View.ViewZoom;
                    this.UpdateFontSize();
                }
                else
                {
                    //this.UpdateFontSize();
                    //base.Invalidate();
                }
            }
            else
            {
                this.View.CurrentViewZoom = this.View.ViewZoom;
                //this.UpdateFontSize();
            }
            #endregion

            #region Setup Graphics Renderer
            ////e.GC.PageUnit = GraphicsUnit.Pixel;
            ////e.GC.CompositingQuality = CompositingQuality.GammaCorrected;
            ////e.GC.InterpolationMode = InterpolationMode.HighQualityBicubic;
            ////e.GC.SmoothingMode = SmoothingMode.HighQuality;
            ////e.GC.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            ////e.GC.PixelOffsetMode = PixelOffsetMode.HighQuality;
            #endregion

            #region Draw Grid
            if (this.m_bShowGrid)
            {
                int num = (int)((this.BGColor.R + this.BGColor.G + this.BGColor.B) / 3);
                Color color;
                if (num < 128)
                {
                    color = Color.FromArgb((int)this.m_iGridAlpha, 255, 255, 255);
                }
                else
                {
                    color = Color.FromArgb((int)this.m_iGridAlpha, 0, 0, 0);
                }
                Pen pen = new Pen(color);
                Point point = this.ControlToView(new Point(0, 0));
                Point point2 = this.ControlToView(new Point(base.Width, base.Height));
                int num2 = point.X - point.X % this.m_iGridPadding;
                int num3 = point.Y - point.Y % this.m_iGridPadding;
                int num4 = point2.X + point2.X % this.m_iGridPadding;
                int num5 = point2.Y + point2.Y % this.m_iGridPadding;
                for (int i = num2; i < num4; i += this.m_iGridPadding)
                {
                    Point pt = this.ViewToControl(new Point(i, point.Y));
                    Point pt2 = this.ViewToControl(new Point(i, point2.Y));
                    e.GC.DrawLine(new NanoPen(pen.Color), pt, pt2);
                }
                for (int j = num3; j < num5; j += this.m_iGridPadding)
                {
                    Point pt = this.ViewToControl(new Point(point.X, j));
                    Point pt2 = this.ViewToControl(new Point(point2.X, j));
                    e.GC.DrawLine(new NanoPen(pen.Color), pt, pt2);
                }
            }
            #endregion

            #region Draw New Link
            this.DrawNewLink(e);
            #endregion

            #region Draw All Links
            this.DrawAllLinks(e);
            #endregion

            #region Draw Nodes
            foreach (NodeGraphNode current in this.View.NodeCollection)
            {
                try
                {
                    current.Draw(e, false);
                }
                catch (Exception ce)
                {
                    NthDimension.Rendering.Utilities.ConsoleUtil.errorlog(ce.Message, ce.StackTrace);
                    //throw;
                }


            }
            #endregion

            #region Draw Selection Box
            this.DrawSelectionBox(e);
            #endregion

            //e.GC.SmoothingMode = SmoothingMode.None;

            #region Draw Debug
            if (this.EnableDrawDebug)
            {
                this.DrawDebug(e);
            }
            #endregion
        }
        private void DrawDebug(PaintEventArgs e)
        {
            NanoSolidBrush br = new NanoSolidBrush(Color.White);

            e.GC.DrawString("Edit Mode:" + this.m_eEditMode.ToString(), br, 0f, 10f);
            e.GC.DrawString("ViewX: " + this.View.ViewX.ToString(), br, 0f, 20f);
            e.GC.DrawString("ViewY: " + this.View.ViewY.ToString(), br, 0f, 30f);
            e.GC.DrawString("ViewZoom: " + this.View.ViewZoom.ToString(), br,0f, 40f);
            e.GC.DrawString("ViewSpace Cursor Location:" + this.m_ViewSpaceCursorLocation.X.ToString() + " : " + this.m_ViewSpaceCursorLocation.Y.ToString(), br, 0f, 60f);
            e.GC.DrawString("AltPressed: " + this.m_bAltPressed.ToString(), br, 0f, 80f);
            NanoPen pen = new NanoPen(Color.Lime);
            Pen p = new Pen(Color.Lime);
            e.GC.DrawLine(pen, this.ViewToControl(new Point(-100, 0)), this.ViewToControl(new Point(100, 0)));
            e.GC.DrawLine(pen, this.ViewToControl(new Point(0, -100)), this.ViewToControl(new Point(0, 100)));
            //e.GC.DrawBezier(new NanoPen(p.Color), this.ViewToControl(this.m_SelectBoxOrigin), this.ViewToControl(this.m_SelectBoxOrigin), this.ViewToControl(this.m_SelectBoxCurrent), this.ViewToControl(this.m_SelectBoxCurrent));

            ////e.GC.DrawString("Edit Mode:" + this.m_eEditMode.ToString(), this.m_oDebugFont, this.m_NodeText, new PointF(0f, 0f));
            ////e.GC.DrawString("ViewX: " + this.View.ViewX.ToString(), this.m_oDebugFont, this.m_NodeText, new PointF(0f, 10f));
            ////e.GC.DrawString("ViewY: " + this.View.ViewY.ToString(), this.m_oDebugFont, this.m_NodeText, new PointF(0f, 20f));
            ////e.GC.DrawString("ViewZoom: " + this.View.ViewZoom.ToString(), this.m_oDebugFont, this.m_NodeText, new PointF(0f, 30f));
            ////e.GC.DrawString("ViewSpace Cursor Location:" + this.m_ViewSpaceCursorLocation.X.ToString() + " : " + this.m_ViewSpaceCursorLocation.Y.ToString(), this.m_oDebugFont, this.m_NodeText, new PointF(0f, 50f));
            ////e.GC.DrawString("AltPressed: " + this.m_bAltPressed.ToString(), this.m_oDebugFont, this.m_NodeText, new PointF(0f, 70f));
            ////Pen pen = new Pen(Color.Lime);
            ////e.GC.DrawLine(pen, this.ViewToControl(new Point(-100, 0)), this.ViewToControl(new Point(100, 0)));
            ////e.GC.DrawLine(pen, this.ViewToControl(new Point(0, -100)), this.ViewToControl(new Point(0, 100)));
            ////e.GC.DrawBezier(pen, this.ViewToControl(this.m_SelectBoxOrigin), this.ViewToControl(this.m_SelectBoxOrigin), this.ViewToControl(this.m_SelectBoxCurrent), this.ViewToControl(this.m_SelectBoxCurrent));
        }

        protected override void DoPaint(PaintEventArgs e)
        {
            if (!m_init)
                this.InitializeComponent();

            //base.DoPaint(e);

            this.NodeGraphPanel_Paint(e);
        }

        //protected override void DoPaint(GContext parentGContext)
        //{
        //    if (!m_init)
        //        this.InitializeComponent();

        //    this.DoPaint(new PaintEventArgs(parentGContext));
        //}


        #endregion

        #region ControlToView & ViewToControl co-ordinates
        public Point ControlToView(Point p_Point)
        {
            return new Point((int)((float)(p_Point.X - base.Width / 2) / this.View.CurrentViewZoom) - this.View.ViewX, (int)((float)(p_Point.Y - base.Height / 2) / this.View.CurrentViewZoom) - this.View.ViewY);
        }
        public Rectangle ControlToView(Rectangle p_Rectangle)
        {
            return new Rectangle(this.ControlToView(new Point(p_Rectangle.X, p_Rectangle.Y)), new Size((int)((float)p_Rectangle.Width / this.View.CurrentViewZoom), (int)((float)p_Rectangle.Height / this.View.CurrentViewZoom)));
        }
        public Point ViewToControl(Point p_Point)
        {
            return new Point((int)((float)(p_Point.X + this.View.ViewX) * this.View.CurrentViewZoom) + base.Width / 2, (int)((float)(p_Point.Y + this.View.ViewY) * this.View.CurrentViewZoom) + base.Height / 2);
        }
        public Rectangle ViewToControl(Rectangle p_Rectangle)
        {
            return new Rectangle(this.ViewToControl(new Point(p_Rectangle.X, p_Rectangle.Y)), new Size((int)((float)p_Rectangle.Width * this.View.CurrentViewZoom), (int)((float)p_Rectangle.Height * this.View.CurrentViewZoom)));
        }
        #endregion

        #region Scrolling
        public bool IsInScrollArea(Point p_CursorLocation)
        {
            return p_CursorLocation.X <= this.m_ScrollMargins || p_CursorLocation.X >= base.Width - this.m_ScrollMargins || p_CursorLocation.Y <= this.m_ScrollMargins || p_CursorLocation.Y >= base.Height - this.m_ScrollMargins;
        }
        private void UpdateScroll(Point p_CursorLocation)
        {
            if (p_CursorLocation.X < this.m_ScrollMargins)
            {
                this.View.ViewX += (int)((float)this.m_ScrollMarginsValue / this.View.CurrentViewZoom);
            }
            else if (p_CursorLocation.X > base.Width - this.m_ScrollMargins)
            {
                this.View.ViewX -= (int)((float)this.m_ScrollMarginsValue / this.View.CurrentViewZoom);
            }
            else if (p_CursorLocation.Y < this.m_ScrollMargins)
            {
                this.View.ViewY += (int)((float)this.m_ScrollMarginsValue / this.View.CurrentViewZoom);
            }
            else if (p_CursorLocation.Y > base.Height - this.m_ScrollMargins)
            {
                this.View.ViewY -= (int)((float)this.m_ScrollMarginsValue / this.View.CurrentViewZoom);
            }
        }
        #endregion

        #region Zooming
        public void Zoom(int delta)
        {
            if (delta != 0)
            {
                float num = this.View.ViewZoom + (float)delta * 0.001f;
                if (num > 0.1f && num < 2f)
                {
                    this.View.ViewZoom = num;
                }
            }
            //if (this.m_eEditMode == enuNodeGraphEditMode.SelectingBox)
            //{
            //    this.m_SelectBoxCurrent = this.ControlToView(new Point(mouseX, mouseY));
            //}
            this.UpdateFontSize();
            //this.Invalidate();
        }
        #endregion

        #region Selection
        private void UpdateHighlights()
        {
            Rectangle rect = default(Rectangle);
            if (this.m_SelectBoxOrigin.X > this.m_SelectBoxCurrent.X)
            {
                rect.X = this.m_SelectBoxCurrent.X;
                rect.Width = this.m_SelectBoxOrigin.X - this.m_SelectBoxCurrent.X;
            }
            else
            {
                rect.X = this.m_SelectBoxOrigin.X;
                rect.Width = this.m_SelectBoxCurrent.X - this.m_SelectBoxOrigin.X;
            }
            if (this.m_SelectBoxOrigin.Y > this.m_SelectBoxCurrent.Y)
            {
                rect.Y = this.m_SelectBoxCurrent.Y;
                rect.Height = this.m_SelectBoxOrigin.Y - this.m_SelectBoxCurrent.Y;
            }
            else
            {
                rect.Y = this.m_SelectBoxOrigin.Y;
                rect.Height = this.m_SelectBoxCurrent.Y - this.m_SelectBoxOrigin.Y;
            }
            foreach (NodeGraphNode current in this.View.NodeCollection)
            {
                if (current.HitRectangle.IntersectsWith(rect) && current.CanBeSelected)
                {
                    current.Highlighted = true;
                }
                else
                {
                    current.Highlighted = false;
                }
            }
        }
        private void CreateSelection()
        {
            this.View.SelectedItems.Clear();
            int num = 0;
            foreach (NodeGraphNode current in this.View.NodeCollection)
            {
                if (current.Highlighted)
                {
                    num++;
                    this.View.SelectedItems.Add(current);
                }
            }
            if (num > 0 && this.onSelectionChanged != null)
            {
                this.onSelectionChanged(this, new NodeGraphPanelSelectionEventArgs(num));
            }
            if (num == 0 && this.onSelectionCleared != null)
            {
                this.onSelectionCleared(this, new NodeGraphPanelSelectionEventArgs(num));
            }
        }
        private void MoveSelection(Point p_Offset)
        {
            foreach (NodeGraphNode current in this.View.SelectedItems)
            {
                current.X -= p_Offset.X;
                current.Y -= p_Offset.Y;
                current.UpdateHitRectangle();
            }
        }
        private void MovePoint(Point p_Offset)
        {
            foreach (NodeGraphLink current in this.View.Links)
            {
                if (current == this.View.SelectedLine.Line)
                {
                    if (this.View.SelectedLine.PointA)
                    {
                        current.p1Offset.X -= p_Offset.X;
                        current.p1Offset.Y -= p_Offset.Y;
                    }

                    if (this.View.SelectedLine.PointB)
                    {
                        current.p2Offset.X -= p_Offset.X;
                        current.p2Offset.Y -= p_Offset.Y;
                    }
                }
            }
        }
        #endregion

        #region Hit
        private HitType HitSelected(Point p_CursorLocation)
        {
            Rectangle rectangle = new Rectangle(this.ControlToView(p_CursorLocation), default(Size));
            HitType result;
            foreach (NodeGraphNode current in this.View.SelectedItems)
            {
                if (rectangle.IntersectsWith(current.HitRectangle))
                {
                    NodeGraphPlug connectorMouseHit = current.GetConnectorMouseHit(p_CursorLocation);
                    if (connectorMouseHit == null)
                    {
                        result = HitType.Node;
                        return result;
                    }
                    result = HitType.Plug;
                    return result;
                }
            }
            result = HitType.Nothing;
            return result;
        }
        private HitType HitAll(Point p_CursorLocation)
        {
            Rectangle rectangle = new Rectangle(this.ControlToView(p_CursorLocation), default(Size));
            HitType result;


            foreach (NodeGraphNode current in this.View.NodeCollection)
            {
                if (rectangle.IntersectsWith(current.HitRectangle))
                {
                    NodeGraphPlug connectorMouseHit = current.GetConnectorMouseHit(p_CursorLocation);
                    if (connectorMouseHit == null)
                    {
                        result = HitType.Node;
                        return result;
                    }
                    result = HitType.Plug;
                    return result;
                }
            }

            Point curPoint = new Point(p_CursorLocation.X - 15, p_CursorLocation.Y - 15);
            Rectangle rectnaglePoint = new Rectangle(curPoint, new Size(15, 15));

            foreach (NodeGraphLink link in this.View.Links)
            {
                if (rectnaglePoint.Contains(link.p1Collision))
                {
                    result = HitType.Point;
                    this.View.SelectedLine = new SelectedLine(link, true, false);
                    return result;
                }

                if (rectnaglePoint.Contains(link.p2Collision))
                {
                    result = HitType.Point;
                    this.View.SelectedLine = new SelectedLine(link, false, true);
                    return result;
                }
            }



            result = HitType.Nothing;
            return result;
        }
        private NodeGraphPlug GetHitPlug(Point p_CursorLocation)
        {
            NodeGraphPlug nodeGraphPlug = null;
            Rectangle rectangle = new Rectangle(this.ControlToView(p_CursorLocation), default(Size));
            NodeGraphPlug result;
            foreach (NodeGraphNode current in this.View.NodeCollection)
            {
                if (rectangle.IntersectsWith(current.HitRectangle))
                {
                    result = current.GetConnectorMouseHit(p_CursorLocation);
                    return result;
                }
            }
            result = nodeGraphPlug;
            return result;
        }
        #endregion

        #region Links
        private void ValidateLink()
        {
            if (this.m_InputLink != null && this.m_OutputLink != null && this.m_InputLink != this.m_OutputLink && this.m_InputLink.Type != this.m_OutputLink.Type)
            {
                //if (this.m_InputLink.DataType == this.m_OutputLink.DataType)
                if (this.m_InputLink.DataType.TypeName == this.m_OutputLink.DataType.TypeName)
                {
                    if (this.IsLinked(this.m_OutputLink))
                    {
                        this.DeleteLinkConnectors(this.m_OutputLink);
                    }
                    this.View.Links.Add(new NodeGraphLink(this.m_InputLink, this.m_OutputLink, this.m_InputLink.DataType));

                    this.m_OutputLink.RaisePlugLinked(this.m_InputLink);

                    //if (this.m_InputLink.Type == PlugType.GasInlet)
                    //{
                    //    if (this.IsLinked(this.m_OutputLink))
                    //    {
                    //        this.DeleteLinkConnectors(this.m_OutputLink);
                    //    }
                    //    this.View.Links.Add(new NodeGraphLink(this.m_InputLink, this.m_OutputLink, this.m_InputLink.DataType));
                    //}
                    //else
                    //{
                    //    if (this.IsLinked(this.m_InputLink))
                    //    {
                    //        this.DeleteLinkConnectors(this.m_InputLink);
                    //    }
                    //    this.View.Links.Add(new NodeGraphLink(this.m_OutputLink, this.m_InputLink, this.m_InputLink.DataType));
                    //}
                }
            }
            this.m_InputLink = null;
            this.m_OutputLink = null;
        }
        public NodeGraphPlug GetLink(NodeGraphPlug pLinkOutPlug)
        {
            NodeGraphPlug result;
            foreach (NodeGraphLink current in this.View.Links)
            {
                if (current.Output == pLinkOutPlug)
                {
                    result = current.Input;
                    return result;
                }
            }
            result = null;
            return result;
        }
        public bool IsLinked(NodeGraphPlug p_Node)
        {
            bool result;
            foreach (NodeGraphLink current in this.View.Links)
            {
                if (current.Input == p_Node || current.Output == p_Node)
                {
                    result = true;
                    return result;
                }
            }
            result = false;
            return result;
        }
        #endregion

        #region Font Size
        private void UpdateFontSize()
        {
            this.m_NodeScaledTitleFont = new NanoFont(this.m_NodeTitleFont, this.m_NodeTitleFont.FontSize * this.View.CurrentViewZoom);
            this.m_NodeScaledConnectorFont = new NanoFont(this.m_NodeConnectorFont, this.m_NodeConnectorFont.FontSize * this.View.CurrentViewZoom);
        }
        #endregion

        #region Save / Load Xml
        public void SaveCurrentView(string p_FileName)
        {
            //this.RestructureHxsGasPath();

            XmlTree xmlTree = new XmlTree("NodeGraphControl");
            xmlTree.m_rootNode.AddChild(this.View.SerializeToXML(xmlTree.m_rootNode));
            xmlTree.SaveXML(p_FileName);
        }
        public void LoadCurrentView(string p_FileName)
        {
            XmlTree xmlTree = new XmlTree("NodeGraphControl");
            xmlTree.LoadXML(p_FileName);
            this.View = new NodeViewState(xmlTree.m_rootNode.GetFirstChild(SerializationUtils.GetFullTypeName(this.View)), this);

            this.InitializeNodegraphPanel(false);

            this.RestructureHxsGasPath();
            this.RestructureHxsFluidPath();
            this.RestructureHxsAirPath();

            this.UpdateFontSize();
            //this.Invalidate();
        }
        #endregion






        #region Heat Exchanger Logic
        public void RestructureHxsGasPath(string removeId = "0")
        {
            //foreach (NodeGraphLink link in this.View.Links)
            //{
            //    if (link.Input.ParentNode is IHeatExchangerNode &&
            //        link.Input.PlugType == ConnectorType._GasOutlet)
            //    {
            //        // TODO:: Check here if HxsIdFromGasPath has alredy a value


            //        if (link.Output.ParentNode is IHeatExchangerGasComponent)
            //        {
            //            // Checks if HxsIdFromGasPath has already a value ( .Length != 0) and append a ';' character if it does before the new HxsId
            //            if (((IHeatExchangerGasComponent)link.Output.ParentNode).HxsIdFromGasPath.Length == 0)
            //                // HxsIdFromGasPath is Empty
            //                ((IHeatExchangerGasComponent)link.Output.ParentNode).HxsIdFromGasPath +=
            //                    ((IHeatExchangerNode)link.Input.ParentNode).HxsId;
            //            else
            //            {
            //                string candidate = ((IHeatExchangerNode)link.Input.ParentNode).HxsId.ToString();
            //                string tPaths = ((IHeatExchangerGasComponent)link.Output.ParentNode).HxsIdFromGasPath;
            //                bool bExists = false;

            //                if (tPaths.Contains(";"))
            //                {
            //                    string[] sPaths = tPaths.Split(';');

            //                    for (int p = 0; p < sPaths.Length; p++)
            //                    {
            //                        if (sPaths[p] == candidate)
            //                            bExists = true;

            //                    }
            //                }

            //                if (!bExists)
            //                {
            //                    if (((IHeatExchangerGasComponent)link.Output.ParentNode).HxsIdFromGasPath == removeId)
            //                    {
            //                        ((IHeatExchangerGasComponent)link.Output.ParentNode).HxsIdFromGasPath = candidate;
            //                    }
            //                    else
            //                    {
            //                        ((IHeatExchangerGasComponent)link.Output.ParentNode).HxsIdFromGasPath += ";" + candidate;
            //                    }


            //                    //if (((IHeatExchangerGasComponent)link.Output.ParentNode).HxsIdFromGasPath.Contains(";;"))
            //                    //    ((IHeatExchangerGasComponent)link.Output.ParentNode).HxsIdFromGasPath.Replace(";;", ";");

            //                    //if (((IHeatExchangerGasComponent) link.Output.ParentNode).HxsIdFromGasPath.StartsWith(";"))
            //                    //    ((IHeatExchangerGasComponent) link.Output.ParentNode).HxsIdFromGasPath.Remove(0,1);

            //                }
            //            }


            //        }


            //    }
            //}

            //// Remove Duplicates
            //foreach (NodeGraphNode node in this.View.NodeCollection)
            //{
            //    if (node is IHeatExchangerGasComponent)
            //    {
            //        string tPaths = ((IHeatExchangerGasComponent)node).HxsIdFromGasPath;
            //        if (tPaths.Contains(";"))
            //        {
            //            string[] sPaths = tPaths.Split(';');
            //            string[] unique = sPaths.Distinct().ToArray();

            //            ((IHeatExchangerGasComponent)node).HxsIdFromGasPath = String.Join(";", unique);
            //        }
            //    }

            //}

        }

        //public List<IHeatExchangerFluidComponent> sortedFluidComponents = new List<IHeatExchangerFluidComponent>();
        public void RestructureHxsFluidPath()
        {
            //foreach (NodeGraphLink link in this.View.Links)
            //{
            //    if (link.Input.ParentNode is IHeatExchangerNode)
            //    {
            //        if (link.Input.PlugType == ConnectorType.FluidOutletBottom ||
            //            link.Input.PlugType == ConnectorType.FluidOutlet)
            //            if (link.Output.ParentNode is IHeatExchangerFluidComponent)
            //                ((IHeatExchangerFluidComponent)link.Output.ParentNode).HxsIdFromFluidPath =
            //                    ((IHeatExchangerNode)link.Input.ParentNode).HxsId;
            //    }
            //}

            //int fluidComponentsCount = 0;

            //List<IHeatExchangerFluidComponent> temp = new List<IHeatExchangerFluidComponent>();

            //for (int i = 0; i < this.View.NodeCollection.Count; i++)
            //    if (this.View.NodeCollection[i] is IHeatExchangerFluidComponent)
            //        temp.Add(this.View.NodeCollection[i] as IHeatExchangerFluidComponent);



            //sortedFluidComponents.Clear();
            //int start = int.Parse(Properties.Settings.Default.iStartID1);

            //// Add 1st


            //sortedFluidComponents.Add(temp.First(a => ((IHeatExchangerNode)a).HxsId == start));

            //int nextIdx = start;
            //int loop = 0;
            //while (loop <= this.View.NodeCollection.Count)
            //{
            //    try
            //    {
            //        IHeatExchangerFluidComponent next = temp.First(a => a.HxsIdFromFluidPath == nextIdx);
            //        sortedFluidComponents.Add(next);
            //        nextIdx = ((IHeatExchangerNode)next).HxsId;

            //    }
            //    catch (Exception e)
            //    {
            //        //EsiApplication.Instance.LOG_ESI_Msg("NextId " + nextIdx.ToString() + " " + e.Message);
            //        //throw;
            //        System.Diagnostics.Debug.Print("NextId " + nextIdx.ToString() + " " + e.Message);
            //    }
            //    finally
            //    {
            //        loop++;
            //    }


            //}
        }
        public void RestructureHxsAirPath()
        {
            //foreach (NodeGraphLink link in this.View.Links)
            //{

            //    if (link.Input.ParentNode is IHeatExchangerNode)
            //    {
            //        if (link.Input.PlugType == ConnectorType.AirOutlet)
            //            if (link.Output.ParentNode is IHeatExchangerAirComponent)
            //                ((IHeatExchangerAirComponent)link.Output.ParentNode).HxsIdFromAirPath =
            //                    ((IHeatExchangerNode)link.Input.ParentNode).HxsId;
            //    }
            //}
        }
        public int GetNextAvailableHxsIndex()
        {
            // Use LINQ to retrieve the first available index!
            int res = -1;

            //List<int> all = new List<int>(EsiApplication.Instance.HxsIndices);

            //List<int> taken = new List<int>();

            //foreach (NodeGraphNode n in this.View.NodeCollection)
            //{
            //    if (n is IHeatExchangerNode)
            //        taken.Add(((IHeatExchangerNode)n).HxsId);
            //}

            //taken.Add(0);

            //List<int> remains = all.Except(taken).ToList();

            //if (remains.Count > 0)
            //    res = remains[0];

            return res;// = candidateId;
        }
        public int[] GetAllAvailableHxsIndices()
        {
            // Use LINQ to retrieve the first available index!
            int res = -1;

            //List<int> all = new List<int>(EsiApplication.Instance.HxsIndices);

            //List<int> taken = new List<int>();
            //taken.Add(0);

            //foreach (NodeGraphNode n in this.View.NodeCollection)
            //{
            //    if (n is IHeatExchangerNode)
            //        taken.Add(((IHeatExchangerNode)n).HxsId);
            //}

            //List<int> remains = all.Except(taken).ToList();



            //return remains.ToArray();

            return new int[0];
        }
        #endregion
    }
}
