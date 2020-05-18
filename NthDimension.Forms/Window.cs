using NthDimension.Forms.Events;
using NthDimension.Forms.Layout;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Forms
{
    public enum WindowState
    {
        Normal,
        Maximized
    }
    /// <summary>
	/// Corresponds to an internal Window
	/// </summary>
	public class Window : Overlay
    {
        //private Panel m_formGrip;
        private const int GRIPHEIGHT = 5;

        protected WHUD m_win;

        public const int DEF_HEADER_HEIGHT = 24;
        readonly Label m_titleLabel;
        readonly CloseButton m_closeButton;

        private bool m_titlebar = true;
        private bool m_controlBox = true;

        private Size restoreSize = new Size();
        private WindowState windowState = WindowState.Normal;

        public bool AlwaysOnTop = true;

        public Label TitleLabel
        {
            get { return m_titleLabel; }
        }

        public Color HeaderColor = Color.FromArgb(255, 48, 80, 113);

        public Window(bool controlbox = true, bool titleBar = true)
        {
            this.m_controlBox = controlbox;
            this.m_titlebar = titleBar;

            NotifyForMouseDown = true;

            BGColor = Color.FromArgb(51, 51, 51);
            Size = new Size(200, 100);

            m_titleLabel = new Label();
            m_titleLabel.Margin = new Spacing(5, 0, 0, 10);
            m_titleLabel.Font = new NanoFont(NanoFont.DefaultRegular, 11f);
            m_titleLabel.FGColor = Color.WhiteSmoke;
            m_titleLabel.TextAlign = ETextAlignment.Left;
            m_titleLabel.MouseDoubleClickEvent += delegate 
                                                        {
                                                            if (windowState == WindowState.Normal)
                                                            {
                                                                restoreSize = this.Size;
                                                                this.Size = m_win.Size;
                                                            }
                                                            else if (windowState == WindowState.Maximized)
                                                            {
                                                                this.Size = restoreSize;
                                                            }
                                                        };

            if (m_controlBox)
            {
                m_closeButton = new CloseButton();
                m_closeButton.MouseClickEvent += delegate
                {
                    Close();
                };

                //Widgets.Add(iTitleLabel);
                Widgets.Add(m_closeButton);

                //this.m_formGrip = new Panel()
                //{
                //    Size = new Size(this.Width, GRIPHEIGHT),
                //    Dock = EDocking.Bottom,
                //    ShowBoundsLines = true
                    
                //};
            }
        }

        #region Properties

        public string Title
        {
            get { return m_titleLabel.Text; }
            set { m_titleLabel.Text = value; }
        }

        public bool IsDialog
        {
            get;
            protected internal set;
        }


        public bool ControlBox
        {
            get { return m_controlBox; }
            set
            {
                m_controlBox = value;
                if (!value)
                {
                    if (this.Widgets.Contains(m_closeButton))
                        this.Widgets.Remove(m_closeButton);
                }
                else
                {
                    if (!this.Widgets.Contains(m_closeButton))
                        this.Widgets.Add(m_closeButton);

                }
            }
        }


        #endregion Properties

        public virtual void Close()
        {
            Hide();
        }

        public override void Show(WHUD win)
        {
            m_win = win;

            base.Show(win);

            Focus();

            restoreSize = this.Size;
        }

        protected override void OnLayout()
        {
            base.OnLayout();

            if (m_titlebar)
            {
                Padding = new Spacing(0, DEF_HEADER_HEIGHT, 0, 0);
                titleRect = new Rectangle(10, 0, Width - 10 - 16, DEF_HEADER_HEIGHT);
                headerRect = new Rectangle(0, 0, Width, DEF_HEADER_HEIGHT);
                m_titleLabel.Size = new Size(m_titleLabel.AutoSize.Width, DEF_HEADER_HEIGHT);
                m_titleLabel.Location = new Point(5, 0);
                
            }

            if (m_controlBox)
            {
                m_closeButton.Bounds = new Rectangle(Width - 20, 2, 16, 16);
            }
        }

        public bool DownForMove
        {
            get;
            private set;
        }
        //public bool DownForResize
        //{
        //    get;
        //    private set;
        //}

        Rectangle titleRect;
        Rectangle headerRect;

        void DoMouseDown(MouseButton button, int x, int y)
        {
            if (button == MouseButton.Left)
            {
                if (titleRect.Contains(x, y))
                {
                    Point wp = WHUD.LibContext.CursorPos;
                    leftSpace = wp.X - X;
                    topSpace = wp.Y - Y;

                    DownForMove = true;
                    
                    Focus();
                    // Esto garantiza que se llamará a
                    // Window.OnMouseUp() al liberar el pulsador del ratón.
                    CaptureMouseFocus();
                }

            }
        }

        int leftSpace, topSpace;

        protected override void OnMouseDown(MouseDownEventArgs e)
        {
            DoMouseDown(e.Button, e.X, e.Y);

            e.FocusedLostFocusOnMouseDown = false;
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            
            base.OnMouseMove(e);
        }

        protected override void OnNotifyMouseDown(MouseEventArgs e)
        {
            DoMouseDown(e.Button, e.X, e.Y);

            base.OnNotifyMouseDown(e);
        }
        
        internal void NotifyForMouseGlobalMove(int x, int y)
        {
            if (DownForMove)
            {
                Point wp = WHUD.LibContext.CursorPos;
                //WindowHUD.Title = wp.ToString();
                Location = new Point(wp.X - leftSpace, wp.Y - topSpace);
                Repaint();
            }
            
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            DownForMove = false;
        }

        protected override void OnPaintBackground(GContext gc)
        {
            if (Widgets.Count <= 2)
                base.OnPaintBackground(gc);

            //if (m_controlBox)
            {
                gc.FillRectangle(new NanoSolidBrush(HeaderColor), headerRect);

                int tAscender = (int)System.Math.Ceiling(Font.Ascender);
                gc.DrawString(m_titleLabel.Text, m_titleLabel.Font, new NanoSolidBrush(m_titleLabel.FGColor), 10, tAscender + 2);
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            DownForMove = false;
            if (AlwaysOnTop)
            {
                if (null != m_win)
                    this.BringToFront(m_win);

                this.Focus();
            }

        }

        #region internal-class CloseButton

        class CloseButton : Button
        {
            bool isMouseOver;

            public CloseButton()
                : base(string.Empty)
            {
                PaintBackGround = false;
            }

            public void DrawCross(GContext gc)
            {
                if (isMouseOver)
                {
                    Color fill = Color.FromArgb(35, SystemColors.Highlight); //renderer.ColorTable.ButtonSelectedHighlight;

                    gc.FillRectangle(new SolidBrush(fill), Bounds);

                    Rectangle borderRect = Bounds;

                    borderRect.Width--;
                    borderRect.Height--;

                    gc.DrawRectangle(new NanoPen(SystemColors.Highlight), borderRect);
                }

                var pen = new NanoPen(Color.WhiteSmoke); //, 1.6f))
                {
                    pen.Width = 1.6f;
                    gc.DrawLine(pen, 3, 3,
                                Width - 5, Height - 4);

                    gc.DrawLine(pen, Width - 5, 3,
                                3, Height - 4);
                }
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                isMouseOver = true;
                Cursors.Cursor = Cursors.Hand;
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                isMouseOver = false;
            }

            protected override void DoPaint(PaintEventArgs e)
            {
                DrawCross(e.GC);
            }
        }




        #endregion internal-class CloseButton

        //public override void UpdateHitRectangle()
        //{
        //    //if(this.Widgets.Count > 0)
        //    //{
        //    //    if(this.Widgets[this.Widgets.Count-1] != this.m_formGrip)
        //    //    {
        //    //        this.Widgets.Remove(this.m_formGrip);
        //    //        this.Widgets.Add(this.m_formGrip);
        //    //    }
        //    //}

        //    base.UpdateHitRectangle();
        //}
    }
}
