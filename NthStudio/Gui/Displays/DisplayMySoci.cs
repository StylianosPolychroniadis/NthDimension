using NthDimension;
using NthDimension.Algebra;
using NthDimension.Forms;
using NthDimension.Forms.Events;
using NthDimension.Rendering.Utilities;
using NthDimension.Rasterizer.NanoVG;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthStudio.Gui.Displays
{
    public class ButtonClickedEventArgs : System.EventArgs
    {
    }
    public partial class DisplaySocialNet : Widget.WHUD, IUiContext
    {
        public enum enuScreeUIState
        {
            LoginScreen,
            GameScreen
        }

        public delegate void ClickButton(ButtonClickedEventArgs e);
        public event ClickButton OnButtonClicked;
        public static bool MousePointerInWindow;
        public static int TopBarHeight = 38;
        public static int NavBarWidth = 160;

        static enuScreeUIState m_screenMode = enuScreeUIState.LoginScreen;
        private StudioWindow m_windowsGame;
        private NVGcontext vg;
        private bool m_antializingAvailable;
        //private UserInfoDesc m_selectedUser;
        //private GUI.LoginForm m_loginForm;
        private bool m_init;
        private bool m_initMouse;
        //// Top Bar
        //private TopBar m_topBar;
        //// NavBar
        //private NavBar m_navBar;
        //// Chat
        //private ChatBase m_chat;
        //private LinearGradientBrush m_brush;
        //private LinearGradientMode m_mode = LinearGradientMode.Horizontal;
        //private IWindowHost navButtonFriends;
        //private IWindowHost navButtonPhotos;
        //// Forms
        //private SettingsForm m_formSettings;
        //// Notification Box
        //private NotificationBase m_notifier;

        #region Properties
        //public IWindowHost NavButtonFriends { get { return navButtonFriends; } private set { navButtonFriends = value; } }
        //public IWindowHost NavButtonPhotos { get { return navButtonPhotos; } private set { navButtonPhotos = value; } }
        // TODO the rest + Link Labels from TopBar
        public Rectangle WorkspaceRectangle
        {
            get { return new Rectangle(NavBarWidth, TopBarHeight, this.Width - NavBarWidth, this.Height - TopBarHeight); }
        }

        public enuScreeUIState ScreenState
        {
            get { return m_screenMode; }
        }

        //public List<NotificationWidget> Notifications
        //{
        //    get
        //    {
        //        if (null == m_notifier) return new List<NotificationWidget>();

        //        return m_notifier.NotificationWidgets;
        //    }

        //}

        //public WindowHome WindowHome { get { return m_topBar.LinkLabelHome.Window as WindowHome; } }
        //public WindowProfile WindowProfile { get { return m_topBar.LinkLabelProfile.Window as WindowProfile; } }

        //public bool WindowHomeReady
        //{
        //    get { return m_topBar.WindowHomeReady; }
        //}
        //public bool WindowProfileReady
        //{
        //    get { return m_topBar.WindowProfileReady; }
        //}
        #endregion

        #region Ctor
        public DisplaySocialNet(StudioWindow window, bool antializingSupport)
        {
            LibContext = this;
            this.m_windowsGame = window;
            this.m_antializingAvailable = antializingSupport;
            this.Name = "MySoci.net User Interface";
            this.vg = StudioWindow.vg;
        }
        ~DisplaySocialNet()
        {

        }
        #endregion Ctor

        public void Initialize(enuScreeUIState state = enuScreeUIState.LoginScreen)
        {
            //SuspendLayout();
            this.Size = new Size(this.m_windowsGame.Width, this.m_windowsGame.Height);
            this.PaintBackGround = false;
            //this.InitializeComponent();
            //ResumeLayout();

            //this.m_windowsGame.Cursor = new OpenTK.MouseCursor(
            //    (int)Cursors.Default.Cursors[0].Xhot,
            //    (int)Cursors.Default.Cursors[0].Yhot,
            //    (int)Cursors.Default.Cursors[0].Width,
            //    (int)Cursors.Default.Cursors[0].Height,
            //    Cursors.Default.Cursors[0].data);

           // Cursors.MouseCursorChanged += onWindow_MouseCursorChanged;


        }
        public void InitializeWindowHome()
        {
            //m_topBar.InitializeWindowHomeComponent();
        }
        public void InitializeWindowProfile()
        {
            //m_topBar.InitializeWindowProfileComponent();
        }
        public void InitializeComponent()
        {
            if (!m_init)
            {
                ////SuspendLayout();
                //this.Size = new Size(this.m_windowsGame.Width, this.m_windowsGame.Height);

                //this.m_navBar = new NavBar(new Size(NavBarWidth, this.Height - TopBarHeight),
                //   new Point(0, TopBarHeight),
                //   this.WorkspaceRectangle,
                //   this.m_brush,
                //   clickNavButton);
                //this.m_navBar.ShowMarginLines = false;

                //this.m_navBar.InitializeComponent(WorkspaceRectangle,
                //                                   out navButtonFriends,
                //                                   out navButtonPhotos);

                //this.m_topBar = new TopBar(new Size(this.Width, TopBarHeight),
                //    new Point(0, 0),
                //    this.WorkspaceRectangle,
                //    clickLinkLabel,
                //    clickToolButton);
                //this.m_topBar.ShowMarginLines = true; //false;

                //this.m_topBar.SetWorkspaceRectangle(WorkspaceRectangle);
                //this.m_navBar.SetWorkspaceRectangle(WorkspaceRectangle);

                //this.Widgets.Add(this.m_topBar);
                //this.Widgets.Add(this.m_navBar);

                //this.m_chat = new ChatBase();
                //this.m_chat.Size = new Size((int)(this.Width / 2.2f), (int)(this.Height / 4.4f));
                //this.m_chat.Location = new Point(this.Width / 2 - this.m_chat.Width / 2, this.Height - this.m_chat.Height - 10);
                //this.m_chat.InitializeComponent();
                //if (Program.Offline == true) this.m_chat.Hide();
                //else this.Widgets.Add(this.m_chat);

                //this.m_notifier = new NotificationBase();
                //this.m_notifier.Name = "Notifications Box";
                //this.m_notifier.Size = new Size(250, 300);
                //this.m_notifier.Location = new Point(this.Width - m_notifier.Width - 13, m_topBar.Height); // 13 is Scrollbar Width
                //this.m_notifier.BGColor = Color.FromArgb(255, 200, 200, 200);
                //this.m_notifier.InitializeComponent();

                m_init = true;

                ////StudioDesign.Instance.RequestFriendsList(Program.UserId);
            }
            //ResumeLayout();

        }

        #region Event Handlers

        #region Input Handlers  (Keyboard)
        public void KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            Keys k = TranslateKey(e.Key);
            var kea = new KeyEventArgs(k);

            ProcessKeyDown(kea);
        }
        public void KeyPress(object sender, OpenTK.KeyPressEventArgs e)
        {
            var kpe = new KeyPressedEventArgs(e.KeyChar);

            ProcessKeyPress(kpe);
        }
        public void KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            Keys k = TranslateKey(e.Key);
            var kea = new KeyEventArgs(k);

            ProcessKeyUp(kea);
        }

        #region TranslateKey
        Keys TranslateKey(OpenTK.Input.Key k)
        {
            #region Key-switch

            switch (k)
            {
                case OpenTK.Input.Key.A:
                    return Keys.A;
                case OpenTK.Input.Key.AltLeft:
                    return Keys.Alt;
                case OpenTK.Input.Key.AltRight:
                    return Keys.Alt;
                case OpenTK.Input.Key.B:
                    return Keys.B;
                case OpenTK.Input.Key.Back:
                    return Keys.Back;
                case OpenTK.Input.Key.BackSlash:
                    return Keys.OemBackslash;
                case OpenTK.Input.Key.BracketLeft:
                    return Keys.OemOpenBrackets;
                case OpenTK.Input.Key.BracketRight:
                    return Keys.OemCloseBrackets;
                case OpenTK.Input.Key.C:
                    return Keys.C;
                case OpenTK.Input.Key.CapsLock:
                    return Keys.CapsLock;
                case OpenTK.Input.Key.Clear:
                    return Keys.Clear;
                case OpenTK.Input.Key.Comma:
                    return Keys.Decimal;
                case OpenTK.Input.Key.ControlLeft:
                    return Keys.LControlKey;
                case OpenTK.Input.Key.ControlRight:
                    return Keys.RControlKey;
                case OpenTK.Input.Key.D:
                    return Keys.D;
                case OpenTK.Input.Key.Delete:
                    return Keys.Delete;
                case OpenTK.Input.Key.Down:
                    return Keys.Down;
                case OpenTK.Input.Key.E:
                    return Keys.E;
                case OpenTK.Input.Key.End:
                    return Keys.End;
                case OpenTK.Input.Key.Enter:
                    return Keys.Enter;
                case OpenTK.Input.Key.Escape:
                    return Keys.Escape;
                case OpenTK.Input.Key.F:
                    return Keys.F;
                case OpenTK.Input.Key.F1:
                    return Keys.F1;
                case OpenTK.Input.Key.F10:
                    return Keys.F10;
                case OpenTK.Input.Key.F11:
                    return Keys.F11;
                case OpenTK.Input.Key.F12:
                    return Keys.F12;
                case OpenTK.Input.Key.F13:
                    return Keys.F13;
                case OpenTK.Input.Key.F14:
                    return Keys.F14;
                case OpenTK.Input.Key.F15:
                    return Keys.F15;
                case OpenTK.Input.Key.F16:
                    return Keys.F16;
                case OpenTK.Input.Key.F17:
                    return Keys.F17;
                case OpenTK.Input.Key.F18:
                    return Keys.F18;
                case OpenTK.Input.Key.F19:
                    return Keys.F19;
                case OpenTK.Input.Key.F2:
                    return Keys.F2;
                case OpenTK.Input.Key.F20:
                    return Keys.F20;
                case OpenTK.Input.Key.F21:
                    return Keys.F21;
                case OpenTK.Input.Key.F22:
                    return Keys.F22;
                case OpenTK.Input.Key.F23:
                    return Keys.F23;
                case OpenTK.Input.Key.F24:
                    return Keys.F24;
                case OpenTK.Input.Key.F3:
                    return Keys.F3;
                case OpenTK.Input.Key.F4:
                    return Keys.F4;
                case OpenTK.Input.Key.F5:
                    return Keys.F5;
                case OpenTK.Input.Key.F6:
                    return Keys.F6;
                case OpenTK.Input.Key.F7:
                    return Keys.F7;
                case OpenTK.Input.Key.F8:
                    return Keys.F8;
                case OpenTK.Input.Key.F9:
                    return Keys.F9;
                case OpenTK.Input.Key.G:
                    return Keys.G;
                case OpenTK.Input.Key.H:
                    return Keys.H;
                case OpenTK.Input.Key.Home:
                    return Keys.Home;
                case OpenTK.Input.Key.I:
                    return Keys.I;
                case OpenTK.Input.Key.Insert:
                    return Keys.Insert;
                case OpenTK.Input.Key.J:
                    return Keys.J;
                case OpenTK.Input.Key.K:
                    return Keys.K;
                case OpenTK.Input.Key.L:
                    return Keys.L;
                case OpenTK.Input.Key.Left:
                    return Keys.Left;
                case OpenTK.Input.Key.LShift:
                    return Keys.LShiftKey;
                case OpenTK.Input.Key.LWin:
                    return Keys.LWin;
                case OpenTK.Input.Key.M:
                    return Keys.M;
                case OpenTK.Input.Key.Menu:
                    return Keys.Menu;
                case OpenTK.Input.Key.Minus:
                    return Keys.OemMinus;
                case OpenTK.Input.Key.N:
                    return Keys.N;
                case OpenTK.Input.Key.Number0:
                    return Keys.NumPad0;
                case OpenTK.Input.Key.Number1:
                    return Keys.NumPad1;
                case OpenTK.Input.Key.Number2:
                    return Keys.NumPad2;
                case OpenTK.Input.Key.Number3:
                    return Keys.NumPad3;
                case OpenTK.Input.Key.Number4:
                    return Keys.NumPad4;
                case OpenTK.Input.Key.Number5:
                    return Keys.NumPad5;
                case OpenTK.Input.Key.Number6:
                    return Keys.NumPad6;
                case OpenTK.Input.Key.Number7:
                    return Keys.NumPad7;
                case OpenTK.Input.Key.Number8:
                    return Keys.NumPad8;
                case OpenTK.Input.Key.Number9:
                    return Keys.NumPad9;
                case OpenTK.Input.Key.NumLock:
                    return Keys.NumLock;
                case OpenTK.Input.Key.O:
                    return Keys.O;
                case OpenTK.Input.Key.P:
                    return Keys.P;
                case OpenTK.Input.Key.PageDown:
                    return Keys.PageDown;
                case OpenTK.Input.Key.PageUp:
                    return Keys.PageUp;
                case OpenTK.Input.Key.Pause:
                    return Keys.Pause;
                case OpenTK.Input.Key.Period:
                    return Keys.OemPeriod;
                case OpenTK.Input.Key.Plus:
                    return Keys.Add;
                case OpenTK.Input.Key.PrintScreen:
                    return Keys.PrintScreen;
                case OpenTK.Input.Key.Q:
                    return Keys.Q;
                case OpenTK.Input.Key.Quote:
                    return Keys.OemQuotes;
                case OpenTK.Input.Key.R:
                    return Keys.R;
                case OpenTK.Input.Key.Right:
                    return Keys.Right;
                case OpenTK.Input.Key.RShift:
                    return Keys.RShiftKey;
                case OpenTK.Input.Key.RWin:
                    return Keys.RWin;
                case OpenTK.Input.Key.S:
                    return Keys.S;
                case OpenTK.Input.Key.ScrollLock:
                    return Keys.Scroll;
                case OpenTK.Input.Key.Semicolon:
                    return Keys.OemSemicolon;
                case OpenTK.Input.Key.Slash:
                    return Keys.OemMinus;
                case OpenTK.Input.Key.Sleep:
                    return Keys.Sleep;
                case OpenTK.Input.Key.Space:
                    return Keys.Space;
                case OpenTK.Input.Key.T:
                    return Keys.T;
                case OpenTK.Input.Key.Tab:
                    return Keys.Tab;
                case OpenTK.Input.Key.U:
                    return Keys.U;
                case OpenTK.Input.Key.Up:
                    return Keys.Up;
                case OpenTK.Input.Key.V:
                    return Keys.V;
                case OpenTK.Input.Key.W:
                    return Keys.W;
                case OpenTK.Input.Key.X:
                    return Keys.X;
                case OpenTK.Input.Key.Y:
                    return Keys.Y;
                case OpenTK.Input.Key.Z:
                    return Keys.Z;
                case OpenTK.Input.Key.KeypadDivide:
                    return Keys.Divide;
                case OpenTK.Input.Key.KeypadMultiply:
                    return Keys.Multiply;
                case OpenTK.Input.Key.Keypad0:
                    return Keys.NumPad0;
                case OpenTK.Input.Key.Keypad1:
                    return Keys.NumPad1;
                case OpenTK.Input.Key.Keypad2:
                    return Keys.NumPad2;
                case OpenTK.Input.Key.Keypad3:
                    return Keys.NumPad3;
                case OpenTK.Input.Key.Keypad4:
                    return Keys.NumPad4;
                case OpenTK.Input.Key.Keypad5:
                    return Keys.NumPad5;
                case OpenTK.Input.Key.Keypad6:
                    return Keys.NumPad6;
                case OpenTK.Input.Key.Keypad7:
                    return Keys.NumPad7;
                case OpenTK.Input.Key.Keypad8:
                    return Keys.NumPad8;
                case OpenTK.Input.Key.Keypad9:
                    return Keys.NumPad9;
                case OpenTK.Input.Key.KeypadAdd:
                    return Keys.Add;
                case OpenTK.Input.Key.KeypadMinus:
                    return Keys.Subtract;
                case OpenTK.Input.Key.KeypadDecimal:
                    return Keys.Decimal;
                case OpenTK.Input.Key.KeypadEnter:
                    return Keys.Enter;
                default:
                    NthDimension.Rendering.Utilities.ConsoleUtil.errorlog("ScrenUI.TranslateKey ",
                        String.Format("Key '{0}' not defined.", k));
                    return Keys.None;

            }
            #endregion Key-switch
        }
        #endregion TranslateKey

        #endregion Input Handlers (Keyboard)

        #region Input Handlers (Mouse)
        public void MouseDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            var mbe = new MouseButtonEventArgs(e.X, e.Y,
                                                       TranslateButton(e.Button),
                                                       e.IsPressed);
            ProcessMouseDown(mbe);
        }
        public void MouseUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            var mbe = new MouseButtonEventArgs(e.X, e.Y,
                                                       TranslateButton(e.Button),
                                                       e.IsPressed);
            ProcessMouseUp(mbe);
        }
        public void MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            try
            {
                MouseButton emb = MouseButton.None;

                if (e.Mouse.IsAnyButtonDown)
                {
                    if (e.Mouse.IsButtonDown(OpenTK.Input.MouseButton.Left))
                        emb = MouseButton.Left;
                    else if (e.Mouse.IsButtonDown(OpenTK.Input.MouseButton.Middle))
                        emb = MouseButton.Middle;
                    else if (e.Mouse.IsButtonDown(OpenTK.Input.MouseButton.Right))
                        emb = MouseButton.Right;
                }


                var ne = new MouseEventArgs(emb, e.X, e.Y, e.XDelta, e.YDelta);
                ProcessMouseMove(ne);
            }
            catch { }
        }
        public void MouseWheel(object sender, OpenTK.Input.MouseWheelEventArgs e)
        {
            var me = new MouseEventArgs(MouseButton.None,
                                                e.X,
                                                e.Y,
                                                0, 0, (int)e.DeltaPrecise);
            ProcessMouseWheel(me);
        }
        private void onWindow_MouseEnter(object sender, EventArgs e)
        {
            ProcessMouseEnter(e);
        }
        private void onWindow_MouseLeave(object sender, EventArgs e)
        {
            /*// Error de OpenTK en Windows, cuando se pulsa el ratón 'MouseDown'
			// también se dispara el evento 'MouseLeave'.
			if (Environment.OSVersion.Platform != PlatformID.Unix && MouseUpFromClick)
				return;*/
            ProcessMouseLeave(e);
        }
        private void onWindow_MouseCursorChanged(object sender, MouseCursorChangedEventArgs args)
        {
            this.m_windowsGame.Cursor = new OpenTK.MouseCursor(
                (int)args.NewCursor.Cursors[0].Xhot,
                (int)args.NewCursor.Cursors[0].Yhot,
                (int)args.NewCursor.Cursors[0].Width,
                (int)args.NewCursor.Cursors[0].Height,
                args.NewCursor.Cursors[0].data);
        }

        #region Translate MouseButton
        MouseButton TranslateButton(OpenTK.Input.MouseButton mb)
        {
            switch (mb)
            {
                case OpenTK.Input.MouseButton.Left:
                    return MouseButton.Left;
                case OpenTK.Input.MouseButton.Middle:
                    return MouseButton.Middle;
                case OpenTK.Input.MouseButton.Right:
                    return MouseButton.Right;
                default:
                    return MouseButton.None;
            }
        }
        #endregion Translate MouseButton

        #endregion Input Handlers (Mouse)

        public void FocusedChanged(object sender, EventArgs e)
        {
            Repaint();
        }
        #endregion

        #region IUiContext implementation

        public Point CursorPos { get; set; }
        public bool ShowCursor { get; set; }
        public FontParameters CreateFont(NanoFont font)
        {
            int id;

            if (font.FontData == null)
                id = NanoVG.CreateFont(vg, font.InternalName, font.FileName);
            else
                id = NanoVG.CreateFont(vg, font.InternalName, font.FontData);

            return GetFontParemeters(font, id, font.Height);
        }
        public FontParameters GetFontParemeters(NanoFont font, int id, float height)
        {
            float ascender = 0f;
            float descender = 0f;
            float lineHeight = 0f;

            if (id < 0)
            {
                if (font.FontData == null)
                    throw new System.IO.FileNotFoundException("Fuente de texto '{0}' no válida.", font.FileName);
            }
            var fp = new FontParameters();

            //NanoVG.nvgSave(vg);
            try
            {
                NanoVG.nvgFontSize(vg, height);
                NanoVG.nvgFontFace(vg, id);
                NanoVG.nvgTextMetrics(vg, ref ascender, ref descender, ref lineHeight);

                fp.Ascender = ascender;
                fp.Descender = descender;
                fp.FontHeight = ascender - descender;
                fp.LineHeight = lineHeight;
                fp.ID = id;
            }
            catch { }
            //NanoVG.nvgRestore(vg);

            return fp;
        }
        // TODO:: Transfer to NanoFont

        //private Dictionary<NanoFont, Size> fontsLookUp = new Dictionary<NanoFont, Size>();
        public virtual Size MeasureText(string text, NanoFont font)
        {
            if (!NanoVG.FontCreated(vg, font.Id))
                throw new Exception(string.Format("Font '{0}', not created", font));

            //if (fontsLookUp.ContainsKey(font))
            //    return fontsLookUp[font];

            Size s = new Size();

            try
            {
                var bounds = new float[4];


                //NanoVG.nvgSave(vg);

                NanoVG.nvgFontSize(vg, font.Height);
                NanoVG.nvgFontFace(vg, font.Id);

                NanoVG.nvgTextBounds(vg, 0, 0, text, bounds);

                //NanoVG.nvgRestore(vg);

                s = new Size((int)(bounds[2] - bounds[0]), (int)(bounds[3] - bounds[1]));

                //fontsLookUp.Add(font, s);
            }
            catch
            {

            }

            return s;
        }
        // TODO:: Transfer to NanoFont
        public virtual Size MeasureGlyph(byte[] text, NanoFont font)
        {
            if (!NanoVG.FontCreated(vg, font.Id))
                throw new Exception(string.Format("Font '{0}', not created", font));

            var bounds = new float[4];


            //NanoVG.nvgSave(vg);

            NanoVG.nvgFontSize(vg, font.FontSize);
            NanoVG.nvgFontFace(vg, font.Id);

            NanoVG.nvgTextBounds(vg, 0, 0, text, bounds);

            //NanoVG.nvgRestore(vg);

            var s = new Size((int)(bounds[2] - bounds[0]), (int)(bounds[3] - bounds[1]));

            return s;
        }
        // TODO:: Transfer to NanoFont
        public virtual float MeasureTextWidth(string text, NanoFont font)
        {
            if (!NanoVG.FontCreated(vg, font.Id))
                throw new Exception(String.Format("Font '{0}', not created", font.ToString()));

            //NanoVG.nvgSave(vg);

            NanoVG.nvgFontSize(vg, font.Height);
            NanoVG.nvgFontFace(vg, font.Id);

            float textWidth = 100f;

            try { textWidth = NanoVG.nvgTextBounds(vg, 0, 0, text); }
            catch { ConsoleUtil.errorlog("Failed to Measure Text", text); }

            //NanoVG.nvgRestore(vg);

            return textWidth;
        }
        #endregion IUiContext

        public void StartChat()
        {

        }

        public void StopChat()
        {

        }
        public void Render()
        {
            this.Repaint();

            if (DoRepaintTree)
            {
                //var ngc = new NanoGContext(this, new Rectangle(0, 0, Width, Height), vg);
                var ngc = new NanoGContext(this, new Rectangle(0, 0, Width, Height), StudioWindow.vg);
                DoPaint(ngc);

                if (!m_initMouse)
                {
                    OpenTK.Input.MouseState ms = OpenTK.Input.Mouse.GetCursorState();
                    Point lmp = m_windowsGame.PointToClient(new Point(ms.X, ms.Y));
                    InitMousePosition(lmp.X, lmp.Y);
                    m_initMouse = true;
                }
            }
        }

        public void Update()
        {
            switch (m_screenMode)
            {
                case enuScreeUIState.GameScreen:
                    {
                        if (!m_init)
                        {
                            this.InitializeComponent();

                            //if (!m_topBar.IsVisible)
                            //    m_topBar.Show();

                            //clickLinkLabel(m_topBar.LinkLabelHome);

                            //((StudioDesign)StudioDesign.Instance).RequestFriendshipRequests(Program.UserId);
                        }

                        //if (!m_navBar.IsVisible && !hideRequest)
                        //    m_navBar.Show();

                        //if (!m_topBar.IsVisible && !hideRequest)
                        //    m_topBar.Show();

                        OpenTK.Input.MouseState cs = OpenTK.Input.Mouse.GetCursorState();
                        bool tMousePointerInWindow = m_windowsGame.Bounds.Contains(new Point(cs.X, cs.Y));

                        if (tMousePointerInWindow && !MousePointerInWindow)
                            onWindow_MouseEnter(this.m_windowsGame, EventArgs.Empty);
                        if (!tMousePointerInWindow && MousePointerInWindow)
                            onWindow_MouseLeave(this.m_windowsGame, EventArgs.Empty);

                        MousePointerInWindow = tMousePointerInWindow;

                        try { ProcessUpdate(); } catch { }

                        if (!hideRequest)
                        {
                            //this.m_topBar.SetWorkspaceRectangle(WorkspaceRectangle);
                            //this.m_navBar.SetWorkspaceRectangle(WorkspaceRectangle);
                        }


                        break;
                    }
                case enuScreeUIState.LoginScreen:
                    {
                        //if (!Program.Offline)
                        //{
                        //    if (null != this.m_navBar)
                        //        if (!m_navBar.IsHide)
                        //            this.m_navBar.Hide();

                        //    if (null != this.m_topBar)
                        //        if (!m_topBar.IsHide)
                        //            this.m_topBar.Hide();

                        //    if (null == this.m_loginForm)
                        //        this.m_loginForm = new GUI.LoginForm();

                        //    if (!this.m_loginForm.IsVisible)
                        //        this.m_loginForm.Show(((StudioDesign)StudioDesign.Instance).Screen2D.WindowHUD);
                        //}
                        //else
                        //    this.CloseLoginForm();

                        try { ProcessUpdate(); } catch { }
                        break;
                    }
            }

            //if (null != m_chat)
            //    if (StudioDesign.Instance.GameState == SYSCON.Graphics.GameState.Playing)
            //    {
            //        if (m_chat.IsHide)
            //            m_chat.Show();
            //    }
            //    else
            //    {
            //        if (!m_chat.IsHide)
            //            m_chat.Hide();
            //    }
        }

        //private void clickNavButton(NavButton sender)
        //{
        //    // Close currently open windows
        //    foreach (Widget nb in m_navBar.Widgets)
        //        if (nb is IWindowHost)
        //            if (sender == nb)
        //            {
        //                if (((IWindowHost)nb).WindowActive)
        //                    ((IWindowHost)nb).CloseWindow();
        //                else
        //                    ((IWindowHost)nb).ShowWindow();
        //            }
        //            else
        //                ((IWindowHost)nb).CloseWindow();

        //    foreach (Widget lb in m_topBar.Widgets)
        //        if (lb is IWindowHost)
        //            ((IWindowHost)lb).CloseWindow();

        //}

        //private void clickLinkLabel(LinkLabel sender)
        //{
        //    foreach (Widget lb in m_topBar.Widgets)
        //        if (lb is IWindowHost)
        //            if (sender == lb)
        //            {
        //                if (((IWindowHost)lb).WindowActive)
        //                    ((IWindowHost)lb).CloseWindow();
        //                ((IWindowHost)lb).ShowWindow();
        //            }
        //            else
        //                ((IWindowHost)lb).CloseWindow();

        //    foreach (Widget nb in m_navBar.Widgets)
        //        if (nb is IWindowHost)
        //            ((IWindowHost)nb).CloseWindow();
        //}


        //private void clickToolButton(ToolButton sender)
        //{

        //    #region DEBUG - DELETE
        //    //if (sender is ToolButtonHelp)
        //    //{
        //    //    VRWindows.Forms.TexturesForm tForm = new Forms.TexturesForm();
        //    //    tForm.Show();
        //    //}
        //    #endregion 

        //    if (sender is ToolButtonSettings)
        //    {
        //        if (null == m_formSettings)
        //            m_formSettings = new SettingsForm();
        //        m_formSettings.Show(this);
        //    }

        //    if (sender is ToolButtonAlerts)
        //    {
        //        if (m_notifier.IsHide)
        //        {
        //            m_notifier.Show(this);
        //            m_notifier.BringToFront(this);
        //        }
        //        else
        //            m_notifier.Hide();
        //    }



        //    /////////////////
        //    //foreach (Widget lb in m_topBar.Widgets)
        //    //    if (lb is IWindowHost)
        //    //        if (sender == lb)
        //    //        {
        //    //            if (((IWindowHost)lb).WindowActive)
        //    //                ((IWindowHost)lb).CloseWindow();
        //    //            else
        //    //                ((IWindowHost)lb).ShowWindow();
        //    //        }
        //    //        else
        //    //            ((IWindowHost)lb).CloseWindow();

        //    //foreach (Widget nb in m_navBar.Widgets)
        //    //    if (nb is IWindowHost)
        //    //        ((IWindowHost)nb).CloseWindow();

        //}

        ///// <summary>
        ///// Closes all other instances of IWindowHost and opens the selected one
        ///// </summary>
        ///// <param name="sender"></param>
        //private void clickLinkLabel_CloseAll(LinkLabel sender)
        //{
        //    foreach (Widget lb in m_topBar.Widgets)
        //        if (lb is IWindowHost)
        //            if (sender == lb)
        //            {
        //                if (((IWindowHost)lb).WindowActive)
        //                    ((IWindowHost)lb).CloseWindow();
        //                else
        //                    ((IWindowHost)lb).ShowWindow();
        //            }
        //            else
        //                ((IWindowHost)lb).CloseWindow();

        //    foreach (Widget nb in m_navBar.Widgets)
        //        if (nb is IWindowHost)
        //            ((IWindowHost)nb).CloseWindow();
        //}

        //public bool WindowActive => (this.m_navBar.WindowActive || this.m_topBar.WindowActive);

        //public void MyNameIs(string first, string last)
        //{
        //    m_navBar.MyNameIs(first, last);
        //    m_navBar.ProfilePictureChanged();
        //}

        //public void BadLogin()
        //{
        //    this.m_loginForm.LoginFailed();
        //}

        //public void Scene3DReady(bool ready)
        //{
        //    if (null != this.m_topBar)
        //        this.m_topBar.Set3DReady(ready);
        //}

        public void CloseAllWindows()
        {
            //foreach (Widget nb in m_topBar.Widgets)
            //    if (nb is IWindowHost)
            //        ((IWindowHost)nb).CloseWindow();

            //foreach (Widget nb in m_navBar.Widgets)
            //    if (nb is IWindowHost)
            //        ((IWindowHost)nb).CloseWindow();
        }
        public void CloseLoginForm()
        {
            m_screenMode = enuScreeUIState.GameScreen;

            //if (null != m_loginForm)    // Offline Mode
            //{
            //    m_loginForm.Ready = true;
            //    m_loginForm.Close();
            //}

            //ShowHomeWindow();

        }
        public void ToolNotifierAcknowledgeOne()
        {
            //m_topBar.NotifierAcknowledgeOne();
        }
        public void ShowHomeWindow()//WHUD win)
        {
            //if (!m_topBar.LinkLabelHome.Window.IsVisible)
            //    clickLinkLabel(m_topBar.LinkLabelHome);
        }
        public void ShowProfileWindow()
        {
            //if (!m_topBar.LinkLabelProfile.Window.IsVisible)
            //    clickLinkLabel(m_topBar.LinkLabelProfile);
        }

        public void SetLoadingProgress(float progress, string scene = "")
        {
            //this.m_topBar.SetLoadingProgress(progress, scene);
        }

        private bool hideRequest = false;
        public bool HideRequested
        {
            get { return hideRequest; }
        }
        public void HideAll()
        {
            hideRequest = true;
            CloseAllWindows();
            //m_topBar.Hide();
            //m_navBar.Hide();
        }

        //public void NewChatUsers(ConcurrentDictionary<string, string> users, ConcurrentDictionary<string, string> locations)
        //{
        //    if (null == this.m_chat) return;
        //    this.m_chat.UpdateChatUsers(users, locations);
        //}
        //public void NewChatMessage(PacketChat.EnvelopeSignalR chatEvent)
        //{
        //    if (null == this.m_chat) return;
        //    this.m_chat.UpdateChatMessage(chatEvent);
        //}
        //public void NewWallMessage(WallInfoDesc desc)
        //{
        //    if (null == m_topBar) return;

        //    if (m_topBar.LinkLabelHome.Window.IsVisible)
        //        m_topBar.NewHomePostItem(desc);
        //    if (m_topBar.LinkLabelProfile.Window.IsVisible)
        //        m_topBar.NewProfilePostItem(desc);
        //}
        private DateTime wallMessagesEndedTimeout = DateTime.Now;
        public void WallMessagesEnded(string userId)
        {
            //if (null == m_topBar) return;

            //((WindowProfile)m_topBar.LinkLabelProfile.Window).UserIdToDisplay = Program.UserId;


        }
        //public void NewFriend(FriendInfoDesc friend)
        //{
        //    if (null == m_navBar) return;
        //    if (null == m_topBar) return;

        //    if (m_navBar.WindowActive)
        //        m_navBar.CreateFriend(friend);
        //    if (m_topBar.WindowActive)
        //        m_topBar.CreateFriend(friend);
        //}
        //public void NewPhoto(string userId, string imgUrl, string time)
        //{
        //    if (null == m_navBar) return;
        //    if (null == m_topBar) return;

        //    if (m_navBar.WindowActive)
        //        m_navBar.CreatePhoto(userId, imgUrl, time);
        //    if (m_topBar.WindowActive)
        //        m_topBar.CreatePhoto(userId, imgUrl, time);



        //}
        //public void AvatarSelect(UserInfoDesc desc)
        //{
        //    //this.SelectedChatUser(m_selectedUser = desc);
        //    m_selectedUser = desc;

        //    Guid user = new Guid();
        //    Guid.TryParse(desc.UserId, out user);

        //    string profilePicture = Path.Combine(DirectoryUtil.AppData_MySoci_Profiles, string.Format("{0}.jpg", desc.UserId));

        //    Guid gusr = new Guid();
        //    Guid.TryParse(desc.UserId, out gusr);

        //    if (!File.Exists(profilePicture))
        //    {
        //        try
        //        {
        //            Bitmap bmp = StudioDesign.Instance.GetUserProfileImage(gusr);
        //            if (null != bmp)
        //                bmp.Save(profilePicture, System.Drawing.Imaging.ImageFormat.Jpeg);
        //            bmp.Dispose();
        //        }
        //        catch
        //        {
        //            SYSCON.Graphics.Utilities.ConsoleUtil.log("Failed to downloadload user profile picture");
        //        }
        //        //System.Threading.Thread.Sleep(200);
        //    }

        //    this.m_navBar.SetProfileSelected(profilePicture, desc.FirstName, desc.LastName);
        //}
        //public void AvatarUnselect(UserInfoDesc desc)
        //{

        //}
        //public void SelectedChatUser(UserInfoDesc desc)
        //{
        //    this.m_navBar.UpdateChatUserSelection(desc);
        //}
        //public void SelectedUser(UserInfoDesc desc)
        //{
        //    this.m_navBar.UpdateChatUserSelection(desc);
        //}
        //public void MessagesCounterIncrement()
        //{
        //    this.m_topBar.MessagesCounterIncrement();
        //}
        //public void MessagesCounterReset()
        //{
        //    this.m_topBar.MessagesCounterReset();
        //}
        // SCENE

        DateTime switchProfileTimeout = DateTime.Now;
        public void SwitchProfileUser(string userId)
        {
            //UserInfoDesc uu = StudioDesign.UserInfos.Where(e => e.Value.UserId == userId).FirstOrDefault().Value;

            //if (null == uu)
            //{
            //    StudioDesign.Instance.RequestUserDetails(userId, string.Empty);
            //    switchProfileTimeout = DateTime.Now.AddSeconds(10);
            //}

            //while (null == uu)
            //{
            //    uu = StudioDesign.UserInfos.Where(e => e.Value.UserId == userId).FirstOrDefault().Value;
            //    if (DateTime.Now > switchProfileTimeout)
            //        break;
            //}

            //if (null != uu)
            //{
            //    m_topBar.SwitcProfileUser(uu);
            //}

        }

        //public void LikeMessage(LikeInfoDesc like)
        //{
        //    if (null == m_topBar) return;

        //    if (like.WhomLikes == Program.UserId &&
        //        like.WhoLikes != Program.UserId &&
        //        bool.Parse(like.Acknowledged) == false)
        //    {
        //        UserInfoDesc uid = new UserInfoDesc();


        //        foreach (KeyValuePair<string, UserInfoDesc> kvp in StudioDesign.UserInfos)
        //        {
        //            if (kvp.Value.UserId == like.WhomLikes)
        //                uid = kvp.Value;
        //        }

        //        if (String.IsNullOrEmpty(uid.UserId))
        //            StudioDesign.Instance.RequestUserDetails(uid.UserId, string.Empty);

        //        string usr = String.IsNullOrEmpty(uid.UserId) ? "A user" : string.Format("{0} {1}", uid.FirstName, uid.LastName);

        //        NotificationLike nlr = new NotificationLike("Like Notification", string.Format("{0} likes your post", usr), like);

        //        nlr.Size = new Size(this.m_notifier.Width - 20, 35);
        //        nlr.InitializeComponent();

        //        this.m_notifier.AddItem(nlr);

        //        //m_topBar.ToolButtonAlerts.EventsCount++;
        //    }

        //    m_topBar.LikeMessage(like);
        //}

        //public void CommentMessage(CommentInfoDesc comment)
        //{
        //    if (null == m_topBar) return;

        //    m_topBar.CommentMessage(comment);
        //}
        //public void NewFriendRequest(FriendshipInfoDesc fid)                // Refactored from Frienships to NewFriendRequest (Jun-06-18)
        //{
        //    if (null == m_topBar) return;

        //    UserInfoDesc uid = new UserInfoDesc();


        //    foreach (KeyValuePair<string, UserInfoDesc> kvp in StudioDesign.UserInfos)
        //    {
        //        if (kvp.Value.UserId == fid.SourceUserId)
        //            uid = kvp.Value;
        //    }

        //    if (String.IsNullOrEmpty(uid.UserId))
        //        StudioDesign.Instance.RequestUserDetails(uid.UserId, string.Empty);

        //    NotificationFriendRequest nfr = new NotificationFriendRequest(fid.SourceUserId,
        //                                                    "New friend request from",
        //                                                    string.Format("{0} {1}", uid.FirstName, uid.LastName),
        //                                                    fid,
        //                                                    typeof(FriendshipInfoDesc));

        //    nfr.Size = new Size(this.m_notifier.Width - 20, 35);
        //    nfr.InitializeComponent();

        //    this.m_notifier.AddItem(nfr);

        //    m_topBar.ToolButtonAlerts.EventsCount++;
        //    m_topBar.Friendships(fid);


        //}



    }

    public class NanoGContext : GContext
    {
        [Flags]
        public enum EAlign
        {
            // Horizontal align
            ALIGN_LEFT = 1 << 0,
            // Default, align text horizontally to left.
            ALIGN_CENTER = 1 << 1,
            // Align text horizontally to center.
            ALIGN_RIGHT = 1 << 2,
            // Align text horizontally to right.
            // Vertical align
            ALIGN_TOP = 1 << 3,
            // Align text vertically to top.
            ALIGN_MIDDLE = 1 << 4,
            // Align text vertically to middle.
            ALIGN_BOTTOM = 1 << 5,
            // Align text vertically to bottom.
            ALIGN_BASELINE = 1 << 6,
            // Default, align text vertically to baseline.
        }
        public enum ESolidity
        {
            // CCW
            SOLID = 1,
            // CW
            HOLE = 2,
        }

        readonly NVGcontext vg;

        public NanoGContext(Widget owner, Rectangle parentWinClipRect, NVGcontext vg)
            : base(owner, parentWinClipRect)
        {
            this.vg = vg;
        }

        public override GContext CreateGContext(Widget owner)
        {
            var gc = new NanoGContext(owner, topWinClipRect, vg);
            return gc;
        }

        #region Util

        private Color NVGcolorToColor(NVGcolor nvgColor)
        {
            Color c = Color.FromArgb((int)(nvgColor.a * 255),
                                     (int)(nvgColor.r * 255),
                                     (int)(nvgColor.g * 255),
                                     (int)(nvgColor.b * 255));
            return c;
        }

        //public NVGcolor ColorToNVGcolor(Color c)
        //{
        //    var nvgc = new NVGcolor();
        //    nvgc.a = c.A / 255f;
        //    nvgc.r = c.R / 255f;
        //    nvgc.g = c.G / 255f;
        //    nvgc.b = c.B / 255f;

        //    return nvgc;
        //}

        void drawEditBoxBase(NVGcontext vg, float x, float y, float w, float h)
        {
            NVGpaint bg;
            // Edit
            bg = NanoVG.nvgBoxGradient(vg, x + 1, y + 1 + 1.5f, w - 2, h - 2, 3, 4, NanoVG.nvgRGBA(255, 255, 255, 32), NanoVG.nvgRGBA(32, 32, 32, 32));
            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRoundedRect(vg, x + 1, y + 1, w - 2, h - 2, 4 - 1);
            NanoVG.nvgFillPaint(vg, bg);
            NanoVG.nvgFill(vg);

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRoundedRect(vg, x + 0.5f, y + 0.5f, w - 1, h - 1, 4 - 0.5f);
            NanoVG.nvgStrokeColor(vg, NanoVG.nvgRGBA(0, 0, 0, 48));
            NanoVG.nvgStroke(vg);


        }

        public override void DrawEditBox(NanoFont font, string text, int x, int y, int w, int h, bool topWinLocation = false)
        {
            if (!NanoVG.FontCreated(vg, font.Id))
                throw new Exception(string.Format("Font '{0}', not created.", font.InternalName));

            if (w < 0 || h < 0)
                return;

            int nx = x + (topWinLocation ? topWinPos.X : 0);
            int ny = y + (topWinLocation ? topWinPos.Y : 0);

            if(topWinLocation)
                SetScissor();

            drawEditBoxBase(vg, nx, ny, w, h);

            NanoVG.nvgFontSize(vg, font.Height);
            NanoVG.nvgFontFace(vg, font.InternalName);
            NanoVG.nvgFillColor(vg, ColorToNVGcolor(Color.WhiteSmoke)); // NanoVG.RGBA(255, 255, 255, 64));
            NanoVG.nvgTextAlign(vg, (int)(EAlign.ALIGN_LEFT | EAlign.ALIGN_MIDDLE));
            NanoVG.nvgText(vg, nx + h * 0.3f, ny + h * 0.5f, text);
        }

        void drawSearchBoxBase(NVGcontext vg, float x, float y, float w, float h)
        {
            float cornerRadius = h / 2 - 1;

            NVGpaint bg = NanoVG.nvgBoxGradient(vg, x,
                h + 1.5f,
                w,
                h,
                h / 2,
                5,
                NanoVG.nvgRGBA(255, 255, 255, 64), NanoVG.nvgRGBA(255, 255, 255, 64));

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRoundedRect(vg, x, y, w, h, cornerRadius);
            NanoVG.nvgFillPaint(vg, bg);
            NanoVG.nvgFill(vg);

            /* Colors Missing
             SearchBoxGradientTopFocusedColor = NanoVG.nvgRGBA(255, 255, 255, 255),
            SearchBoxGradientBotFocusedColor = NanoVG.nvgRGBA(255, 255, 255, 255),
            SearchBoxGradientTopUnfocusedColor = NanoVG.nvgRGBA(30, 50, 71, 255),
            SearchBoxGradientBotUnfocusedColor = NanoVG.nvgRGBA(30, 50, 71, 255),
            SearchBoxGradientIconSearchFocusedColor = NanoVG.nvgRGBA(0, 0, 0, 255),
            SearchBoxGradientIconSearchUnfocusedColor = NanoVG.nvgRGBA(237, 237, 237, 255),
            SearchBoxGradientTextFocusedColor = NanoVG.nvgRGBA(0, 0, 0, 255),
            SearchBoxGradientTextUnfocusedColor = NanoVG.nvgRGBA(237, 237, 237, 255),
            SearchBoxGradientIconClearFocusedColor = NanoVG.nvgRGBA(0, 0, 0, 255),
            SearchBoxGradientIconClearUnfocusedColor = NanoVG.nvgRGBA(237, 237, 237, 255) 
             */


            #region Magnifying Glass Icon - TODO // Switch to DrawGlyph
            NanoVG.nvgFontSize(vg, h * 1.3f);
            NanoVG.nvgFontFace(vg, "icons");
            //NanoVG.nvgFillColor(vg, this.focused ? Theme.LightTheme.SearchBoxGradientIconSearchFocusedColor : Theme.LightTheme.SearchBoxGradientIconSearchUnfocusedColor);
            NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(0, 0, 0, 255)); // ThemeOld.LightThemeOld.SearchBoxGradientIconSearchFocusedColor);
            NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_CENTER | NVGalign.NVG_ALIGN_MIDDLE));
            string sts = UITool.cpToUTF8((int)Entypo.ICON_SEARCH);
            NanoVG.nvgText(vg, x + h * 0.55f, y + h * 0.55f, sts);
            #endregion

            #region Clear Icon  - TODO // Switch to DrawGlyph
            NanoVG.nvgFontSize(vg, h * 1.3f);
            NanoVG.nvgFontFace(vg, "icons");
            //NanoVG.nvgFillColor(vg, this.focused ? theme.searchBoxGradientIconClearFocusedColor : theme.searchBoxGradientIconClearUnfocusedColor);
            NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(0, 0, 0, 255));// ThemeOld.SearchBoxGradientIconSearchFocusedColor);
            NanoVG.nvgTextAlign(vg, (int)(NVGalign.NVG_ALIGN_CENTER | NVGalign.NVG_ALIGN_MIDDLE));
            NanoVG.nvgText(vg, x + w - h * 0.55f, y + h * 0.55f, NanoFont.UnicodeToUTF8((int)Entypo.ICON_CIRCLED_CROSS));
            #endregion
        }
        public override void DrawSearchBox(NanoFont font, string text, int x, int y, int w, int h)
        {
            if (!NanoVG.FontCreated(vg, font.Id))
                throw new Exception(string.Format("Font '{0}', not created.", font.InternalName));

            if (w < 0 || h < 0)
                return;

            int nx = x + topWinPos.X;
            int ny = y + topWinPos.Y;

            SetScissor();

            drawSearchBoxBase(vg, nx, ny, w, h);

            NanoVG.nvgFontSize(vg, font.Height);
            NanoVG.nvgFontFace(vg, font.InternalName);
            NanoVG.nvgFillColor(vg, ColorToNVGcolor(Color.WhiteSmoke)); // NanoVG.RGBA(255, 255, 255, 64));
            NanoVG.nvgTextAlign(vg, (int)(EAlign.ALIGN_LEFT | EAlign.ALIGN_MIDDLE));
            NanoVG.nvgText(vg, nx + h * 0.3f, ny + h * 0.5f, text);
        }
        #endregion Util

        public override void Clear()
        {
            Clear(new NanoSolidBrush(BGColor));//BGColor));
        }

        //public override void Clear()
        //{
        //    Clear2(new LinearGradientBrush(STEDRect, STColor, EDColor, 0.5f));
        //}


        public override void Clear(NanoSolidBrush sb)
        {
            if (owner.Parent == null && owner is NthDimension.Forms.Widget.WHUD)
            {
                //GL.ClearColor(sb.Color);
                //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            }
            else
                FillRectangle(sb, 0, 0, owner.Width, owner.Height);
        }

        //public override void Clear2(LinearGradientBrush lgb)
        //{
        //    if (owner.Parent == null && owner is WHUD)
        //    {

        //    }
        //    else
        //    {
        //        FillGradientRectangle(lgb, STEDRect);
        //    }
        //}

        //public override void FillGradientRectangle(LinearGradientBrush brush, Rectangle rect)
        //{
        //    base.FillGradientRectangle(brush, rect);
        //}


        public override float DrawString(string text, NanoSolidBrush sb, float x, float y)
        {
            return DrawString(text, owner.Font, sb, x, y);
        }
        public override float DrawGlyph(byte[] text, NanoSolidBrush sb, float x, float y)
        {
            return DrawGlyph(text, owner.FontGlyph, sb, x, y);
        }

        public override float DrawString(string text, NanoFont font, NanoSolidBrush sb, float x, float y)
        {
            if (!NanoVG.FontCreated(vg, font.Id))
                throw new Exception(string.Format("Font '{0}', not created.", font.InternalName));

            SetScissor();

            NanoVG.nvgFontSize(vg, font.Height);
            NanoVG.nvgFontFace(vg, font.InternalName);
            NanoVG.nvgFillColor(vg, ColorToNVGcolor(sb.Color));
            float xo = NanoVG.nvgText(vg, x + topWinPos.X, y + topWinPos.Y, text);
            return xo;
        }
        public override float DrawGlyph(byte[] text, NanoFont font, NanoSolidBrush sb, float x, float y)
        {
            if (!NanoVG.FontCreated(vg, font.Id))
                throw new Exception(string.Format("Font '{0}', not created.", font.InternalName));

            SetScissor();

            //NanoVG.nvgFontSize(vg, font.Height);
            NanoVG.nvgFontSize(vg, font.FontSize);
            NanoVG.nvgFontFace(vg, font.InternalName);
            NanoVG.nvgFillColor(vg, ColorToNVGcolor(sb.Color));
            float xo = NanoVG.nvgText(vg, x + topWinPos.X, y + topWinPos.Y, text);
            return xo;
        }


        public override float DrawString(string text, NanoFont font, NanoSolidBrush sb, int y, Rectangle rect)
        {
            if (!NanoVG.FontCreated(vg, font.Id))
                throw new Exception(string.Format("Font '{0}', not created.", font.InternalName));

            SetClip2(rect);

            NanoVG.nvgFontSize(vg, font.Height);
            NanoVG.nvgFontFace(vg, font.InternalName);
            NanoVG.nvgFillColor(vg, ColorToNVGcolor(sb.Color));
            float xo = NanoVG.nvgText(vg, rect.X + topWinPos.X, y + topWinPos.Y, text);
            return xo;
        }
        public override float DrawGlyph(byte[] text, NanoFont font, NanoSolidBrush sb, int y, Rectangle rect)
        {
            if (!NanoVG.FontCreated(vg, font.Id))
                throw new Exception(string.Format("Font '{0}', not created.", font.InternalName));

            SetClip2(rect);

            //NanoVG.nvgFontSize(vg, font.Height);
            NanoVG.nvgFontSize(vg, font.FontSize);
            NanoVG.nvgFontFace(vg, font.InternalName);
            NanoVG.nvgFillColor(vg, ColorToNVGcolor(sb.Color));
            float xo = NanoVG.nvgText(vg, rect.X + topWinPos.X, y + topWinPos.Y, text);
            return xo;
        }

        public override void DrawLine(NanoPen p, int x1, int y1, int x2, int y2)
        {
            SetScissor();

            NanoVG.nvgBeginPath(vg);
            float nx1 = x1;
            float nx2 = x2;
            float ny1 = y1;
            float ny2 = y2;

            if (/*p.Width == 1f && */y1 == y2)
            {
                // Horizontal
                ny1 += 0.5f;
                ny2 += 0.5f;
            }
            else if (/*p.Width == 1f && */x1 == x2)
            {
                // vertical
                nx1 += 0.5f;
                nx2 += 0.5f;
            }
            else if (Math.Abs(p.Width - 1f) < float.Epsilon)
            {
                nx1 += 0.5f;
                ny1 += 0.5f;
                nx2 -= 1f;
                ny2 -= 1f;
            }

            NanoVG.nvgMoveTo(vg, nx1 + topWinPos.X, ny1 + topWinPos.Y);
            NanoVG.nvgLineTo(vg, nx2 + topWinPos.X, ny2 + topWinPos.Y);

            NanoVG.nvgStrokeWidth(vg, p.Width);
            NanoVG.nvgStrokeColor(vg, ColorToNVGcolor(p.Color));
            NanoVG.nvgStroke(vg);
        }

        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="p">P.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width. Value must be greater than zero for display.</param>
        /// <param name="height">Height. Value must be greater than zero for display.</param>
        public override void DrawRectangle(NanoPen p, int x, int y, int width, int height)
        {
            if (width < 0 || height < 0)
                return;

            int nx = x + topWinPos.X;
            int ny = y + topWinPos.Y;

            SetScissor();

            NanoVG.nvgBeginPath(vg);

            if (Math.Abs(p.Width - 1f) < float.Epsilon)
                NanoVG.nvgRect(vg, nx + 0.5f, ny + 0.5f, width - 1f, height - 1f);
            else
                NanoVG.nvgRect(vg, nx, ny, width, height);

            NanoVG.nvgStrokeWidth(vg, p.Width);
            NanoVG.nvgStrokeColor(vg, ColorToNVGcolor(p.Color));
            NanoVG.nvgStroke(vg);
        }

        /// <summary>
        /// Draws the rounded rect.
        /// </summary>
        /// <param name="p">P.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width. Value must be greater than zero for display.</param>
        /// <param name="height">Height. Value must be greater than zero for display.</param>
        /// <param name="radius">Radius.</param>
        public override void DrawRoundedRect(NanoPen p, int x, int y, int width, int height, float radius)
        {
            if (width < 0 || height < 0)
                return;

            SetScissor();

            NanoVG.nvgBeginPath(vg);

            if (Math.Abs(p.Width - 1f) < float.Epsilon)
                NanoVG.nvgRoundedRect(vg, x + 0.5f + topWinPos.X, y + 0.5f + topWinPos.Y, width - 1f, height - 1f, radius);
            else
                NanoVG.nvgRoundedRect(vg, x + topWinPos.X, y + topWinPos.Y, width, height, radius);

            NanoVG.nvgStrokeWidth(vg, p.Width);
            NanoVG.nvgStrokeColor(vg, ColorToNVGcolor(p.Color));
            NanoVG.nvgStroke(vg);
        }

        public override void DrawImage(Image img, int x, int y, int width, int height)
        {
            NVGpaint imgPaint;
            int iImg;

            int nx = x + topWinPos.X;
            int ny = y + topWinPos.Y;

            SetScissor(new Rectangle(nx, ny, width, height));

            if (img.Tag == null)
            {
                iImg = NanoVG.CreateImage(vg, (Bitmap)img, 0);
                if (iImg < 0)
                    return;
                img.Tag = iImg;
            }
            else
                iImg = (int)img.Tag;

            NanoVG.nvgSave(vg);
            {
                imgPaint = NanoVG.nvgImagePattern(vg, nx, ny, width, height, iImg);
                NanoVG.nvgBeginPath(vg);
                NanoVG.nvgRect(vg, nx, ny, width, height);
                NanoVG.nvgFillPaint(vg, imgPaint);
                NanoVG.nvgFill(vg);
            }
            NanoVG.nvgRestore(vg);
        }
        public override void DrawImage(int imgIndex, int x, int y, int width, int height)
        {
            NVGpaint imgPaint;

            int nx = x + topWinPos.X;
            int ny = y + topWinPos.Y;

            //SetScissor(new Rectangle(nx, ny, width, height));  // DO NOT EXECUTE - OTHERWISE SCREWS SCRISSORING

            NanoVG.nvgSave(vg);
            {
                imgPaint = NanoVG.nvgImagePattern(vg, nx, ny, width, height, imgIndex);
                NanoVG.nvgBeginPath(vg);
                NanoVG.nvgRect(vg, nx, ny, width, height);
                NanoVG.nvgFillPaint(vg, imgPaint);
                NanoVG.nvgFill(vg);
            }
            NanoVG.nvgRestore(vg);
        }
        public override void DrawImageScissored(int imgIndex, int x, int y, int width, int height)
        {
            NVGpaint imgPaint;

            int nx = x + topWinPos.X;
            int ny = y + topWinPos.Y;

            SetScissor(new Rectangle(nx, ny, width, height));  // DO NOT EXECUTE - OTHERWISE SCREWS SCRISSORING

            NanoVG.nvgSave(vg);
            {
                imgPaint = NanoVG.nvgImagePattern(vg, nx, ny, width, height, imgIndex);
                NanoVG.nvgBeginPath(vg);
                NanoVG.nvgRect(vg, nx, ny, width, height);
                NanoVG.nvgFillPaint(vg, imgPaint);
                NanoVG.nvgFill(vg);
            }
            NanoVG.nvgRestore(vg);
        }

        public override void DrawImageRounded(int imgIndex, int x, int y, int width, int height, float radius)
        {
            NVGpaint imgPaint;

            int nx = x + topWinPos.X;
            int ny = y + topWinPos.Y;

            //SetScissor(new Rectangle(nx, ny, width, height));

            NanoVG.nvgSave(vg);
            {
                imgPaint = NanoVG.nvgImagePattern(vg, nx, ny, width, height, imgIndex);
                NanoVG.nvgBeginPath(vg);
                NanoVG.nvgRoundedRect(vg, nx, ny, width, height, radius);
                NanoVG.nvgFillPaint(vg, imgPaint);
                NanoVG.nvgFill(vg);
            }
            NanoVG.nvgRestore(vg);
        }
        public override void DrawImageRoundedScissored(int imgIndex, int x, int y, int width, int height, float radius)
        {
            NVGpaint imgPaint;

            int nx = x + topWinPos.X;
            int ny = y + topWinPos.Y;

            SetScissor(new Rectangle(nx, ny, width, height));

            NanoVG.nvgSave(vg);
            {
                imgPaint = NanoVG.nvgImagePattern(vg, nx, ny, width, height, imgIndex);
                NanoVG.nvgBeginPath(vg);
                NanoVG.nvgRoundedRect(vg, nx, ny, width, height, radius);
                NanoVG.nvgFillPaint(vg, imgPaint);
                NanoVG.nvgFill(vg);
            }
            NanoVG.nvgRestore(vg);
        }
        public override void DrawImageCircle(Image img, int x, int y, int width, int height, int radius)
        {
            NVGpaint imgPaint;
            int iImg;

            int nx = x + topWinPos.X;
            int ny = y + topWinPos.Y;

            //SetScissor(new Rectangle(nx, ny, width, height));

            if (img.Tag == null)
            {
                iImg = NanoVG.CreateImage(vg, (Bitmap)img, 0);
                if (iImg < 0)
                    return;
                img.Tag = iImg;
            }
            else
                iImg = (int)img.Tag;

            NanoVG.nvgSave(vg);
            {
                imgPaint = NanoVG.nvgImagePattern(vg, nx, ny, width, height, iImg);
                NanoVG.nvgBeginPath(vg);
                NanoVG.nvgCircle(vg, nx, ny, radius);
                NanoVG.nvgFillPaint(vg, imgPaint);
                NanoVG.nvgFill(vg);
            }
            NanoVG.nvgRestore(vg);
        }
        public override void DrawImageCircle(int imgIndex, int x, int y, int width, int height, int radius)
        {




            //int nx = x + topWinPos.X;
            //int ny = y + topWinPos.Y;

            int nx = x;
            int ny = y;

            //SetScissor(new Rectangle(nx, ny, width, height));
            //NanoVG.nvgResetScissor(vg);
            NanoVG.nvgSave(vg);
            {
                NanoVG.nvgTranslate(vg, topWinPos.X, topWinPos.Y);

                //SetScissor(new Rectangle(nx, ny, width, height));

                // imgPaint = NanoVG.nvgImagePattern(vg, nx, ny, width, height, imgIndex);
                NVGpaint imgPaint = NanoVG.nvgImagePattern(vg, nx - width / 2, ny - height / 2, width, height, imgIndex);
                NanoVG.nvgBeginPath(vg);
                //NanoVG.nvgCircle(vg, nx - radius, ny - radius, radius);
                NanoVG.nvgCircle(vg, nx, ny, radius);
                NanoVG.nvgFillPaint(vg, imgPaint);
                NanoVG.nvgFill(vg);
                NanoVG.nvgTranslate(vg, -topWinPos.X, -topWinPos.Y);
            }
            NanoVG.nvgRestore(vg);
        }
        public override void DrawImageCircleScissored(int imgIndex, int x, int y, int width, int height, int radius)
        {
            int nx = x + topWinPos.X;
            int ny = y + topWinPos.Y;

            SetScissor(new Rectangle(nx, ny, width, height));

            NanoVG.nvgSave(vg);
            {
                NanoVG.nvgTranslate(vg, topWinPos.X, topWinPos.Y);

                //SetScissor(new Rectangle(nx, ny, width, height));

                NVGpaint imgPaint = NanoVG.nvgImagePattern(vg, nx, ny, width, height, imgIndex);

                NanoVG.nvgBeginPath(vg);
                //NanoVG.nvgCircle(vg, nx - radius, ny - radius, radius);
                NanoVG.nvgCircle(vg, nx, ny, radius);
                NanoVG.nvgFillPaint(vg, imgPaint);
                NanoVG.nvgFill(vg);
                NanoVG.nvgTranslate(vg, -topWinPos.X, -topWinPos.Y);
            }
            NanoVG.nvgRestore(vg);
        }

        public override void FillBorderShadow(int x, int y, int width, int height, float cornerRadius = 0f, NanoSolidBrush sb1 = null, NanoSolidBrush sb2 = null)
        {
            int nx = x + topWinPos.X;
            int ny = y + topWinPos.Y;

            NVGpaint shadowPaint;

            // Es necesario establecer el 'scissor', de no hacerlo, el actual 'scissor' sería
            // el establecido anteriormente. Por esto en todos los métodos se llama a 'SetScissor()'.
            SetScissor(new Rectangle(nx - 5, ny - 5, width + 15, height + 15));

            // Drop shadow
            shadowPaint = NanoVG.nvgBoxGradient(vg, nx, ny, width, height, cornerRadius * 2, 10f,
                                                NanoVG.nvgRGBA(0, 0, 0, 64), NanoVG.nvgRGBA(0, 0, 0, 0));
            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRect(vg, nx - 5, ny - 5, width + 15, height + 15);
            NanoVG.nvgRoundedRect(vg, nx, ny, width, height, cornerRadius);
            NanoVG.nvgPathWinding(vg, (int)ESolidity.HOLE);
            NanoVG.nvgFillPaint(vg, shadowPaint);
            NanoVG.nvgFill(vg);
        }

        /// <summary>
        /// Fills the rectangle.
        /// </summary>
        /// <param name="sb">Sb.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width. Value must be greater than zero for display.</param>
        /// <param name="height">Height. Value must be greater than zero for display.</param>
        public override void FillRectangle(NanoSolidBrush sb, int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0)
                return;

            int nx = x + topWinPos.X;
            int ny = y + topWinPos.Y;

            SetScissor();



            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRect(vg, nx, ny, width, height);
            NanoVG.nvgFillColor(vg, ColorToNVGcolor(sb.Color));
            NanoVG.nvgFill(vg);
        }

        /// <summary>
        /// Fills the rounded rect.
        /// </summary>
        /// <param name="sb">Sb.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width. Value must be greater than zero for display.</param>
        /// <param name="height">Height. Value must be greater than zero for display.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="smoothing">If set to <c>true</c> smoothing.</param>
        public override void FillRoundedRect(NanoSolidBrush sb, int x, int y, int width, int height, float radius, bool smoothing = true)
        {
            if (width <= 0 || height <= 0)
                return;

            SetScissor();

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRoundedRect(vg, x + topWinPos.X, y + topWinPos.Y, width, height, radius);
            NanoVG.nvgFillColor(vg, ColorToNVGcolor(sb.Color));
            NanoVG.nvgFill(vg);
        }

        public override void FillEllipse(NanoSolidBrush sb, Rectangle rc)
        {
            if (rc.Width <= 0 || rc.Height <= 0)
                return;

            float rx = rc.Width / 2f;
            float ry = rc.Height / 2f;
            float px = rc.X + rx;
            float py = rc.Y + rx;

            SetScissor();

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgEllipse(vg, px + topWinPos.X, py + topWinPos.Y, rx, ry);
            NanoVG.nvgFillColor(vg, ColorToNVGcolor(sb.Color));
            NanoVG.nvgFill(vg);
        }

        public override void FillPolygon(NanoSolidBrush sb, Point[] points)
        {
            SetScissor();

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgMoveTo(vg, points[0].X + topWinPos.X, points[0].Y + topWinPos.Y);
            for (int cont = 1; cont < points.Length; cont++)
            {
                NanoVG.nvgLineTo(vg, points[cont].X + topWinPos.X, points[cont].Y + topWinPos.Y);
            }
            NanoVG.nvgClosePath(vg);
            NanoVG.nvgFillColor(vg, ColorToNVGcolor(sb.Color));
            NanoVG.nvgFill(vg);
        }

        public override void FillPolygon(NanoSolidBrush sb, PointF[] points)
        {
            SetScissor();

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgMoveTo(vg, points[0].X + topWinPos.X, points[0].Y + topWinPos.Y);
            for (int cont = 1; cont < points.Length; cont++)
            {
                NanoVG.nvgLineTo(vg, points[cont].X + topWinPos.X, points[cont].Y + topWinPos.Y);
            }
            NanoVG.nvgClosePath(vg);
            NanoVG.nvgFillColor(vg, ColorToNVGcolor(sb.Color));
            NanoVG.nvgFill(vg);
        }

        #region Helper-Paint-Widgets

        /// <summary>
        /// Returns 1 if col.rgba is (0.0f,0.0f,0.0f,0.0f), 0 otherwise
        /// </summary>
        /// <returns><c>true</c>, if black was ised, <c>false</c> otherwise.</returns>
        /// <param name="col">Col.</param>
        bool isBlack(NVGcolor col)
        {
            if (Math.Abs(col.r) < float.Epsilon
                && Math.Abs(col.g) < float.Epsilon
                && Math.Abs(col.b) < float.Epsilon) // && col.a == 0.0f)
            {
                return true;
            }
            return false;
        }

        byte ocolAlpha1;
        byte ocolAlpha2;
        byte borderAlpha;

        void drawButton(NVGcontext nvg, string text, NanoFont font, Color c, float x, float y,
                        float w, float h, float cornerRadius = 4f, bool clicked = false)
        {
            ocolAlpha1 = 80;
            ocolAlpha2 = 140;
            borderAlpha = 48;

            if (clicked)
            {
                ocolAlpha1 = 120;
                ocolAlpha2 = 200;
                borderAlpha = 200;
            }

            NVGpaint bg;
            float tw = 0, iw = 0;
            NVGcolor col = ColorToNVGcolor(c);

            NVGcolor icol = NanoVG.nvgRGBA(255, 255, 255, (byte)(isBlack(col) ? 16 : 32));
            NVGcolor ocol = NanoVG.nvgRGBA(0, 0, 0, (byte)(isBlack(col) ? ocolAlpha1 : ocolAlpha2));

            bg = NanoVG.nvgLinearGradient(nvg, x, y, x, y + h, icol, ocol);
            NanoVG.nvgBeginPath(nvg);
            NanoVG.nvgRoundedRect(nvg, x + 1, y + 1, w - 2, h - 2, cornerRadius - 1);

            if (!isBlack(col))
            {
                NanoVG.nvgFillColor(nvg, col);
                NanoVG.nvgFill(nvg);
            }
            NanoVG.nvgFillPaint(nvg, bg);
            NanoVG.nvgFill(nvg);

            // Representa el trazo que delimita al botón
            NanoVG.nvgBeginPath(nvg);
            NanoVG.nvgRoundedRect(nvg, x + 0.5f, y + 0.5f, w - 1, h - 1, cornerRadius - 0.5f);
            NanoVG.nvgStrokeColor(nvg, NanoVG.nvgRGBA(0, 0, 0, borderAlpha));
            NanoVG.nvgStroke(nvg);

            // Solo es el texto
            NanoVG.nvgFontSize(nvg, font.Height);
            NanoVG.nvgFontFace(nvg, font.InternalName);






            // 'NanoVG.TextBounds()' requiere un 'NanoVG.StateSave()' previo.
            tw = NanoVG.nvgTextBounds(nvg, 0, 0, text);

            NanoVG.nvgTextAlign(nvg, (int)(EAlign.ALIGN_LEFT | EAlign.ALIGN_MIDDLE));
            NanoVG.nvgFillColor(nvg, NanoVG.nvgRGBA(0, 0, 0, 160));
            NanoVG.nvgText(nvg, x + w * 0.5f - tw * 0.5f + iw * 0.25f, y + h * 0.5f - 1, text);
            NanoVG.nvgFillColor(nvg, NanoVG.nvgRGBA(255, 255, 255, 160));
            NanoVG.nvgText(nvg, x + w * 0.5f - tw * 0.5f + iw * 0.25f, y + h * 0.5f, text);
        }
        #endregion Helper-Paint-Widgets

        public override void PaintButton(string text, NanoFont font, Color c, int x, int y, int width, int height,
                                         float radius = 4f, bool clicked = false)
        {
            if (text == null)
                return;

            int nx = x + topWinPos.X;
            int ny = y + topWinPos.Y;

            NanoVG.nvgSave(vg);

            SetScissor();

            drawButton(vg, text, font, c, nx, ny, width, height, radius, clicked);

            NanoVG.nvgRestore(vg);
        }

        public override void PaintLinearGradientRect(NanoSolidBrush beginColor, NanoSolidBrush endColor, Rectangle rect)
        {
            int nx = rect.X + topWinPos.X;
            int ny = rect.Y + topWinPos.Y;

            NVGpaint lgr = NanoVG.nvgLinearGradient(vg, nx, ny, nx, ny + rect.Height,
                                                    ColorToNVGcolor(beginColor.Color),
                                                    ColorToNVGcolor(endColor.Color));
            SetScissor();

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRect(vg, nx, ny, rect.Width, rect.Height);
            NanoVG.nvgFillPaint(vg, lgr);
            NanoVG.nvgFill(vg);
        }

        /// <summary>
        /// Sets the Scissor. Solo se dibujan las operaciones de dibujo internas al área de 'scissor'.
        /// <remarks>
        /// El rectangulo de 'scissor' es una operación global, es decir, no se almacena un 'scissor' por cada NVGstate,
        /// un 'GContext' en una operación anterior puede haber cambiado el 'scissor' luego antes de cualquier operación
        /// hay que establecer el 'scissor' del 'GContex' actual.
        /// </remarks>
        /// </summary>
        public override void SetScissor()
        {
            NanoVG.nvgScissor(vg, topWinClipRect.X, topWinClipRect.Y, topWinClipRect.Width, topWinClipRect.Height);
        }

        public override void SetScissor(Rectangle rect)
        {
            // TODO Solo funciona si las coordenadas de 'rect' están en coordenadas de 'WHUD'.
            NanoVG.nvgScissor(vg, rect.X, rect.Y,
                              rect.Width,
                              rect.Height);
        }

        /// <summary>
        /// Establece un área de recorte en el Widget no permanente.
        /// </summary>
        /// <param name="rect">Rectángulo de recorte en coordenadas de Widget</param>
        protected override void SetClip2(Rectangle rect)
        {
            Point p = owner.LocalToWindow();
            var clip = new Rectangle(p.X + rect.X, p.Y + rect.Y, rect.Width, rect.Height);
            var resultClip = new Rectangle(topWinClipRect.Location, topWinClipRect.Size);
            resultClip.Intersect(clip);
            NanoVG.nvgScissor(vg, resultClip.X, resultClip.Y, resultClip.Width, resultClip.Height);
        }
        /// <summary>
        /// Establece un área de recorte en el Widget permanente.
        /// </summary>
        /// <param name="rect">Rectángulo de recorte en coordenadas de Widget</param>
        public override void SetClip(Rectangle rect)
        {
            Point p = owner.LocalToWindow();
            var clip = new Rectangle(p.X + rect.X, p.Y + rect.Y, rect.Width, rect.Height);
            topWinClipRect.Intersect(clip);
            NanoVG.nvgScissor(vg, topWinClipRect.X, topWinClipRect.Y, topWinClipRect.Width, topWinClipRect.Height);
        }
    }

    public class ThemeBase
    {
        //public static ThemeBase Theme = new ThemeBase.LightTheme();
        //public static ThemeBase Theme = new ThemeLight();
        //public static ThemeBase Theme = new ThemeDark();
        public static ThemeBase Theme = new ThemeDefault();

        public NanoFont ThemeFont { get; set; }
        public NanoFont ThemeFontSmall { get; set; }
        public NanoFont ThemeFontMedium { get; set; }
        public NanoFont ThemeFontBig { get; set; }
        public NanoFont ThemeFontSecondary { get; set; }
        public Size ScrollBarSize { get; set; }
        public Color GeneralTextColor { get; set; }
        public Color SecondaryTextColor { get; set; }
        public Color ThirdTextColor { get; set; }

        #region TopBarColor Properties
        public Color TopBarBGColorStart { get; set; }
        public Color TopBarBGColorEnd { get; set; }
        public NVGcolor TopBarStartColor { get; set; }
        public NVGcolor TopBarEndColor { get; set; }
        ////
        public Color SearchBoxBGColor { get; set; }



        //public Color SearchBoxTextColor { get; set; }
        #endregion

        #region TopBarIconsColors
        public Color TopBarIconsHoverColor { get; set; }
        public Color TopBarIconsDownColor { get; set; }
        public Color TopBarIconsShadowColor { get; set; }
        public Color TopBarIconsGlyphCircleColor { get; set; }
        public Color TopBarIconsGlyphCircleTextColor { get; set; }
        public NVGcolor TopBarSearchBoxIconsColor { get; set; }
        public NVGcolor TopBarSearchBoxNormalColor { get; set; }
        public NVGcolor TopBarSearchBoxClickedColor { get; set; }
        public NVGcolor TopBarSearchBoxTextNormalColor { get; set; }
        public NVGcolor TopBarSearchBoxTextClickedColor { get; set; }
        public NVGcolor TopBarSearcBoxFocusedColor { get; set; }
        public NVGcolor TopBarSearchBoxUnfocusedColor { get; set; }

        #endregion

        #region OTHER PROPERTIES
        //public NVGcolor SearchIconColor { get; set; }
        //public NVColor ToolButtonAlertNormalColor { get; set; }
        //public Color ToolButtonAlertNormalColor { get; set; }
        //public Color ToolButtonAlertHoverColor { get; set; }
        //public Color ToolButtonAlertClickedColor { get; set; }

        ////public NVColor ToolButtonMessagesNormalColor { get; set; }
        //public Color ToolButtonMessagesNormalColor { get; set; }
        //public Color ToolButtonMessagesHoverColor { get; set; }
        //public Color ToolButtonMessagesClickedColor { get; set; }

        ////public NVColor ToolButtonSettingsNormalColor { get; set; }
        //public Color ToolButtonSettingsNormalColor { get; set; }
        //public Color ToolButtonSettingsHoverColor { get; set; }
        //public Color ToolButtonSettingsClickedColor { get; set; }

        ////public NVColor ToolButtonHelpNormalColor { get; set; }
        //public Color ToolButtonHelpNormalColor { get; set; }
        //public Color ToolButtonHelpHoverColor { get; set; }
        //public Color ToolButtonHelpClickedColor { get; set; }

        //TO:DO:Create List Box Colors
        #endregion

        #region NavBarColor Properties
        public Color NavBarBGColorStart { get; set; }
        public Color NavBarBGColorEnd { get; set; }
        public Color NavButtonHoverColor { get; set; }
        public Color NavButtonDownColor { get; set; }
        public Color NavButtonNormalColor { get; set; }
        public Color NavButtonTextNormalColor { get; set; }
        public Color NavButtonTextHoverColor { get; set; }
        public Color NavButtonTextClickedColor { get; set; }

        #endregion

        #region NavWindowsColors
        public Color WindowPhotosBGColor { get; set; }
        public Color WindowCitiesBGColor { get; set; }
        public Color WindowInventoryBGColor { get; set; }
        #endregion


        #region WindowFriendsColors
        public Color WindowFriendsBGColor { get; set; }
        public Color WindowFriendsCanvasBGColor { get; set; }
        public Color WindowFriendsListBGColor { get; set; }
        public Color WindowFriendsFriendItemBGColor { get; set; }
        public Color WindowFriendsContainerBGColor { get; set; }
        public Color WindowFriendsButtonFriendsBGColor { get; set; }
        public Color WindowFriendsButtonFriendsFGColor { get; set; }
        public Color WindowFriendsButtonMessageBGColor { get; set; }
        public Color WindowFriendsButtonMessagesFGColor { get; set; }
        public Color WindowFriendsButtonInviteBGColor { get; set; }
        public Color WindowFriendsButtonInviteFGColor { get; set; }
        #endregion

        #region LinkProfile Properties
        public Color WindowProfileBGColor { get; set; }
        public Color WindowProfileCanvasBGColor { get; set; }
        #endregion

        #region WindowDialoguesColors
        public Color WindowDialoguesBGColor { get; set; }
        public Color WindowDialoguesChatUsersBGColor { get; set; }
        public Color WindowDialoguesChatUsersDetailsBGColor { get; set; }
        public Color WindowDialoguesChatBGColor { get; set; }
        public Color WindowDialoguesChatUsersListBGColor { get; set; }
        public Color WindowDialoguesChatUsersListNormalColor { get; set; }
        public Color WindowDialoguesChatUsersListHoverColor { get; set; }
        public Color WindowDialoguesChatUsersListDownColor { get; set; }
        public Color WindowDialoguesCanvasBGColor { get; set; }
        public Color WindowDialoguesFontColor { get; set; }
        public Color WindowDialoguesChatInputBoxBGColor { get; set; }
        public NVGcolor WindowDialoguesChatInputBoxColor { get; set; }
        public NVGcolor WindowDialoguesChatBoxIconsColor { get; set; }
        public Color WindowDialoguesChatBubbleMyselfColor { get; set; }
        public Color WindowDialoguesChatBubbleOtherUserColor { get; set; }



        #endregion

        #region SceneListDropDown
        public Size DropDownButtonSize { get; set; }
        public Color DropDownBackColor { get; set; }
        public Color DropDownBackColorHover { get; set; }
        public Color DropDownBackColorClick { get; set; }
        public Color DropDownForeColor { get; set; }
        public Color DropDownForeColorHover { get; set; }
        public Color DropDownForeColorClick { get; set; }

        #endregion SceneListDropDown


        #region ScrollBarColors
        public Color ScrollBarColor { get; set; }
        #endregion




        public class Margin
        {
            public float Top;
            public float Left;
            public float Bottom;
            public float Right;

            public Margin(float top, float left, float bottom, float right)
            {
                this.Top = top;
                this.Left = left;
                this.Bottom = bottom;
                this.Right = right;
            }
        }
    }

    public partial class ThemeOld
    {

        #region General Properties
        private ThemeOld(string name)
        {
            this.Name = name;
        }

        public static implicit operator bool(ThemeOld obj)
        {
            return (null != obj);
        }

        public static ThemeOld FromFile(string filename)
        {
            ThemeOld ret = new ThemeOld(filename);
            // TODO:

            throw new System.NotImplementedException();

            return ret;
        }
        #endregion


        // General Information
        //
        // -Padding is white space immediately surrounding an element or another object
        // -Margin is a white space around an element or another object on a surface

        #region Margin class
        public class Margin
        {
            public float Top;
            public float Left;
            public float Bottom;
            public float Right;

            public Margin(float top, float left, float bottom, float right)
            {
                this.Top = top;
                this.Left = left;
                this.Bottom = bottom;
                this.Right = right;
            }
        }
        #endregion

        #region Properties (Generic)
        public string Name { get; private set; }
        public NVGcolor DropShadowColor { get; private set; }
        public NVGcolor TransparentColor { get; private set; }
        public NVGcolor BorderDarkColor { get; private set; }
        public NVGcolor BorderLightColor { get; private set; }
        public NVGcolor BorderMediumColor { get; private set; }
        public NVGcolor TextColor { get; private set; }
        public NVGcolor TextDisabledColor { get; private set; }
        public NVGcolor TextShadowColor { get; private set; }
        public NVGcolor IconColor { get; private set; }
        #endregion Properties (Generic)

        #region Properties (Font)
        public string FontNormal { get; private set; }
        public string FontBold { get; private set; }
        public string FontIcons { get; private set; }
        public string FontEmoji { get; private set; }

        public int FontSize_Standard { get; private set; }
        public int FontSize_Button { get; private set; }
        public int FontSize_TextBox { get; private set; }
        #endregion  Properties (Font)

        #region Properties (Button)
        public int ButtonCornerRadius { get; private set; }
        public NVGcolor ButtonGradientTopFocusedColor { get; private set; }
        public NVGcolor ButtonGradientBotFocusedColor { get; private set; }
        public NVGcolor ButtonGradientTopUnfocusedColor { get; private set; }
        public NVGcolor ButtonGradientBotUnfocusedColor { get; private set; }
        public NVGcolor ButtonGradientTopPushedColor { get; private set; }
        public NVGcolor ButtonGradientBotPushedColor { get; private set; }
        #endregion

        #region TextLine
        public int Data_TextLineBorderTextSpacing { get; private set; }
        #endregion

        #region Properties (ChatBox)
        public NVGcolor ChatTextFocused { get; private set; }                               // TRANSFER PENDING
        public NVGcolor ChatTextUnfocused { get; private set; }                             // TRANSFER PENDING
        #endregion

        #region Properties (Scrollbar)
        public float ScrollbarCornerRadius { get; private set; }
        public float ScrollbarWidth { get; private set; }
        public float ScrollbarMinGripSize { get; private set; }
        public Margin ScrollbarMargin { get; private set; }
        public NVGcolor ScrollbarTrackTop { get; private set; }
        public NVGcolor ScrollbarTrackBottom { get; private set; }
        public NVGcolor ScrollbarGripTop { get; private set; }
        public NVGcolor ScrollbarGripBottom { get; private set; }
        public float Data_ScrollbarMinDragbarSize { get; private set; }
        public float Data_ScrollbarButtonSize { get; private set; }
        #endregion

        #region Properties (SearchBox)
        public NVGcolor SearchBoxGradientTopFocusedColor { get; private set; }
        public NVGcolor SearchBoxGradientBotFocusedColor { get; private set; }
        public NVGcolor SearchBoxGradientTopUnfocusedColor { get; private set; }
        public NVGcolor SearchBoxGradientBotUnfocusedColor { get; private set; }
        public NVGcolor SearchBoxGradientTextFocusedColor { get; private set; }
        public NVGcolor SearchBoxGradientTextUnfocusedColor { get; private set; }
        public NVGcolor SearchBoxGradientIconSearchFocusedColor { get; private set; }
        public NVGcolor SearchBoxGradientIconSearchUnfocusedColor { get; private set; }
        public NVGcolor SearchBoxGradientIconClearFocusedColor { get; private set; }
        public NVGcolor SearchBoxGradientIconClearUnfocusedColor { get; private set; }
        #endregion

        #region Properties (Tab)
        public float TabBorderWidth { get; private set; }
        public int TabInnerMargin { get; private set; }
        public int TabMinButtonWidth { get; private set; }
        public int TabMaxButtonWidth { get; private set; }
        public int TabControlWidth { get; private set; }
        public int TabButtonHorizontalPadding { get; private set; }
        public int TabButtonVerticalPadding { get; private set; }
        #endregion Properties (Tab)

        #region Properties (Window)
        public int WindowCornerRadius { get; private set; }
        public int WindowHeaderHeight { get; private set; }
        public int WindowDropShadowSize { get; private set; }

        public NVGcolor WindowColor { get; private set; }
        public NVGcolor WindowFocusedColor { get; private set; }
        public NVGcolor WindowHoverColor { get; private set; }
        public NVGcolor WindowHoverFocusedColor { get; private set; }

        public NVGcolor WindowTitleColor { get; private set; }
        public NVGcolor WindowTitleFocusedColor { get; private set; }
        public NVGcolor WindowTitleHoverColor { get; private set; }
        public NVGcolor WindowTitleHoverFocusedColor { get; private set; }

        public NVGcolor WindowHeaderGradientTopColor { get; private set; }
        public NVGcolor WindowHeaderGradientBotColor { get; private set; }
        public NVGcolor WindowHeaderSepTopColor { get; private set; }
        public NVGcolor WindowHeaderSepBotColor { get; private set; }

        public int Data_WindowFocusFx { get; private set; }
        public int Data_WindowHoverFx { get; private set; }

        public int Data_WindowResizeToleranceEdge { get; private set; }
        public int Data_WindowResizeToleranceBorder { get; private set; }

        public int Data_WindowChildrenClrMod { get; private set; }

        #endregion Properties (Window)

        #region Properties (Window Popup)
        public NVGcolor WindowPopupColor { get; private set; }
        public NVGcolor WindowPopupTransparentColor { get; private set; }
        #endregion Properties (Window Popup)

        #region Properties ChatBox
        public NVGcolor ChatUsers_BackgroundColor { get; set; }
        public NVGcolor ChatUsers_BackgroundColorPushed { get; set; }
        public NVGcolor ChatUserItem_TextFocusedColor { get; set; }
        public NVGcolor ChatUserItem_TextUnfocusedColor { get; set; }

        #endregion


    }

    public class ThemeDefault : ThemeBase
    {
        private ColorStartEnd color = new ColorStartEnd(Color.FromArgb(255, 80, 114, 145), Color.Empty);

        public ThemeDefault()
        {
            ThemeFont = NanoFont.DefaultRegular;
            ThemeFontSmall = new NanoFont(NanoFont.DefaultRegular, 12f);
            ThemeFontMedium = new NanoFont(NanoFont.DefaultRegular, 16f);
            ThemeFontBig = new NanoFont(NanoFont.DefaultRegular, 18f);
            ThemeFontSecondary = NanoFont.DefaultRegular;
            GeneralTextColor = Color.FromArgb(255, 80, 114, 145);
            SecondaryTextColor = Color.FromArgb(255, 97, 103, 112);
            ThirdTextColor = Color.FromArgb(255, 217, 219, 221);
            ScrollBarSize = new Size(13, 13);

            #region TopBarColors

            //TopBarBGColorStart = color.StartColor;
            TopBarBGColorStart = Color.FromArgb(255, 80, 114, 145);
            TopBarBGColorEnd = Color.FromArgb(255, 255, 0, 0);
            ////
            TopBarStartColor = new Color4f(80, 114, 145, 255).ToNVGColor();
            TopBarEndColor = new Color4f(48, 80, 113, 255).ToNVGColor();
            ////
            SearchBoxBGColor = Color.FromArgb(255, 94, 94, 94);

            #endregion

            #region WindowProfileColors
            WindowProfileBGColor = Color.FromArgb(255, 255, 255, 255);//Color.White;//Color.YellowGreen;
            WindowProfileCanvasBGColor = Color.FromArgb(0, Color.Gray);
            #endregion

            #region NavBarColors

            NavBarBGColorStart = Color.FromArgb(255, 80, 114, 145);
            NavButtonHoverColor = Color.FromArgb(255, 48, 80, 113);
            NavButtonDownColor = Color.FromArgb(255, 255, 255, 255); //Color.FromArgb(255, 36, 42, 49);
            NavButtonNormalColor = Color.FromArgb(0, 0, 0, 0);
            NavButtonTextNormalColor = Color.FromArgb(255, 255, 255, 255);
            NavButtonTextHoverColor = Color.FromArgb(255, 255, 255, 255);
            NavButtonTextClickedColor = Color.FromArgb(255, 80, 114, 145);

            #endregion

            #region WindowCitiesColors

            WindowCitiesBGColor = Color.FromArgb(255, 255, 255, 255);
            WindowPhotosBGColor = Color.FromArgb(255, 255, 255, 255);
            WindowInventoryBGColor = Color.FromArgb(255, 255, 255, 255);

            #endregion

            #region WindowFriendsColors

            WindowFriendsBGColor = Color.FromArgb(255, 255, 255, 255);
            WindowFriendsCanvasBGColor = Color.FromArgb(0, Color.Gray);
            WindowFriendsListBGColor = Color.FromArgb(30, Color.Gray);
            WindowFriendsFriendItemBGColor = Color.White;
            WindowFriendsButtonFriendsFGColor = Color.FromArgb(255, 80, 114, 145);
            WindowFriendsButtonMessagesFGColor = Color.FromArgb(255, 80, 114, 145);
            WindowFriendsButtonInviteFGColor = Color.FromArgb(255, 80, 114, 145);

            #endregion

            #region WindowDialoguesColors

            WindowDialoguesBGColor = Color.FromArgb(255, 255, 255, 255);
            WindowDialoguesChatUsersBGColor = Color.FromArgb(255, 255, 255, 255); //Color.FromArgb(50, 255, 255, 255);
            WindowDialoguesChatUsersListBGColor = Color.FromArgb(255, 240, 240, 240); //Color.FromArgb(30, Color.Gray);
            WindowDialoguesChatUsersDetailsBGColor = Color.FromArgb(255, 240, 240, 240); //Color.FromArgb(50, 255, 255, 255);
            WindowDialoguesChatBGColor = Color.FromArgb(255, 240, 240, 240); //Color.FromArgb(150, 255, 255, 255);
            WindowDialoguesCanvasBGColor = Color.FromArgb(30, Color.Gray);
            WindowDialoguesFontColor = Color.FromArgb(255, 80, 114, 145);
            WindowDialoguesChatInputBoxColor = new Color4f(255, 255, 255, 255).ToNVGColor();
            WindowDialoguesChatInputBoxBGColor = Color.FromArgb(255, 240, 240, 240);
            WindowDialoguesChatBoxIconsColor = new Color4f(80, 114, 145, 255).ToNVGColor();
            WindowDialoguesChatBubbleMyselfColor = Color.FromArgb(255, 255, 255, 255);
            WindowDialoguesChatBubbleOtherUserColor = Color.FromArgb(255, 255, 255, 255);

            #endregion

            #region ScrollBarColors

            ScrollBarColor = Color.FromArgb(255, 35, 74, 86);

            #endregion

            #region TopBarIconsColors

            TopBarIconsHoverColor = Color.FromArgb(255, 48, 80, 113);
            TopBarIconsDownColor = Color.White;
            TopBarIconsShadowColor = Color.FromArgb(80, 114, 145);
            TopBarIconsGlyphCircleColor = Color.Red;
            TopBarIconsGlyphCircleTextColor = Color.White;

            TopBarSearchBoxIconsColor = new Color4f(80, 114, 145, 255).ToNVGColor();
            TopBarSearchBoxNormalColor = new Color4f(48, 80, 113, 255).ToNVGColor();
            TopBarSearchBoxClickedColor = new Color4f(255, 255, 255, 255).ToNVGColor();
            TopBarSearchBoxTextNormalColor = new Color4f(80, 114, 145, 255).ToNVGColor();

            TopBarSearcBoxFocusedColor = new Color4f(255, 255, 255, 255).ToNVGColor();
            TopBarSearchBoxUnfocusedColor = new Color4f(48, 80, 113, 255).ToNVGColor();

            #endregion

            #region SceneListDropDown

            DropDownButtonSize = new Size(32, 32);
            DropDownBackColor = Color.FromArgb(255, 80, 114, 145);
            DropDownBackColorHover = Color.FromArgb(255, 100, 134, 165);
            DropDownBackColorClick = Color.FromArgb(255, 70, 94, 125);
            DropDownForeColor = Color.WhiteSmoke;
            DropDownForeColorHover = Color.WhiteSmoke;
            DropDownForeColorClick = Color.WhiteSmoke;

            #endregion SceneListDropDown
        }
    }

    public struct Color4f
    {
        public static readonly Color4f White = new Color4f(1f, 1f);
        public static readonly Color4f Black = new Color4f(0f, 1f);
        public static readonly Color4f Red = new Color4f(1f, 0f, 0f, 1f);
        public static readonly Color4f Green = new Color4f(0f, 1f, 0f, 1f);
        public static readonly Color4f Blue = new Color4f(0f, 0f, 1f, 1f);

        private Vector4 v;

        public float R { get { return v.X; } }
        public float G { get { return v.Y; } }
        public float B { get { return v.Z; } }
        public float A { get { return v.W; } }

        public byte Rb { get { return (byte)(v.X * 255); } }
        public byte Gb { get { return (byte)(v.Y * 255); } }
        public byte Bb { get { return (byte)(v.Z * 255); } }
        public byte Ab { get { return (byte)(v.W * 255); } }

        public Color4f(Vector4 color)
        {
            this.v = color;
        }

        public Color4f(float r, float g, float b, float a)
            : this(new Vector4(r, g, b, a)) { }

        public Color4f(Vector3 color, float alpha)
            : this(color.X, color.Y, color.Z, alpha) { }

        public Color4f(float intensity, float alpha)
            : this(Vector3.One * intensity, alpha) { }

        public Color4f(int intensity, int alpha)
            : this(Vector3.One * intensity / 255f, alpha / 255f) { }

        public Color4f(Vector3 color)
            : this(color, 1f) { }

        public Color4f(Vector3i color)
            : this(color.Div(255f)) { }

        public Color4f(int r, int g, int b, int a)
            : this((new Vector4i(r, g, b, a).Div(255f))) { }

        public Color4f Contrast()
        {
            float luminance = v.Dot(new Vector4(0.299f, 0.587f, 0.144f, 0f));
            float intensity = luminance < 0.5f ? 1f : 0f;
            return new Color4f(intensity, 1f);
        }

        public Color4f WithAlpha(int alpha)
        {
            this.v.W = alpha / 255f;
            return this;
        }

        public NVGcolor ToNVGColor()
        {
            NVGcolor ret;
            ret.r = this.R;
            ret.g = this.G;
            ret.b = this.B;
            ret.a = this.A;
            return ret;
        }


    }

    public class ColorStartEnd
    {
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }



        public ColorStartEnd()
        { }

        public ColorStartEnd(Color startcolor)
        {
            this.StartColor = startcolor;
            this.EndColor = startcolor;
        }
        public ColorStartEnd(Color startcolor, Color endcolor)
        {
            this.StartColor = startcolor;
            this.EndColor = endcolor;
        }
    }

    public class UITool
    {



        /// <summary>
        /// cp to UTF8. (mysterious code)
        /// </summary>
        /// <returns>The to UTF8.</returns>
        /// <param name="cp">Cp.</param>
        public static string cpToUTF8(int cp)
        {
            byte[] icon = new byte[8];

            int n = 0;
            if (cp < 0x80)
                n = 1;
            else if (cp < 0x800)
                n = 2;
            else if (cp < 0x10000)
                n = 3;
            else if (cp < 0x200000)
                n = 4;
            else if (cp < 0x4000000)
                n = 5;
            else if (cp <= 0x7fffffff)
                n = 6;
            icon[n] = (byte)'\0';
            switch (n)
            {
                case 6:
                    goto case_6;
                case 5:
                    goto case_5;
                case 4:
                    goto case_4;
                case 3:
                    goto case_3;
                case 2:
                    goto case_2;
                case 1:
                    goto case_1;
            }
            goto end;

            case_6:
            icon[5] = (byte)(0x80 | (cp & 0x3f));
            cp = cp >> 6;
            cp |= 0x4000000;
            case_5:
            icon[4] = (byte)(0x80 | (cp & 0x3f));
            cp = cp >> 6;
            cp |= 0x200000;
            case_4:
            icon[3] = (byte)(0x80 | (cp & 0x3f));
            cp = cp >> 6;
            cp |= 0x10000;
            case_3:
            icon[2] = (byte)(0x80 | (cp & 0x3f));
            cp = cp >> 6;
            cp |= 0x800;
            case_2:
            icon[1] = (byte)(0x80 | (cp & 0x3f));
            cp = cp >> 6;
            cp |= 0xc0;
            case_1:
            icon[0] = (byte)cp;

            end:

            string r = new string(Encoding.UTF8.GetChars(icon, 0, n));
            r = r.Trim(new char[] { '\0' });
            int rl = r.Length;

            return r;
        }

        /// <summary>
        /// Returns 1 if col.rgba is (0.0f,0.0f,0.0f,0.0f), 0 otherwise
        /// </summary>
        /// <returns><c>true</c>, if black was ised, <c>false</c> otherwise.</returns>
        /// <param name="col">Col.</param>
        public bool isBlack(NVGcolor col)
        {
            if (col.r == 0.0f && col.g == 0.0f && col.b == 0.0f && col.a == 0.0f)
            {
                return true;
            }
            return false;
        }


    }

    public static class VectorExtensions    // TODO:: Is this necessary? All functions above are inherent in Vector4
    {
        public static float Sum(this Vector4 v)
        {
            return v.X + v.Y + v.Z + v.W;
        }

        public static float Dot(this Vector4 v, Vector4 other)
        {
            Vector4 ret = v;
            ret.Scale(other);
            return ret.Sum();
        }

        public static Vector4 Div(this Vector4i v, float divisor)
        {
            Vector4 ret;
            ret.X = v.X / divisor;
            ret.Y = v.Y / divisor;
            ret.Z = v.Z / divisor;
            ret.W = v.W / divisor;
            return ret;
        }

        public static Vector3 Div(this Vector3i v, float divisor)
        {
            Vector3 ret;
            ret.X = v.X / divisor;
            ret.Y = v.Y / divisor;
            ret.Z = v.Z / divisor;
            return ret;
        }
    }

    public enum Entypo
    {
        ICON_PHONE = 0x1F4DE,
        ICON_MOBILE = 0x1F4F1,
        ICON_MOUSE = 0xE789,
        ICON_ADDRESS = 0xE723,
        ICON_MAIL = 0x2709,
        ICON_PAPER_PLANE = 0x1F53F,
        ICON_PENCIL = 0x270E,
        ICON_FEATHER = 0x2712,
        ICON_ATTACH = 0x1F4CE,
        ICON_INBOX = 0xE777,
        ICON_REPLY = 0xE712,
        ICON_REPLY_ALL = 0xE713,
        ICON_FORWARD = 0x27A6,
        ICON_USER = 0x1F464,
        ICON_USERS = 0x1F465,
        ICON_ADD_USER = 0xE700,
        ICON_VCARD = 0xE722,
        ICON_EXPORT = 0xE715,
        ICON_LOCATION = 0xE724,
        ICON_MAP = 0xE727,
        ICON_COMPASS = 0xE728,
        ICON_DIRECTION = 0x27A2,
        ICON_HAIR_CROSS = 0x1F3AF,
        ICON_SHARE = 0xE73C,
        ICON_SHAREABLE = 0xE73E,
        ICON_HEART = 0x2665,
        ICON_HEART_EMPTY = 0x2661,
        ICON_STAR = 0x2605,
        ICON_STAR_EMPTY = 0x2606,
        ICON_THUMBS_UP = 0x1F44D,
        ICON_THUMBS_DOWN = 0x1F44E,
        ICON_CHAT = 0xE720,
        ICON_COMMENT = 0xE718,
        ICON_QUOTE = 0x275E,
        ICON_HOME = 0x2302,
        ICON_POPUP = 0xE74C,
        ICON_SEARCH = 0x1F50D,
        ICON_FLASHLIGHT = 0x1F526,
        ICON_PRINT = 0xE716,
        ICON_BELL = 0x1F514,
        ICON_LINK = 0x1F517,
        ICON_FLAG = 0x2691,
        ICON_COG = 0x2699,
        ICON_TOOLS = 0x2692,
        ICON_TROPHY = 0x1F3C6,
        ICON_TAG = 0xE70C,
        ICON_CAMERA = 0x1F4F7,
        ICON_MEGAPHONE = 0x1F4E3,
        ICON_MOON = 0x263D,
        ICON_PALETTE = 0x1F3A8,
        ICON_LEAF = 0x1F342,
        ICON_NOTE = 0x266A,
        ICON_BEAMED_NOTE = 0x266B,
        ICON_NEW = 0x1F4A5,
        ICON_GRADUATION_CAP = 0x1F393,
        ICON_BOOK = 0x1F4D5,
        ICON_NEWSPAPER = 0x1F4F0,
        ICON_BAG = 0x1F45C,
        ICON_AIRPLANE = 0x2708,
        ICON_LIFEBUOY = 0xE788,
        ICON_EYE = 0xE70A,
        ICON_CLOCK = 0x1F554,
        ICON_MIC = 0x1F3A4,
        ICON_CALENDAR = 0x1F4C5,
        ICON_FLASH = 0x26A1,
        ICON_THUNDER_CLOUD = 0x26C8,
        ICON_DROPLET = 0x1F4A7,
        ICON_CD = 0x1F4BF,
        ICON_BRIEFCASE = 0x1F4BC,
        ICON_AIR = 0x1F4A8,
        ICON_HOURGLASS = 0x23F3,
        ICON_GAUGE = 0x1F6C7,
        ICON_LANGUAGE = 0x1F394,
        ICON_NETWORK = 0xE776,
        ICON_KEY = 0x1F511,
        ICON_BATTERY = 0x1F50B,
        ICON_BUCKET = 0x1F4FE,
        ICON_MAGNET = 0xE7A1,
        ICON_DRIVE = 0x1F4FD,
        ICON_CUP = 0x2615,
        ICON_ROCKET = 0x1F680,
        ICON_BRUSH = 0xE79A,
        ICON_SUITCASE = 0x1F6C6,
        ICON_TRAFFIC_CONE = 0x1F6C8,
        ICON_GLOBE = 0x1F30E,
        ICON_KEYBOARD = 0x2328,
        ICON_BROWSER = 0xE74E,
        ICON_PUBLISH = 0xE74D,
        ICON_PROGRESS_3 = 0xE76B,
        ICON_PROGRESS_2 = 0xE76A,
        ICON_PROGRESS_1 = 0xE769,
        ICON_PROGRESS_0 = 0xE768,
        ICON_LIGHT_DOWN = 0x1F505,
        ICON_LIGHT_UP = 0x1F506,
        ICON_ADJUST = 0x25D1,
        ICON_CODE = 0xE714,
        ICON_MONITOR = 0x1F4BB,
        ICON_INFINITY = 0x221E,
        ICON_LIGHT_BULB = 0x1F4A1,
        ICON_CREDIT_CARD = 0x1F4B3,
        ICON_DATABASE = 0x1F4F8,
        ICON_VOICEMAIL = 0x2707,
        ICON_CLIPBOARD = 0x1F4CB,
        ICON_CART = 0xE73D,
        ICON_BOX = 0x1F4E6,
        ICON_TICKET = 0x1F3AB,
        ICON_RSS = 0xE73A,
        ICON_SIGNAL = 0x1F4F6,
        ICON_THERMOMETER = 0x1F4FF,
        ICON_WATER = 0x1F4A6,
        ICON_SWEDEN = 0xF601,
        ICON_LINE_GRAPH = 0x1F4C8,
        ICON_PIE_CHART = 0x25F4,
        ICON_BAR_GRAPH = 0x1F4CA,
        ICON_AREA_GRAPH = 0x1F53E,
        ICON_LOCK = 0x1F512,
        ICON_LOCK_OPEN = 0x1F513,
        ICON_LOGOUT = 0xE741,
        ICON_LOGIN = 0xE740,
        ICON_CHECK = 0x2713,
        ICON_CROSS = 0x274C,
        ICON_SQUARED_MINUS = 0x229F,
        ICON_SQUARED_PLUS = 0x229E,
        ICON_SQUARED_CROSS = 0x274E,
        ICON_CIRCLED_MINUS = 0x2296,
        ICON_CIRCLED_PLUS = 0x2295,
        ICON_CIRCLED_CROSS = 0x2716,
        ICON_MINUS = 0x2796,
        ICON_PLUS = 0x2795,
        ICON_ERASE = 0x232B,
        ICON_BLOCK = 0x1F6AB,
        ICON_INFO = 0x2139,
        ICON_CIRCLED_INFO = 0xE705,
        ICON_HELP = 0x2753,
        ICON_CIRCLED_HELP = 0xE704,
        ICON_WARNING = 0x26A0,
        ICON_CYCLE = 0x1F504,
        ICON_CW = 0x27F3,
        ICON_CCW = 0x27F2,
        ICON_SHUFFLE = 0x1F500,
        ICON_BACK = 0x1F519,
        ICON_LEVEL_DOWN = 0x21B3,
        ICON_RETWEET = 0xE717,
        ICON_LOOP = 0x1F501,
        ICON_BACK_IN_TIME = 0xE771,
        ICON_LEVEL_UP = 0x21B0,
        ICON_SWITCH = 0x21C6,
        ICON_NUMBERED_LIST = 0xE005,
        ICON_ADD_TO_LIST = 0xE003,
        ICON_LAYOUT = 0x268F,
        ICON_LIST = 0x2630,
        ICON_TEXT_DOC = 0x1F4C4,
        ICON_TEXT_DOC_INVERTED = 0xE731,
        ICON_DOC = 0xE730,
        ICON_DOCS = 0xE736,
        ICON_LANDSCAPE_DOC = 0xE737,
        ICON_PICTURE = 0x1F304,
        ICON_VIDEO = 0x1F3AC,
        ICON_MUSIC = 0x1F3B5,
        ICON_FOLDER = 0x1F4C1,
        ICON_ARCHIVE = 0xE800,
        ICON_TRASH = 0xE729,
        ICON_UPLOAD = 0x1F4E4,
        ICON_DOWNLOAD = 0x1F4E5,
        ICON_SAVE = 0x1F4BE,
        ICON_INSTALL = 0xE778,
        ICON_CLOUD = 0x2601,
        ICON_UPLOAD_CLOUD = 0xE711,
        ICON_BOOKMARK = 0x1F516,
        ICON_BOOKMARKS = 0x1F4D1,
        ICON_OPEN_BOOK = 0x1F4D6,
        ICON_PLAY = 0x25B6,
        ICON_PAUS = 0x2016,
        ICON_RECORD = 0x25CF,
        ICON_STOP = 0x25A0,
        ICON_FF = 0x23E9,
        ICON_FB = 0x23EA,
        ICON_TO_START = 0x23EE,
        ICON_TO_END = 0x23ED,
        ICON_RESIZE_FULL = 0xE744,
        ICON_RESIZE_SMALL = 0xE746,
        ICON_VOLUME = 0x23F7,
        ICON_SOUND = 0x1F50A,
        ICON_MUTE = 0x1F507,
        ICON_FLOW_CASCADE = 0x1F568,
        ICON_FLOW_BRANCH = 0x1F569,
        ICON_FLOW_TREE = 0x1F56A,
        ICON_FLOW_LINE = 0x1F56B,
        ICON_FLOW_PARALLEL = 0x1F56C,
        ICON_LEFT_BOLD = 0xE4AD,
        ICON_DOWN_BOLD = 0xE4B0,
        ICON_UP_BOLD = 0xE4AF,
        ICON_RIGHT_BOLD = 0xE4AE,
        ICON_LEFT = 0x2B05,
        ICON_DOWN = 0x2B07,
        ICON_UP = 0x2B06,
        ICON_RIGHT = 0x27A1,
        ICON_CIRCLED_LEFT = 0xE759,
        ICON_CIRCLED_DOWN = 0xE758,
        ICON_CIRCLED_UP = 0xE75B,
        ICON_CIRCLED_RIGHT = 0xE75A,
        ICON_TRIANGLE_LEFT = 0x25C2,
        ICON_TRIANGLE_DOWN = 0x25BE,
        ICON_TRIANGLE_UP = 0x25B4,
        ICON_TRIANGLE_RIGHT = 0x25B8,
        ICON_CHEVRON_LEFT = 0xE75D,
        ICON_CHEVRON_DOWN = 0xE75C,
        ICON_CHEVRON_UP = 0xE75F,
        ICON_CHEVRON_RIGHT = 0xE75E,
        ICON_CHEVRON_SMALL_LEFT = 0xE761,
        ICON_CHEVRON_SMALL_DOWN = 0xE760,
        ICON_CHEVRON_SMALL_UP = 0xE763,
        ICON_CHEVRON_SMALL_RIGHT = 0xE762,
        ICON_CHEVRON_THIN_LEFT = 0xE765,
        ICON_CHEVRON_THIN_DOWN = 0xE764,
        ICON_CHEVRON_THIN_UP = 0xE767,
        ICON_CHEVRON_THIN_RIGHT = 0xE766,
        ICON_LEFT_THIN = 0x2190,
        ICON_DOWN_THIN = 0x2193,
        ICON_UP_THIN = 0x2191,
        ICON_RIGHT_THIN = 0x2192,
        ICON_ARROW_COMBO = 0xE74F,
        ICON_THREE_DOTS = 0x23F6,
        ICON_TWO_DOTS = 0x23F5,
        ICON_DOT = 0x23F4,
        ICON_CC = 0x1F545,
        ICON_CC_BY = 0x1F546,
        ICON_CC_NC = 0x1F547,
        ICON_CC_NC_EU = 0x1F548,
        ICON_CC_NC_JP = 0x1F549,
        ICON_CC_SA = 0x1F54A,
        ICON_CC_ND = 0x1F54B,
        ICON_CC_PD = 0x1F54C,
        ICON_CC_ZERO = 0x1F54D,
        ICON_CC_SHARE = 0x1F54E,
        ICON_CC_REMIX = 0x1F54F,
        ICON_DB_LOGO = 0x1F5F9,
        ICON_DB_SHAPE = 0x1F5FA,
        ICON_GITHUB = 0xF300,
        ICON_C_GITHUB = 0xF301,
        ICON_FLICKR = 0xF303,
        ICON_C_FLICKR = 0xF304,
        ICON_VIMEO = 0xF306,
        ICON_C_VIMEO = 0xF307,
        ICON_TWITTER = 0xF309,
        ICON_C_TWITTER = 0xF30A,
        ICON_FACEBOOK = 0xF30C,
        ICON_C_FACEBOOK = 0xF30D,
        ICON_S_FACEBOOK = 0xF30E,
        ICON_GOOGLEPLUS = 0xF30F,
        ICON_C_GOOGLEPLUS = 0xF310,
        ICON_PINTEREST = 0xF312,
        ICON_C_PINTEREST = 0xF313,
        ICON_TUMBLR = 0xF315,
        ICON_C_TUMBLR = 0xF316,
        ICON_LINKEDIN = 0xF318,
        ICON_C_LINKEDIN = 0xF319,
        ICON_DRIBBBLE = 0xF31B,
        ICON_C_DRIBBBLE = 0xF31C,
        ICON_STUMBLEUPON = 0xF31E,
        ICON_C_STUMBLEUPON = 0xF31F,
        ICON_LASTFM = 0xF321,
        ICON_C_LASTFM = 0xF322,
        ICON_RDIO = 0xF324,
        ICON_C_RDIO = 0xF325,
        ICON_SPOTIFY = 0xF327,
        ICON_C_SPOTIFY = 0xF328,
        ICON_QQ = 0xF32A,
        ICON_INSTAGRAM = 0xF32D,
        ICON_DROPBOX = 0xF330,
        ICON_EVERNOTE = 0xF333,
        ICON_FLATTR = 0xF336,
        ICON_SKYPE = 0xF339,
        ICON_C_SKYPE = 0xF33A,
        ICON_RENREN = 0xF33C,
        ICON_SINA_WEIBO = 0xF33F,
        ICON_PAYPAL = 0xF342,
        ICON_PICASA = 0xF345,
        ICON_SOUNDCLOUD = 0xF348,
        ICON_MIXI = 0xF34B,
        ICON_BEHANCE = 0xF34E,
        ICON_GOOGLE_CIRCLES = 0xF351,
        ICON_VK = 0xF354,
        ICON_SMASHING = 0xF357,
    }
}
