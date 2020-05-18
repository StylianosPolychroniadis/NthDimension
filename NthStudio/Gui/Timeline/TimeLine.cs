/*
 * Creado por NIN
 * User: sebastian.lucas
 * Date: 30/04/2007
 * Time: 15:24
 */

using System;
using System.Drawing;
using System.Collections.Generic;
using NthDimension.Forms.Events;

#if DEBUG

#endif


namespace NthStudio.Gui
{
	public partial class TimeLine : NthDimension.Forms.Panel
	{
		// TODO:: Expose Colors to Properties
		public static readonly Color color_clear = Color.Gray;//Color.FromKnownColor(KnownColor.Control);


		public static readonly Color color_keyframe_empty_a = Color.Gray;//Color.FromArgb(255, 128, 128, 128)//Color.FromArgb(255,255,255,255);
		public static readonly Color color_keyframe_empty_b = Color.DarkGray;//Color.FromArgb(255, 160, 128, 100);//Color.FromArgb(255,193,204,216);

		public static readonly Color color_keyframe_sep		= Color.FromArgb(64, 180, 180, 180);//Color.FromArgb(255,131,153,177);
	    public static readonly Color color_keyframe_fill    = Color.FromArgb(255,124,124,124);

		public static readonly Color color_timeline_bkg = Color.FromArgb(255, 128, 128, 128);// Color.FromArgb(255,193,204,216);
		public static readonly Color color_timeline_fkg = Color.FromArgb(255, 160, 128, 100);//Color.FromArgb(255,79,101,125);


		public static Color             m_color_guide = Color.Red;
	    public static Color             m_colorRulerText = Color.Black;
	    
	    public static readonly int MAX_LAYERS       = 99;
	    public static readonly int MAX_KEYFRAMES    = 1000;
	    public static readonly int LAYER_HEIGHT     = 18;
	    
	    public static readonly int MIDDLE_PERCENT   = 30;

	    public static readonly int KEYFRAME_WIDTH   = 7;
	    public static readonly int KEYFRAME_HEIGHT  = LAYER_HEIGHT;
	    public static readonly int KEYFRAME_SPACE   = 4;
	    public static readonly int KEYFRAME_SIMPLE_A    = 0;
	    public static readonly int KEYFRAME_SIMPLE_B    = 1;
	    public static readonly int KEYFRAME_FILL        = 2;
	    public static readonly int KEYFRAME_START       = 3;
	    public static readonly int KEYFRAME_END         = 4;
	    public static readonly int KEYFRAME_BETWEEN     = 5;
	    
	    public static readonly int LAYER_DELETE         = 0;
	    public static readonly int LAYER_MOVEUP         = 1;
	    public static readonly int LAYER_MOVEDOWN       = 2;
	    
	    public static readonly int KEYFRAME_ADD			= 100;
	    
	    public static readonly int TYPE_EMPTY = -1;
	    public static readonly int TYPE_START = KEYFRAME_START;
	    public static readonly int TYPE_END = KEYFRAME_END;
	    public static readonly int TYPE_BETWEEN = KEYFRAME_BETWEEN;
	    
	    /*
	     * Variables
	     */
	    private int                                         w                       = 0;
	    private int                                         h                       = 0;

		private bool										parentHeader;

	    public int                                          timeline_middle         = 0;
	    
	    public int                                          layer_selected          = -1;
	    private List<TlLayer>                               _layersDic              = new List<TlLayer>();

		private NthDimension.Forms.Widgets.ScrollBarH 		scrollFrames			= new NthDimension.Forms.Widgets.ScrollBarH();
		private NthDimension.Forms.Widgets.ScrollBarV		scrollLayers			= new NthDimension.Forms.Widgets.ScrollBarV();
	    
	    private int                                         guide_last_index        = 0;
	    public int                                          guide_max               = MAX_KEYFRAMES;
	    public int                                          guide_index             = 0;
	    public int                                          mouse_y                 = -1;


	    
	    public delegate void KeyframeChangeHandler(object sender, KeyframeArgs e);
	    public event KeyframeChangeHandler                  KeyframeChange          = null;
	    public delegate void LayerChangeHandler(object sender, LayerArgs e);
	    public event LayerChangeHandler                     LayerChange             = null;
	    public delegate void GuideChangeHandler(object sender, int newpos);
	    public event GuideChangeHandler                     GuideChange             = null;
	    
	    public TimeLine()
		{
			Size = new Size(100, 100);
			//InitializeComponent();
			KeyframeChange = null;

			this.scrollLayers.Hide();//.Visible = false;
			this.scrollFrames.Hide();// Visible = false;

			parentHeader = true;// (Parent is NthDimension.Forms.Window) ?  true/*((NthDimension.Forms.Window)Parent).h*/  : false;
		}
	    
        public void Clear ()
	    {
	        int i=0;
	        
	        for (i=0; i<_layersDic.Count; i++)
	        {
	            _layersDic[i]._keyframesDic.Clear();
	        }
	        
	        _layersDic.Clear();

			this.scrollLayers.Hide();//Visible = false;
			this.scrollFrames.Hide();// Visible = false;
			
			this.scrollLayers.Value = 0;
			this.scrollFrames.Value = 0;
	        
	        layer_selected = -1;
	    }
        public int AddLayer (string name)
        {
            _layersDic.Add(new TlLayer(name));
            
            if (_layersDic.Count > 0)
            {
				this.scrollLayers.Show();// = true;
                this.scrollLayers.Maximum = _layersDic.Count - 1;
            }
            
            mouse_y = -1;
            this.Invalidate();
            return _layersDic.Count - 1;
        }
        public int AddKeyframe (int layer, int position, int kindof)
        {
            if (layer < 0
            || layer > _layersDic.Count)
            {
                return -1;
            }
            
            TlLayer l = _layersDic[layer];
            
            l._keyframesDic[position].alt = kindof;
            
            this.Invalidate();
            return l.keyframes_count++;
        }

	    private void RecalcTimelineMiddle ()
	    {
	        timeline_middle = (MIDDLE_PERCENT*this.Width)/100;
	        this.scrollFrames.Location = new Point(timeline_middle, this.Height - this.scrollFrames.Height);
	        this.scrollFrames.Size = new Size(this.Width - (timeline_middle + this.scrollLayers.Width), this.scrollFrames.Height);

	        this.scrollLayers.Location = new Point(this.Width - this.scrollLayers.Width, 0);
	        this.scrollLayers.Size = new Size(this.scrollLayers.Width, this.Height - this.scrollFrames.Height);
	        
	        w = this.Width - this.scrollLayers.Width;
	        h = this.Height - this.scrollFrames.Height;
	    }
	    private void DrawTimeLineBackground (NthDimension.Forms.GContext g)
	    {
	        Pen p = new Pen(color_timeline_fkg);
	        SolidBrush b = new SolidBrush(color_timeline_bkg);
	        
	        g.FillRectangle(b, 
							timeline_middle, 
							0 + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), 
							(w-timeline_middle), 
							h + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0));

	        g.DrawLine(new NthDimension.Forms.NanoPen(p.Color), 
						timeline_middle - 1, 
						0 + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), 
						timeline_middle - 1, 
						h + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0));
	        g.DrawLine(new NthDimension.Forms.NanoPen(p.Color), 
						timeline_middle, 
						0 + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), 
						w, 
						0 + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0));
	        g.DrawLine(new NthDimension.Forms.NanoPen(p.Color), 
						timeline_middle, 
						h-1 + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), 
						w, 
						h-1 + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0));
	    }
	    private void DrawTopLineGuide (NthDimension.Forms.GContext g)
	    {
	        Pen p = new Pen(m_color_guide);
	        int x = timeline_middle + (guide_index * KEYFRAME_WIDTH) + guide_index;
	        
	        g.DrawRectangle(new NthDimension.Forms.NanoPen(p.Color), 
							x, 
							0 + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), 
							KEYFRAME_WIDTH, 
							KEYFRAME_HEIGHT + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0));
	        
			g.DrawLine(new NthDimension.Forms.NanoPen(p.Color), 
						x + (KEYFRAME_WIDTH / 2), 
						KEYFRAME_HEIGHT + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), 
						x + (KEYFRAME_WIDTH / 2),
						h + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0));
	        
	        if (GuideChange != null)
			{
            	GuideChange(this, guide_index);
			}
	    }
	    private void DrawTimeRuler (NthDimension.Forms.GContext g)
	    {
	        int i=0;
	        int x=0;
	        int y= 0 + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0);
	        int real_guide_index=guide_last_index;

	        //Font f = new Font("Sans",8);
	        Pen p = new Pen(m_colorRulerText);
	        SolidBrush b = new SolidBrush(color_keyframe_empty_b);
            SolidBrush m = new SolidBrush(m_colorRulerText);
	        
	        for (i=MAX_KEYFRAMES; i>-1; i--)
	        {
	            x = timeline_middle + (i * KEYFRAME_WIDTH) + i;
	            
	            if (guide_index >= x
	            && guide_index < x + KEYFRAME_WIDTH)
	            {
	                if (i <= guide_max)
	                {
	                    real_guide_index = i;
	                    guide_last_index = i;
	                }
	            }
	            
	            if (x > w)
	            {
	                continue;
	            }
	            
	            g.FillRectangle(b, 
								x, 
								y/* + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0)*/, 
								KEYFRAME_WIDTH, 
								KEYFRAME_HEIGHT/* + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0)*/);
	            g.DrawLine(new NthDimension.Forms.NanoPen(p.Color), 
						   x + KEYFRAME_WIDTH, 
						   y/* + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0)*/,
						   x + KEYFRAME_WIDTH, 
						   y + 2/* + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0)*/);
	            
	            if (i % TimeLine.KEYFRAME_SPACE == 0)
	            {
	                g.DrawString((i+1).ToString(), new NthDimension.Forms.NanoSolidBrush(m.Color), x - 2, y + 2);
	            }
	        }
	        
	        guide_index = real_guide_index;
	    }
	    private void DrawSimpleKeyframe (NthDimension.Forms.GContext g, int x, int y, int type)
	    {
	        NthDimension.Forms.NanoPen p = new NthDimension.Forms.NanoPen(color_keyframe_sep);
	        NthDimension.Forms.NanoSolidBrush b = null;
	        
	        if (type == KEYFRAME_SIMPLE_A)
	        {
	            b = new NthDimension.Forms.NanoSolidBrush(color_keyframe_empty_a);
	        }
	        else if (type == KEYFRAME_SIMPLE_B)
	        {
	            b = new NthDimension.Forms.NanoSolidBrush(color_keyframe_empty_b);
	        }
	        else if (type == KEYFRAME_START
	        || type == KEYFRAME_END
	        || type == KEYFRAME_BETWEEN)
	        {
	            b = new NthDimension.Forms.NanoSolidBrush(color_keyframe_fill);
	        }
	        
	        g.FillRectangle(b, x, y + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), KEYFRAME_WIDTH, KEYFRAME_HEIGHT);
	        
	        if (type == KEYFRAME_START
	        && type == KEYFRAME_END
	        && type == KEYFRAME_BETWEEN)
	        {
	            g.DrawLine(p, x + KEYFRAME_WIDTH, y + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), x + KEYFRAME_WIDTH, y + KEYFRAME_HEIGHT + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0));
	        }
	        
	        if (type == KEYFRAME_START)
	        {
	            b = new NthDimension.Forms.NanoSolidBrush(Color.White);
	            g.FillEllipse(b, new Rectangle(x-1, y + (KEYFRAME_HEIGHT/3) + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), KEYFRAME_WIDTH, KEYFRAME_WIDTH + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0)));
	        }
	        else if (type == KEYFRAME_END)
	        {
	            b = new NthDimension.Forms.NanoSolidBrush(Color.White);
	            g.FillRectangle(b, x + 1, y + (KEYFRAME_HEIGHT/3) + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), KEYFRAME_WIDTH - 2 + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), KEYFRAME_WIDTH);
	        }
	        else if (type == KEYFRAME_BETWEEN)
	        {
	            p = new NthDimension.Forms.NanoPen(Color.White);
	            g.DrawLine(p, x, y + (KEYFRAME_HEIGHT / 2) + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), x + KEYFRAME_WIDTH, y + (KEYFRAME_HEIGHT / 2) + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0));
	        }
	    }
	    private void DrawLayer (NthDimension.Forms.GContext g, int index, TlLayer layer)
	    {
	        Pen p = new Pen(color_timeline_fkg);
	        int i = 0;
	        int j = 0;
	        int y = (index * LAYER_HEIGHT) + LAYER_HEIGHT + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0);
	        bool drawbetweenframes = false;
	        
	        for (i=0; i<layer._keyframesDic.Count; i++)
            {
                if (drawbetweenframes
                && layer._keyframesDic[i].alt == KEYFRAME_END)
                {
                    drawbetweenframes = false;
                }
                
                if (!drawbetweenframes)
                {
                    if (layer._keyframesDic[i].alt != -1)
                    {
                        DrawSimpleKeyframe(g, timeline_middle + (i * KEYFRAME_WIDTH) + i, y, layer._keyframesDic[i].alt);
                        
                        if (layer._keyframesDic[i].alt == KEYFRAME_START)
                        {
                            for (j=i+1; j<layer._keyframesDic.Count; j++)
                            {
                                if (layer._keyframesDic[j].alt == KEYFRAME_END)
                                {
                                    drawbetweenframes = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        DrawSimpleKeyframe(g, timeline_middle + (i * KEYFRAME_WIDTH) + i, y, layer._keyframesDic[i].type);
                    }
                }
                else
                {
                    DrawSimpleKeyframe(g, timeline_middle + (i * KEYFRAME_WIDTH) + i, y, KEYFRAME_BETWEEN);
                }
            }
	        
	        if (layer_selected == -1 && mouse_y >= y && mouse_y < y + LAYER_HEIGHT)
	        {
	        	//Utils.Trace("902340");
	            SolidBrush b = new SolidBrush(Color.Black);
	            g.FillRectangle(b, 0, y, timeline_middle, LAYER_HEIGHT);
	            
	            p = new Pen(Color.White);
	            
	            g.DrawLine(new NthDimension.Forms.NanoPen(p), 0, y, w, y);
	            g.DrawString(layer.name, new NthDimension.Forms.NanoSolidBrush(Color.FromKnownColor(KnownColor.White)), 5, y + 2);
	            g.DrawLine(new NthDimension.Forms.NanoPen(p), 0, y + LAYER_HEIGHT, w, y + LAYER_HEIGHT);
	            
	            layer_selected = index;
//this.ContextMenuStrip = this.contextLayer;
	        }
	        else if (layer_selected != -1
	        && layer_selected - this.scrollLayers.Value == index)
	        {
	        	SolidBrush b = new SolidBrush(Color.Black);
	            g.FillRectangle(b, 0, y, timeline_middle, LAYER_HEIGHT);
	            
	            p = new Pen(Color.White);
	            
	            g.DrawLine(new NthDimension.Forms.NanoPen(p), 0, y, w, y);
	            g.DrawString(layer.name, new NthDimension.Forms.NanoSolidBrush(Color.FromKnownColor(KnownColor.White)), 5, y + 2);
	            g.DrawLine(new NthDimension.Forms.NanoPen(p), 0, y + LAYER_HEIGHT, w, y + LAYER_HEIGHT);
	            
//this.ContextMenuStrip = this.contextLayer;
	        }
	        else
	        {
	            g.DrawLine(new NthDimension.Forms.NanoPen(p), 0, y, w, y);
	            g.DrawString(layer.name, new NthDimension.Forms.NanoSolidBrush(Color.FromKnownColor(KnownColor.Black)), 5, y + 2);
	            g.DrawLine(new NthDimension.Forms.NanoPen(p), 0, y + LAYER_HEIGHT, w, y + LAYER_HEIGHT);
	        }
	    }
	    
        public void ControlPaint (object sender, PaintEventArgs e)
		{
            int i = 0;
            NthDimension.Forms.GContext g = e.GC;
            
            RecalcTimelineMiddle();

            
            //g.Clear(ColorARGB32.FromKnownColor(KnownColor.MenuBar));
            g.Clear(new NthDimension.Forms.NanoSolidBrush(color_clear));
            DrawTimeLineBackground(g);
            DrawTimeRuler(g);
            
            //layer_selected = -1;
            
            g.SetClip(new Rectangle(0, 0 + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), this.Width, h + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0)));
            for (i=0; i<_layersDic.Count; i++)
            {
                if (i + this.scrollLayers.Value < _layersDic.Count)
                {
                    DrawLayer(g, i, _layersDic[i + this.scrollLayers.Value]);
                }
            }
            g.SetClip(new Rectangle(0, 0 + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0), this.Width, this.Height + (parentHeader ? NthDimension.Forms.Window.DEF_HEADER_HEIGHT : 0)));
            
            if (layer_selected == -1)
	        {
//this.ContextMenuStrip = null;
            }
            
            DrawTopLineGuide(g);
            
            //g.Flush();
		}

		protected override void DoPaint(NthDimension.Forms.GContext parentGContext)
		{
			this.ControlPaint(this, new PaintEventArgs(parentGContext));
			base.DoPaint(parentGContext);

		}

		public void ControlMouseDown (object sender, MouseEventArgs e)
        {
            if (e.X > timeline_middle)
            {
            	guide_index = e.X;
            	this.Invalidate();
            }
            else
            {
            	layer_selected = -1;
                guide_index = e.X;
                mouse_y = e.Y;
                this.Invalidate();
            }
        }

		protected override void OnMouseDown(MouseDownEventArgs e)
		{
			this.ControlMouseDown(this, e);
			base.OnMouseDown(e);

		}

		void DeleteToolStripMenuItemClick(object sender, EventArgs e)
		{
		    if (LayerChange != null)
		    {
		        LayerChange(this, new LayerArgs(layer_selected + this.scrollLayers.Value, LAYER_DELETE));
		    }
		    
		    layer_selected = -1;
		}
		void MoveUpToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (LayerChange != null)
		    {
		        LayerChange(this, new LayerArgs(layer_selected + this.scrollLayers.Value, LAYER_MOVEUP));
		    }
		}
		void MoveDownToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (LayerChange != null)
		    {
		        LayerChange(this, new LayerArgs(layer_selected + this.scrollLayers.Value, LAYER_MOVEDOWN));
		    }
		}
		void InsertToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (KeyframeChange != null)
			{
				KeyframeChange(this, new KeyframeArgs(layer_selected + this.scrollLayers.Value, guide_index, KEYFRAME_ADD));
			}
		}
		void ScrLayersScroll(object sender, ScrollEventArgs e)
		{
		    this.Invalidate();
		}

        public void SetGuideIndex(int index)
        {
            this.guide_index = index;
            this.guide_last_index = index;

            if (GuideChange != null)
            {
                GuideChange(this, index);
            }
        }
	}
	

    public class KeyframeArgs
    {
        public int layer    = 0;
        public int keyframe	= 0;
        public int type		= 0;
        
        public KeyframeArgs(int s, int y, int k)
        {
            layer = s;
            keyframe = y;
            type = k;
        }
    }
    
    public class LayerArgs
    {
        public int layer    = 0;
        public int type     = 0;
        
        public LayerArgs(int l, int t)
        {
            layer = l;
            type = t;
        }
    }
    
    public class TlKeyframe
    {
        public int type = TimeLine.KEYFRAME_SIMPLE_A;
        public int alt = -1;
        
        public TlKeyframe()
        {
        }
    }
    
    public class TlLayer
    {
        public string name = "";
        public int keyframes_count = 0;
        public List<TlKeyframe> _keyframesDic = new List<TlKeyframe>();
        
        public TlLayer(string _name)
        {
            int i=0;
            
            for (i=0; i<TimeLine.MAX_KEYFRAMES; i++)
            {
                _keyframesDic.Add(new TlKeyframe());
                
                if ((i % TimeLine.KEYFRAME_SPACE) == 0)
                {
                    _keyframesDic[i].type = TimeLine.KEYFRAME_SIMPLE_B;
                }
            }
            
            name = _name;
        }
    }
}
