namespace NthStudio.Gui.GraphPlot
{
    using NthDimension.Forms;
    using System.Collections.Generic;
    using System.Drawing;

    public class PlotterGraphPaneEx : Panel
    {
        public enum LayoutMode
        {
            NORMAL,
            STACKED,
            VERTICAL_ARRANGED,
            TILES_VER,
            TILES_HOR,
        }

        private BackBuffer memGraphics;
        private int ActiveSources = 0;

        public Bitmap GRAPHBITMAP;

        public LayoutMode layout = LayoutMode.NORMAL;

        public Color MajorGridColor = Color.DarkGray;
        public Color MinorGridColor = Color.DarkGray;
        public Color GraphColor = Color.DarkGreen;
        public Color BgndColorTop = Color.White;
        public Color BgndColorBot = Color.White;
        public Color LabelColor = Color.White;
        public Color GraphBoxColor = Color.White;
        public bool useDoubleBuffer = false;
        public Font legendFont = new Font(FontFamily.GenericSansSerif, 8.25f);

        private List<DataSource> sources = new List<DataSource>();

        //public SmoothingMode smoothing = SmoothingMode.None;

        public bool hasMovingGrid = true;
        public bool hasBoundingBox = true;

        private Point mousePos = new Point();
        private bool mouseDown = false;

        public float starting_idx = 0;
        public float XD0 = -50;
        public float XD1 = 100;
        public float DX = 0;
        public float off_X = 0;
        public float CurXD0 = 0;
        public float CurXD1 = 0;

        public float grid_distance_x = 200;       // grid distance in samples ( draw a vertical line every 200 samples )
        public float grid_off_x = 0;
        public float GraphCaptionLineHeight = 28;

        public float pad_inter = 4;         // padding between graphs
        public float pad_left = 10;         // left padding
        public float pad_right = 10;        // right padding
        public float pad_top = 10;          // top
        public float pad_bot = 10;          // bottom padding
        public float pad_label = 40;        // y-label area width
        public float pad_xlabel = 8;        // x-label padding ( bottom area left and right were x labels are still visible )

        public float[] MinorGridPattern = new float[] { 2, 4 };
        public float[] MajorGridPattern = new float[] { 2, 2 };

        //DashStyle MinorGridDashStyle = DashStyle.Custom;
        //DashStyle MajorGridDashStyle = DashStyle.Custom;
        public bool MoveMinorGrid = true;

        public List<DataSource> Sources
        {
            get
            {
                return sources;
            }
        }

    }
}
