//#define uDEBUG

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

using NthDimension.Algebra;
using NthDimension.Forms;
using NthDimension.Forms.Events;
using NthDimension.Forms.Widgets;
using NthDimension.Rendering.Utilities;
using NthDimension.Rasterizer.NanoVG;

//using NthStudio.Plugins;
using NthStudio.Gui.Dialogs;
using NthStudio.Gui.Widgets;
using NthStudio.Gui.Widgets.PropertyGrid;

namespace NthStudio.Gui.Displays
{
    using NthDimension.Forms.Dialogs;
    using NthDimension.Physics;
    using NthDimension.Physics.Dynamics;
    using NthDimension.Procedural.Dungeon;
    using NthDimension.Rendering;
    using NthDimension.Rendering.Drawables.Level;
    using NthDimension.Rendering.Drawables.Models;
    using NthDimension.Utilities;
    using NthStudio.Game.Procedural;
    using System.IO;
    using System.Linq;
    //using System.Windows.Forms;

    public class WorkspaceDisplay : Widget.WHUD, IUiContext
    {
        #region IUiContext implementation

        public virtual Point CursorPos
        {
            get
            {
                //MouseState ms = otkWindow.Mouse.GetState();
                Point p = new Point(m_window.Mouse.X, m_window.Mouse.Y);
                return p; //new Point(ms.X, ms.Y);
            }
        }
        public virtual bool ShowCursor
        {
            set
            {
                m_window.CursorVisible = value;
            }
        }
        public FontParameters CreateFont(NanoFont font)
        {
            int id;

            if (font.FontData == null)
                id = NanoVG.CreateFont(m_vg, font.InternalName, font.FileName);
            else
                id = NanoVG.CreateFont(m_vg, font.InternalName, font.FontData);

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
                NanoVG.nvgFontSize(m_vg, height);
                NanoVG.nvgFontFace(m_vg, id);
                NanoVG.nvgTextMetrics(m_vg, ref ascender, ref descender, ref lineHeight);

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
            if (!NanoVG.FontCreated(m_vg, font.Id))
                throw new Exception(string.Format("Font '{0}', not created", font));

            //if (fontsLookUp.ContainsKey(font))
            //    return fontsLookUp[font];

            Size s = new Size();

            try
            {
                var bounds = new float[4];


                //NanoVG.nvgSave(vg);

                NanoVG.nvgFontSize(m_vg, font.Height);
                NanoVG.nvgFontFace(m_vg, font.Id);

                NanoVG.nvgTextBounds(m_vg, 0, 0, text, bounds);

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
            if (!NanoVG.FontCreated(m_vg, font.Id))
                throw new Exception(string.Format("Font '{0}', not created", font));

            var bounds = new float[4];


            //NanoVG.nvgSave(vg);

            NanoVG.nvgFontSize(m_vg, font.FontSize);
            NanoVG.nvgFontFace(m_vg, font.Id);

            NanoVG.nvgTextBounds(m_vg, 0, 0, text, bounds);

            //NanoVG.nvgRestore(vg);

            var s = new Size((int)(bounds[2] - bounds[0]), (int)(bounds[3] - bounds[1]));

            return s;
        }
        // TODO:: Transfer to NanoFont
        public virtual float MeasureTextWidth(string text, NanoFont font)
        {
            if (!NanoVG.FontCreated(m_vg, font.Id))
                throw new Exception(String.Format("Font '{0}', not created", font.ToString()));

            //NanoVG.nvgSave(vg);

            NanoVG.nvgFontSize(m_vg, font.Height);
            NanoVG.nvgFontFace(m_vg, font.Id);

            float textWidth = 100f;

            try { textWidth = NanoVG.nvgTextBounds(m_vg, 0, 0, text); }
            catch { ConsoleUtil.errorlog("Failed to Measure Text", text); }

            //NanoVG.nvgRestore(vg);

            return textWidth;
        }
        #endregion IUiContext

        #region Plugin Variables
#pragma warning disable CS0162
        //private PluginStoreRefreshDialog m_refresher;
        //private PluginStore m_pluginStore;
        private Widget m_pluginWidget;
        #endregion Plugin Variables

        private bool m_initMouse = false;
        private readonly NVGcontext m_vg;
        private readonly StudioWindow m_window;

        private WorkspaceDisplayController m_controller;

        public bool Init = false;

#if uDEBUG
        private Widget lastDebug;
#endif


        private MenuStrip           menuMain;
        private MenuStripItem       menuMain_ProjectScene;
        private MenuStripItem       menuMain_ProjectOpen;
        private OpenFilesDialog                 openSceneDialog;
        private OpenFilesDialogResultHandler    openSceneHandler;
        private MenuStripItem menuMain_Network;
        private MenuStripItem menuMain_Network_Server;
        private MenuStripItem menuMainNetwork_Client;
        private MenuStripItem       menuMain_Select;
        //private MenuStripItem       menuMain_ProjectNew;
        //private MenuStripItem                   miFileOpen;
        //private MenuStripItem                   miFileClose;
        //private MenuStripItem                   miOpenFile;
        //private MenuStripItem                   miOpenProSol;
        //private MenuStripItem                   miCloseFile;
        //private MenuStripItem                   miCloseProSol;
        private MenuStripItem       miView;
        private MenuStripItem       miWindow;
        private MenuStripItem       miWindow_FBO;
        private MenuStripItem       miWindow_FBOView;
        //private MenuStripItem                   miWindow_FBONormalsDepthView;
        private MenuStripItem       miSimulation;
        private MenuStripItem       miSimulation_Crowd;
        private MenuStripItem       miWindowAssets;
        private MenuStripItem       miWindowMaterials;
        private MenuStripItem       miWindowDotNet;
        //private SplitterBox         m_splitBarFromEditor;
        private TabbedDocs          m_editor;
        private TreeView            m_tree;
        private ProjectNode         m_projectNode;
        private CameraNode          m_projectCameraViewsNode;
        private InputNode           m_projectInputHandlersNode;
        private SceneNode           m_projectScenesNode;
        private string              m_projectNodeString = "New Project";
        private string              m_projectCameraViewsNodeString = "Camera Views ({0})";
        private string              m_projectInputHandlersNodeString = "Input Handlers ({0})";
        private string              m_projectScenesNodeString = "Scenes ({0})";
        private TreeView            m_scriptTree;
        private PropertyGrid        m_properies;
        private DefaultObjectAdapter m_propertiesAdapter;
        private Panel               m_statusStrip;
        private LabelProgress       m_statusLabel;
        private Label               m_statusTimeLabel;

        //private Picture                         FBOPic;

        private DateTime            guiInactive = DateTime.Now;
        private int                 guiInactiveIntervalMsec = 5000;

        private DateTime            mouseInactive = DateTime.Now;
        private int                 mouseInactiveIntervalMsec = 12000;
        private Point               mouseLast;

        readonly Random             rand = new Random();

        System.Text.StringBuilder   sb;

        #region Ctor

        public WorkspaceDisplay(StudioWindow window, bool aaSupport)
        {
            LibContext = this;
            this.m_vg = StudioWindow.vg;
            this.m_window = window;
            this.BGColor = Color.Red;
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Function initializes widget controls and registers parent window input handlers for mouse and keyboard
        /// </summary>
        public void Initialize()
        {
            SuspendLayout();
            {
                Size = new Size(m_window.Width, m_window.Height);
                ConstructApp();
            }
            ResumeLayout();

            this.m_window.Cursor = new OpenTK.MouseCursor(
                (int)Cursors.Default.Cursors[0].Xhot,
                (int)Cursors.Default.Cursors[0].Yhot,
                (int)Cursors.Default.Cursors[0].Width,
                (int)Cursors.Default.Cursors[0].Height,
                Cursors.Default.Cursors[0].data);

            Cursors.MouseCursorChanged += onWindow_MouseCursorChanged;

            this.registerInputHandlers();

            //StudioWindow.Instance.Scene = m_controller.Scene;
        }
        private void registerInputHandlers()
        {
            this.m_window.MouseDown += delegate (object sender, OpenTK.Input.MouseButtonEventArgs mE)
            {
                this.MouseDown(new MouseButtonEventArgs(mE.X, mE.Y, mE.Button.ToNth(), mE.IsPressed));
            };
            this.m_window.MouseUp += delegate (object sender, OpenTK.Input.MouseButtonEventArgs mE)
            {
                this.MouseUp(new MouseButtonEventArgs(mE.X, mE.Y, mE.Button.ToNth(), mE.IsPressed));
            };
            this.m_window.MouseMove += delegate (object sender, OpenTK.Input.MouseMoveEventArgs mE)
            {
                NthDimension.MouseButton emb = NthDimension.MouseButton.None;

                if (mE.Mouse.IsAnyButtonDown)
                {
                    if (mE.Mouse.IsButtonDown(OpenTK.Input.MouseButton.Left))
                        emb = NthDimension.MouseButton.Left;
                    else if (mE.Mouse.IsButtonDown(OpenTK.Input.MouseButton.Middle))
                        emb = NthDimension.MouseButton.Middle;
                    else if (mE.Mouse.IsButtonDown(OpenTK.Input.MouseButton.Right))
                        emb = NthDimension.MouseButton.Right;
                }


                this.MouseMove(new MouseEventArgs(emb, mE.X, mE.Y, mE.XDelta, mE.YDelta));
            };
            this.m_window.MouseWheel += delegate (object sender, OpenTK.Input.MouseWheelEventArgs mE)
            {
                this.MouseWheel(new MouseEventArgs(NthDimension.MouseButton.None, mE.X, mE.Y, 0, 0, mE.Delta));
            };
            this.m_window.MouseEnter += delegate (object sender, EventArgs mE)
            {
                this.MouseEnter(mE);
            };
            this.m_window.MouseLeave += delegate (object sender, EventArgs mE)
            {
                this.MouseLeave(mE);
            };

            this.m_window.KeyDown += delegate (object sender, OpenTK.Input.KeyboardKeyEventArgs kE)
            {
                this.KeyDown(new KeyEventArgs(kE.Key.ToNth()));
            };
            this.m_window.KeyUp += delegate (object sender, OpenTK.Input.KeyboardKeyEventArgs kE)
            {
                this.KeyUp(new KeyEventArgs(kE.Key.ToNth()));
            };
            this.m_window.KeyPress += delegate (object sender, OpenTK.KeyPressEventArgs kE)
            {
                this.KeyPress(new KeyPressedEventArgs(kE.KeyChar));
            };
        }
        private void onWindow_MouseCursorChanged(object sender, MouseCursorChangedEventArgs args)
        {
            try
            {
                this.m_window.Cursor = new OpenTK.MouseCursor(
                    (int)args.NewCursor.Cursors[0].Xhot,
                    (int)args.NewCursor.Cursors[0].Yhot,
                    (int)args.NewCursor.Cursors[0].Width,
                    (int)args.NewCursor.Cursors[0].Height,
                    args.NewCursor.Cursors[0].data);
            }
            catch 
            {

                // Below is error supressed starting Nov-27-2019 (Added NonPlayerModel+MousePicking+NanoVGCursor over UI)

                /*
                 WindowsGame.mousePickAvatars()    at System.Drawing.Bitmap.GetHicon()
                    at OpenTK.Platform.Windows.WinGLNative.set_Cursor(MouseCursor value)
                    at NthStudio.Gui.Displays.WorkspaceDisplay.onWindow_MouseCursorChanged(Object sender, MouseCursorChangedEventArgs args) in D:\NthDimension 2018 Revisited\NthDimension\NthStudio\Gui\Displays\WorkspaceDisplay.cs:line 352
                    at NthDimension.Forms.Cursors.set_Cursor(NanoCursor value) in D:\NthDimension 2018 Revisited\NthDimension\NthDimension.Forms\Cursors.cs:line 77
                    at NthStudio.StudioWindow.<>c__DisplayClass119_0.<mousePickAvatars>b__0(Drawable drawable) in D:\NthDimension 2018 Revisited\NthDimension\NthStudio\StudioWindow.Picking.cs:line 86WindowsGame.mousePickAvatars()    at System.Drawing.Bitmap.GetHicon()
                    at OpenTK.Platform.Windows.WinGLNative.set_Cursor(MouseCursor value)
                    at NthStudio.Gui.Displays.WorkspaceDisplay.onWindow_MouseCursorChanged(Object sender, MouseCursorChangedEventArgs args) in D:\NthDimension 2018 Revisited\NthDimension\NthStudio\Gui\Displays\WorkspaceDisplay.cs:line 352
                    at NthDimension.Forms.Cursors.set_Cursor(NanoCursor value) in D:\NthDimension 2018 Revisited\NthDimension\NthDimension.Forms\Cursors.cs:line 77
                    at NthStudio.StudioWindow.<>c__DisplayClass119_0.<mousePickAvatars>b__0(Drawable drawable) in D:\NthDimension 2018 Revisited\NthDimension\NthStudio\StudioWindow.Picking.cs:line 97A generic error occurred in GDI+.
                    A generic error occurred in GDI+.

                */
            }
        }

        #region Plugins List
        //public void PluginsRefresh()
        //{
        //    Program.PluginStore = m_refresher.RefreshPluginStore(this);

        //    if (m_pluginStore.Plugins.Count > 0)
        //        this.loadPluginsList();


        //    foreach (Widget plugin in m_pluginWidget.Widgets)
        //        if (plugin is INotifyWidget)
        //            ((INotifyWidget)plugin).OnNotify += this.OnNotify;

        //}

        private void loadPluginsList()
        {
            //foreach (PluginInfo plugin in Program.PluginStore.Plugins)
            //    doPluginLoad(plugin);
        }
        /// <summary>
        /// Sets up interfaces as well
        /// </summary>
        /// <param name="plugin"></param>
        private void doPluginLoad(object plugin)
        {
            ////if (this.InvokeRequired)
            ////    this.Invoke(new PluginAction(doPluginLoad), new object[] { plugin });
            ////else
            //{
            //    //this.Cursor = Cursors.AppStarting;
            //    PluginInfo p = (PluginInfo)plugin;

            //    #region Interface setup IoC

            //    // TODO:: IInputService and IOutputService

            //    ServiceManager.SingleInstance.Services = new List<IService>();

            //    if (p is IService)
            //    {
            //        ServiceManager.SingleInstance.Services.Add(p as IService);

            //        //this.Subscribe<ping>(
            //        //    async ap =>
            //        //        {
            //        //            Console.WriteLine("Ping...");
            //        //            await Task.Delay(100);
            //        //            await this.Publish(TaskExtensions.AsTask(new pong()));
            //        //        }

            //        //    );
            //    }

            //    #endregion

            //    #region 1xControl Create the Plugin Control gui

            //    Widget pluginControl = (Widget)Activator.CreateInstance(Assembly.LoadFile(Plugins.EnvironmentSettings.GetFullPath(p.AssemblyFile)).GetType(p.Type));
            //    //pluginControl.SuspendLayout();
            //    pluginControl.Anchor = EAnchorStyle.Top & EAnchorStyle.Left;
            //    //pluginControl.AutoSize = true;
            //    //pluginControl.Top = 0;
            //    //pluginControl.Left = 0;

            //    pluginControl.Dock = EDocking.Fill;


            //    throw new NotImplementedException();

            //    #region Setup gui control INotify

            //    //if (pluginControl is INotifyWidget)
            //    //    ((INotifyWidget)pluginControl).OnNotify += this.OnNotify;

            //    #endregion

            //    //m_controls.Add(pluginControl);
            //    // ???  //this.Widgets.Add(pluginControl);

            //    #endregion

            //    //this.iocPanel.SuspendLayout();
            //    //this.iocPanel.Controls.Add(pluginControl);

            //    //pluginControl.ResumeLayout();

            //    //this.iocPanel.ResumeLayout();

            //    //pluginControl.Refresh();


            //    //this.Cursor = Cursors.Default;
            //}
        }
        #endregion Plugins List

        #endregion

        #region Update
        static bool mousePointerInWindow;
        public void Update()
        {
            this.ProcessUpdate();

            if (null == m_propertiesAdapter.SelectedObject)
                m_propertiesAdapter.SelectedObject = this as WorkspaceDisplay;

            this.updateUiTimeouts();
        }
        protected void updateUiTimeouts()
        {
            Vector3 mouse3d = new Vector3(); // TODO:: Raycasting from acrtive camera??? (3rd person only?)
            OpenTK.Input.MouseState mouseInput = OpenTK.Input.Mouse.GetCursorState();
            var p = new Point(mouseInput.X, mouseInput.Y);
            bool mouseInWindow = m_window.Bounds.Contains(p);
            bool mouseStill = p == mouseLast;

            if (mouseInWindow && !mousePointerInWindow)
                MouseEnter(EventArgs.Empty);
            if (!mouseInWindow && mousePointerInWindow)
                MouseLeave(EventArgs.Empty);

            mousePointerInWindow = mouseInWindow;

            if (DateTime.Now > guiInactive.AddMilliseconds(guiInactiveIntervalMsec))
            {
                if (StudioWindow.Instance.Mouse.Y > menuMain.Height + m_controller.Height)
                {
                    if (!menuMain.IsHide)
                        menuMain.Hide();
                    if (!m_controller.IsHide)
                        m_controller.Hide();
                }
                else
                {
                    if (!menuMain.IsVisible)
                        menuMain.Show();
                    if (!m_controller.IsVisible)
                        m_controller.Show();
                }
            }

            if (!mouseStill)
                mouseInactive = DateTime.Now.AddMilliseconds(mouseInactiveIntervalMsec);

            //ConsoleUtil.log(string.Format("{0} {1}", mouseStill, DateTime.Now - mouseInactive), false);

            if (DateTime.Now > mouseInactive /*.AddMilliseconds(mouseInactiveIntervalMsec) */)
            {
                if (m_statusStrip.IsVisible)
                    m_statusStrip.Hide();
                if (m_statusLabel.IsVisible)
                    m_statusLabel.Hide();
                if (m_statusTimeLabel.IsVisible)
                    m_statusTimeLabel.Hide();
            }
            else
            {
                m_statusLabel.Text = StudioWindow.Instance.VAR_AppState.ToString();
                m_statusTimeLabel.Text = string.Format("2D: {0}, 3D: {1}", mouseInput, mouse3d);//DateTime.Now.ToString("HH:mm:ss.fff");

                if (m_statusStrip.IsHide)
                    m_statusStrip.Show();
                if (m_statusLabel.IsHide)
                    m_statusLabel.Show();
                if (m_statusTimeLabel.IsHide)
                    m_statusTimeLabel.Show();
            }

            mouseLast = p;
        }
        #endregion Update

        #region Render
        public void Render()
        {
            this.Repaint();

            if (DoRepaintTree)
            {
                NanoVG.nvgBeginFrame(m_vg, Width, Height, 1f);
                var ngc = new NanoGContext(this, new Rectangle(0, 0, Width, Height), m_vg);
                DoPaint(ngc);

                if (!m_initMouse)
                {
                    OpenTK.Input.MouseState ms = OpenTK.Input.Mouse.GetCursorState();
                    Point lmp = m_window.PointToClient(new Point(ms.X, ms.Y));
                    InitMousePosition(lmp.X, lmp.Y);
                    m_initMouse = true;
                }



                NanoVG.nvgEndFrame(m_vg);
            }

            //foreach (RoomModel m in StudioWindow.Instance.Scene.Models)
            //    DrawRoomDimensions(m);
        }

        public void DrawRoomDimensions(RoomModel model)
        {
            if (null != model)
            {
                float width = StudioWindow.Instance.Width;
                float height = StudioWindow.Instance.Height;

                Vector3 topVertex = model.Position;
                topVertex.Y += model.Height / 2;

                Matrix4 scl = Matrix4.CreateScale(model.Size);
                scl.TransformVector(ref topVertex);

                Vector3 avatarPos = Vector3.Zero;
                Vector3 avatarTopVertex = model.Position + topVertex;
                Vector3 avatarPoint = new Vector3(avatarPos.X + avatarTopVertex.X,
                                                  avatarPos.Y + avatarTopVertex.Y,
                                                  avatarPos.Z + avatarTopVertex.Z);

                Vector2 infoPos = ((StudioWindow)StudioWindow.Instance).UnProject(avatarPoint,
                                                                                  StudioWindow.Instance.Player.ViewInfo.modelviewMatrix,
                                                                                  StudioWindow.Instance.Player.ViewInfo.projectionMatrix, (int)width, (int)height);
                infoPos.Y += 15;

                NanoVG.nvgSave(StudioWindow.vg);

                #region NanoVg draw
                NanoVG.nvgScissor(StudioWindow.vg, 0, 45, width, height);



                string dimensions = string.Format("{0}m W {1}m L {2}m H", model.Width, model.Length, model.Height);
                var bounds = new float[4];

                NanoVG.nvgFontSize(StudioWindow.vg, 20);
                NanoVG.nvgFontFace(StudioWindow.vg, "PlayRegular");
                NanoVG.nvgTextBounds(StudioWindow.vg, 0, 0, dimensions, bounds);

                var s = new Size((int)(bounds[2] - bounds[0]), (int)(bounds[3] - bounds[1]));

                NanoVG.nvgBeginPath(StudioWindow.vg);
                NanoVG.nvgRoundedRect(StudioWindow.vg, infoPos.X - s.Width / 2 - 5, height - infoPos.Y - s.Height / 2 - 3, s.Width + 10, s.Height + 5, 5f);
                NanoVG.nvgClosePath(StudioWindow.vg);

                NVGcolor print = Color.White.ToNVGColor();// vrColorUnselected;//vrColorDefault;

                //if (info.Gender.ToLower() == "male") print = vrColorMale;
                //if (info.Gender.ToLower() == "female") print = vrColorFemale;

                NanoVG.nvgFillColor(StudioWindow.vg, print);
                NanoVG.nvgFill(StudioWindow.vg);

                NanoVG.nvgFillColor(StudioWindow.vg, NanoVG.nvgRGBA(255, 255, 255, 255));
                NanoVG.nvgTextAlign(StudioWindow.vg, (int)(NanoGContext.EAlign.ALIGN_LEFT | NanoGContext.EAlign.ALIGN_MIDDLE));
                NanoVG.nvgText(StudioWindow.vg, infoPos.X - s.Width / 2, height - infoPos.Y, dimensions);
                #endregion

                NanoVG.nvgRestore(StudioWindow.vg);
            }
        }
        #endregion Render

        #region Input Handlers
        protected void MouseDown(MouseEventArgs e)
        {
            //this.m_window.playBlip();
            ProcessMouseDown(e);
        }
        protected void MouseUp(MouseEventArgs e)
        {
            ProcessMouseUp(e);
        }
        protected void MouseMove(MouseEventArgs e)
        {
#if uDEBUG
            int lx, ly = 0;
            Widget w = GetControlAt(e.X, e.Y, out lx, out ly);

            if (null != w)
            {
                m_window.Title = w.Name;
                w.ShowBoundsLines = true;
                lastDebug = w;
            }
#endif
            ProcessMouseMove(e);
        }
        protected void MouseWheel(MouseEventArgs e)
        {
            ProcessMouseWheel(e);
        }
        protected void MouseEnter(EventArgs e)
        {
            ProcessMouseEnter(e);
        }
        protected void MouseLeave(EventArgs e)
        {
            ProcessMouseLeave(e);
        }
        protected void KeyDown(KeyEventArgs e)
        {
            ProcessKeyDown(e);
        }
        protected void KeyUp(KeyEventArgs e)
        {
            ProcessKeyUp(e);
        }
        protected void KeyPress(KeyPressedEventArgs e)
        {
            ProcessKeyPress(e);
        }
        #endregion Input Handlers

        #region Plugin Notify
        #region Framework UI Event Handlers
        //public Action<INotificationSource, NotificationEventArgs> OnNotify(NotificationEventArgs notificationEventArgs)
        //private void OnNotify(INotifyWidget notifyControl, NotificationEventArgs notificationEventArgs)
        //{
        //    //MonoFlat_NotificationBox nfb = new MonoFlat_NotificationBox();
        //    //if (notificationEventArgs.Type == NotificationType.Error)
        //    //    nfb.NotificationType = MonoFlat_NotificationBox.Type.Error;
        //    //if (notificationEventArgs.Type == NotificationType.Notice)
        //    //    nfb.NotificationType = MonoFlat_NotificationBox.Type.Notice;
        //    //if (notificationEventArgs.Type == NotificationType.Success)
        //    //    nfb.NotificationType = MonoFlat_NotificationBox.Type.Success;
        //    //if (notificationEventArgs.Type == NotificationType.Warning)
        //    //    nfb.NotificationType = MonoFlat_NotificationBox.Type.Warning;

        //    //nfb.Size = new Size(200, 60);
        //    //nfb.Text = notificationEventArgs.Text;




        //    //this.m_pluginWidget.Widgets.Add(nfb);

        //    ConsoleUtil.log(notificationEventArgs.Text);


        //}
        #endregion
        #endregion Plugin Notify

        #region TreeNode Classes
        class ProjectNode : TreeNode
        {
            public ProjectNode()
            {
                Name = "New Project";
                Text = Name;
            }
            public ProjectNode(string name)
            {
                Name = name;
                Text = name;
            }
        }
        class SceneNode : TreeNode
        {
            public SceneNode()
            {
                Name = "New Scene";
                Text = Name;
            }
            public SceneNode(string name)
            {
                Name = name;
                Text = name;
            }
            public SceneNode(string name, TreeNode[] nodes) : base(name, nodes) { }
        }
        class InputNode : TreeNode
        {
            public InputNode()
            {
                Name = "New Input";
                Text = Name;
            }
            public InputNode(string name)
            {
                Name = name;
                Text = name;
            }
        }
        class CameraNode : TreeNode
        {
            public CameraNode()
            {
                Name = "New Camera";
                Text = Name;
            }
            public CameraNode(string name)
            {
                Name = name;
                Text = name;
            }
        }

        #endregion TreeNode Classes

        #region Tree Handlers
        private void treeNode_SelectionChanged()
        {
            //ConsoleUtil.log(string.Format("Click from {0}", m_tree.SelectedNode.Text));

            m_propertiesAdapter.SelectedObject = m_tree.SelectedNode;
        }

        private void treeNode_MouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void treeNode_MouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void treeNode_MouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is SceneNode)
                ConsoleUtil.log(string.Format("Scene Node {0} Clicked!", e.Node.Text));
        }
        #endregion TreeHandlers

        #region Design flaws
        public void FocusedChanged(object sender, EventArgs e)
        {
            //Repaint();
        }
        #endregion

        private void ConstructApp()
        {
            m_controller = new WorkspaceDisplayController();

            CreateMenus(m_controller);
            Widgets.Add(m_controller);

            #region Status Strip
            m_statusStrip = new Panel();
            m_statusStrip.Size = new Size(200, 20);
            m_statusStrip.Dock = EDocking.Bottom;
            m_statusStrip.PaintBackGround = true;

            m_statusLabel = new LabelProgress("Ready", Color.FromArgb(255, 100, 134, 165));
            m_statusLabel.Size = new Size(450, 18);
            m_statusLabel.Location = new Point(5, 0);
            m_statusLabel.Font = new NanoFont(NanoFont.DefaultRegular, 12f);
            m_statusLabel.BGColor = new Color();
            m_statusLabel.FGColor = Color.WhiteSmoke;
            m_statusLabel.PaintBackGround = true;
            m_statusStrip.Widgets.Add(m_statusLabel);

            m_statusTimeLabel = new Label(DateTime.Now.ToString("hh:mm:ss.fff"));
            m_statusTimeLabel.Size = new Size(100, 18);
            m_statusTimeLabel.Location = new Point(m_statusStrip.Width - m_statusTimeLabel.Width - 5, 0);
            m_statusTimeLabel.Font = new NanoFont(NanoFont.DefaultRegular, 12f);
            m_statusTimeLabel.Anchor = EAnchorStyle.Right;
            m_statusStrip.Widgets.Add(m_statusTimeLabel);
            //m_statusStrip.Widgets.Add(new TimeLine() { Size = new Size(100, 300), Dock = EDocking.Right });

            #endregion Status Strip

            Widgets.Add(m_statusStrip);

            #region Text Editor

            m_editor = new TabbedDocs();
            m_editor.Name = "TextEditor";
            m_editor.Dock = EDocking.Fill;
            m_editor.BGColor = Color.FromArgb(10, 72, 75, 85);
            #endregion Text Editor

            var m_splitBarFromEditor = new SplitterBox(ESplitterType.HorizontalScroll);
            m_splitBarFromEditor.Name = "SplitBoxH";
            m_splitBarFromEditor.SplitterBarLocation = 0.2f;
            m_splitBarFromEditor.Size = new Size(400, 400);
            m_splitBarFromEditor.Dock = EDocking.Fill;
            m_splitBarFromEditor.PaintBackGround = false;
            m_splitBarFromEditor.ShowBoundsLines = true;

            var m_splitBar = new SplitterBox(ESplitterType.VerticalScroll);
            m_splitBar.Name = "SplitBoxV";
            m_splitBar.SplitterBarLocation = 0f;
            m_splitBar.Size = new Size(400, 400);
            m_splitBar.Dock = EDocking.Fill;
            m_splitBar.BGColor = Color.FromArgb(10, 72, 75, 85);
            m_splitBar.PaintBackGround = false;
            m_splitBar.ShowBoundsLines = true;
            #region Project TreeView

            m_tree = new TreeView();
            m_tree.Name = "TreeView";
            m_tree.BGColor = Color.FromArgb(10, 72, 75, 85);
            m_tree.FGColor = Color.LightGray;
            m_tree.Dock = EDocking.Fill;
            m_tree.ShowMarginLines = false;
            m_tree.Size = new Size(800, 600);
            m_tree.PaintBackGround = false;


            m_tree.ShowLines = true;
            m_tree.FullRowSelect = true;
            //iTreeView.CheckBoxes = true;

            int projNode = m_tree.Nodes.Add(new TreeNode(m_projectNodeString, new TreeNode[]
            {
                m_projectCameraViewsNode    = new CameraNode(string.Format(m_projectCameraViewsNodeString, 0)),
                m_projectInputHandlersNode  = new InputNode(string.Format(m_projectInputHandlersNodeString, 0)),
                m_projectScenesNode         = new SceneNode(string.Format(m_projectScenesNodeString, 0))
            }));

            m_projectNode = m_tree.Nodes[projNode] as ProjectNode;

            m_tree.OnSelectionChanged += treeNode_SelectionChanged;
            m_tree.NodeMouseClick += treeNode_MouseClick;
            m_tree.NodeMouseDoubleClick += treeNode_MouseDoubleClick;
            m_tree.NodeMouseHover += treeNode_MouseHover;


            //m_treeView.Canvas.ShowMessageCentered = "Solution is empty";

            #endregion TreeView

            //#region Source Project TreeView
            //m_scriptTree = new TreeView();
            //m_scriptTree.Name = "TreeView Script";
            //m_scriptTree.BGColor = Color.FromArgb(10, 72, 75, 85);
            //m_scriptTree.FGColor = Color.LightGray;
            //m_scriptTree.Dock = EDocking.Fill;
            //m_scriptTree.ShowMarginLines = true;
            //m_scriptTree.Size = new Size(800, 600);
            //m_scriptTree.Nodes.Add(new TreeNode("Node 0") { ShowBoundsLines = true });
            //m_scriptTree.Canvas.BGColor = m_scriptTree.BGColor;
            //#endregion Source Project TreeView

            m_properies = new PropertyGrid();
            m_properies.Name = "Property Grid";
            m_properies.Dock = EDocking.Fill;
            m_properies.Size = new Size(800, 600);
            m_properies.BGColor = new Color(); // Transparent

            m_properies.ShowBoundsLines = true;

            m_splitBar.Panel0.Widgets.Add(m_tree);
            m_splitBar.Panel1.Widgets.Add(m_properies);

            m_splitBar.Panel0.BGColor = new Color();
            m_splitBar.Panel1.BGColor = new Color();

            m_splitBarFromEditor.Panel0.Widgets.Add(m_splitBar);
            m_splitBarFromEditor.Panel1.Widgets.Add(m_editor  );

            //Widgets.Add(m_splitDisplayH);

            m_propertiesAdapter = new DefaultObjectAdapter();
            m_propertiesAdapter.TargetPropertyGrid = m_properies;

            m_controller.TabbedTextEditor = m_editor;
            //m_controller.SolutionTreeView = m_scriptTree; // This call clears the tree's nodes

            m_controller.MouseEnterEvent += delegate
            {
                if (!menuMain.IsVisible)
                    menuMain.Show();
                if (!m_controller.IsVisible)
                    m_controller.Show();
            };
            m_controller.MouseLeaveEvent += delegate
            {
                if (StudioWindow.Instance.Mouse.Y > menuMain.Height + m_controller.Height &&
                !menuMain.IsHide)
                    menuMain.Hide();

                guiInactive = DateTime.Now;
            };
        }

        #region Menus

        List<StaticModel> roomModels = new List<StaticModel>();

        void CreateMenus(WorkspaceDisplayController pCentralManager)
        {
            menuMain = new MenuStrip();
            menuMain.BGColor = Color.FromArgb(66, 66, 66);

            {
                Widgets.Add(menuMain);
            }

            menuMain_ProjectScene = new MenuStripItem("Scene");
            menuMain_ProjectScene.Name = "Scene";
            {
                MenuStripItem menuMain_ProjectRecent = new MenuStripItem("Recent", true);
                {
                    var menuMain_ProjectRecent0 = new MenuStripItem("Appartment");
                    menuMain_ProjectRecent0.ItemClickedEvent += delegate (object sender, EventArgs e)
                    {
                        ((StudioWindow)StudioWindow.Instance).LoadScene(new ScenePackage("appartment0",    // CAUTION this is the actual scene\[name]\ used in cache creation
                                                                                            "appartment.xsn",
                                                                                            "cacheModelappartment0.ucf",
                                                                                            "cacheTextureappartment0.ucf",
                                                                                            "cacheMaterialappartment0.ucf"),
                                                                                            "Appartment");

                    };
                    var menuMain_ProjectRecent1 = new MenuStripItem("Kalamata");
                    menuMain_ProjectRecent1.ItemClickedEvent += delegate
                    {
                        ((StudioWindow)StudioWindow.Instance).LoadScene(new ScenePackage("kalamata",   // CAUTION this is the actual scene\[name]\ used in cache creation
                                                                                           "kalamata0.xsn",
                                                                                           "cacheModelkalamata0.ucf",
                                                                                           "cacheTexturekalamata0.ucf",
                                                                                           "cacheMaterialkalamata0.ucf"), 
                                                                                           "Kalamata");
                    };
                    var menuMain_ProjectRecent2 = new MenuStripItem("New York");
                    menuMain_ProjectRecent2.ItemClickedEvent += delegate
                    {
                        ((StudioWindow)StudioWindow.Instance).LoadScene(new ScenePackage("nyc",
                                                                                           "nyc.xsn",
                                                                                           "cacheModelnyc.ucf",
                                                                                           "cacheTexturenyc.ucf",
                                                                                           "cacheMaterialnyc.ucf"), 
                                                                                           "New York");
                    };
                    var menuMain_ProjectRecent3 = new MenuStripItem("Lounge");
                    menuMain_ProjectRecent3.ItemClickedEvent += delegate
                    {
                        ((StudioWindow)StudioWindow.Instance).LoadScene(new ScenePackage("lounge",
                                                                                          "lounge.xsn",
                                                                                          "cacheModellounge.ucf",
                                                                                          "cacheTexturelounge.ucf",
                                                                                          "cacheMateriallounge.ucf"), 
                                                                                          "Lounge");
                    };
                    var menuMain_ProjectRecent4 = new MenuStripItem("Hadley");
                    menuMain_ProjectRecent4.ItemClickedEvent += delegate
                    {
                        ((StudioWindow)StudioWindow.Instance).LoadScene(new ScenePackage("Hadley",
                                                                                          "Hadley.xsn",
                                                                                          "cacheModelhadley.ucf",
                                                                                          "cacheTexturehadley.ucf",
                                                                                          "cacheMaterialhadley.ucf"), 
                                                                                          "Hadley");
                    };
                    var menuMain_ProjectRecent5 = new MenuStripItem("Sponza");
                    menuMain_ProjectRecent5.ItemClickedEvent += delegate
                    {
                        ((StudioWindow)StudioWindow.Instance).LoadScene(new ScenePackage("sponza",
                                                                                          "sponza.xsn",
                                                                                          "cacheModelsponza.ucf",
                                                                                          "cacheTexturesponza.ucf",
                                                                                          "cacheMaterialsponza.ucf"), 
                                                                                          "Sponza");
                    };


                    menuMain_ProjectRecent.MenuItems.Add(menuMain_ProjectRecent0);
                    menuMain_ProjectRecent.MenuItems.Add(menuMain_ProjectRecent1);
                    menuMain_ProjectRecent.MenuItems.Add(menuMain_ProjectRecent2);
                    menuMain_ProjectRecent.MenuItems.Add(menuMain_ProjectRecent3);
                    menuMain_ProjectRecent.MenuItems.Add(menuMain_ProjectRecent4);
                    menuMain_ProjectRecent.MenuItems.Add(menuMain_ProjectRecent5);
                }


                menuMain_ProjectScene.MenuItems.Add(menuMain_ProjectRecent);
                menuMain_ProjectScene.MenuItems.Add(new MenuStripItemSeparator());
                //New
                MenuStripItem menuMain_ProjectNew = new MenuStripItem("New", false, "Ctrl-N") { Name = "New" };
                menuMain_ProjectScene.MenuItems.Add(menuMain_ProjectNew);
                menuMain_ProjectNew.ItemClickedEvent += delegate (object sender, EventArgs e)
                {
                    ((StudioWindow)StudioWindow.Instance).NewScene();
                };
                // Open
                menuMain_ProjectOpen = new MenuStripItem("Open", false, "Ctrl-O") { Name = "Open" };
                menuMain_ProjectScene.MenuItems.Add(menuMain_ProjectOpen);
                menuMain_ProjectOpen.ItemClickedEvent += delegate (object sender, EventArgs e)
                {
                    openSceneDialog = new OpenFilesDialog("Open Project", new string[] { "xsn" });
                    openSceneHandler = delegate (OpenFilesDialog o, EDialogResult result)
                    {
                        if(result == EDialogResult.Accept)
                        {
                            if (string.IsNullOrEmpty(o.FilesNames[0]))
                                throw new Exception("File name cannot be null");

                            string file = Path.GetFileName(o.FilesNames[0]);
                            string scene = Path.GetFileNameWithoutExtension(file);

                            ((StudioWindow)StudioWindow.Instance).LoadScene(new ScenePackage(string.Format("{0}", scene),
                                                                                        string.Format("{0}.xsn", scene),
                                                                                        string.Format("cacheModel{0}.ucf", scene),
                                                                                        string.Format("cacheTexture{0}.ucf", scene),
                                                                                        string.Format("cacheMaterial{0}.ucf", scene)),
                                                                                        string.Format("{0}{1}", scene.Substring(0,1).ToUpper(),
                                                                                                                scene.Substring(1, scene.Length - 1).ToLower()), 
                                                                                        true);

                        }
                    };
                    openSceneDialog.StartupPath = NthDimension.Utilities.DirectoryUtil.Documents;
                    openSceneDialog.Show(WindowHUD, openSceneHandler);
                };

                // Save
                MenuStripItem menuMain_ProjectSave = new MenuStripItem("Save", false, "Ctrl-S") { Name = "Save" };
                menuMain_ProjectScene.MenuItems.Add(menuMain_ProjectSave);
                menuMain_ProjectSave.ItemClickedEvent += delegate (object sender, EventArgs e)
                {
                    sb = new System.Text.StringBuilder();                    
                    StudioWindow.Instance.Scene.save(ref sb, 4);
                    

                    string fileParam = StudioWindow.Instance.Scene.SceneXsnFile;

                    if (null != ((StudioWindow)StudioWindow.Instance).ActiveSceneFile)
                        fileParam = Path.Combine(DirectoryUtil.Documents_Cache, string.Format("{0}.xsn", ((StudioWindow)StudioWindow.Instance).ActiveSceneFile.SceneIdentifier));

                    using (StreamWriter outfile = new StreamWriter(fileParam))
                        outfile.Write(sb.ToString());

                    ConsoleUtil.log(string.Format("{0} \n{2} Scene saved - {1}", sb.ToString(), 
                                                                                 DateTime.Now.ToString("hh:mm:ss MM/DD/yyyy"), 
                                                                                 !String.IsNullOrEmpty(fileParam) ?
                                                                                    fileParam : 
                                                                                    "[ No file ]"));

                    //SaveFileDialogResultHandler sfdr = delegate (SaveFileDialog o, EDialogResult result)
                    //{

                    //    if (result == EDialogResult.Cancel) return;

                    //    if (result == EDialogResult.Yes)
                    //    {
                    //        using (StreamWriter outfile = new StreamWriter(o.FileName))
                    //        {
                    //            outfile.Write(sb.ToString());
                    //        }
                    //    }

                    //    if (!String.IsNullOrEmpty(o.FileName))
                    //    {
                    //        string file = o.FileName;

                    //        using (StreamWriter sw = new StreamWriter(file))
                    //        {
                    //            sw.Write(sb.ToString());
                    //        }
                    //    }
                    //};



                    //if (null != ((StudioWindow)StudioWindow.Instance).ActiveSceneFile)
                    //    fileParam = Path.Combine(DirectoryUtil.Documents_Cache, string.Format("{0}.xsn", ((StudioWindow)StudioWindow.Instance).ActiveSceneFile.SceneIdentifier));

                    //SaveFileDialog sfd = new SaveFileDialog("Save scene", fileParam, new string[] { "*.xsn" });
                    //sfd.Show(((StudioWindow)StudioWindow.Instance).Screen2D, sfdr);

                };
                // Save As
                MenuStripItem menuMain_ProjectSaveAs = new MenuStripItem("Save As", false, "Ctrl-Shift-S") { Name = "Save As" };
                menuMain_ProjectScene.MenuItems.Add(menuMain_ProjectSaveAs);
                menuMain_ProjectSaveAs.ItemClickedEvent += delegate (object sender, EventArgs e)
                {
                    sb = new System.Text.StringBuilder();
                    StudioWindow.Instance.Scene.save(ref sb, 4);

                    string fileParam = StudioWindow.Instance.Scene.SceneXsnFile;

                    if (null != ((StudioWindow)StudioWindow.Instance).ActiveSceneFile)
                        fileParam = Path.Combine(DirectoryUtil.Documents_Cache, string.Format("{0}.xsn", ((StudioWindow)StudioWindow.Instance).ActiveSceneFile.SceneIdentifier));

                    SaveFileDialogResultHandler sfdr = delegate (SaveFileDialog o, EDialogResult result)
                    {

                        if (result == EDialogResult.Cancel) return;

                        if (result == EDialogResult.Accept)
                        {
                            using (StreamWriter outfile = new StreamWriter(o.FileName))
                            {
                                outfile.Write(sb.ToString());
                            }
                        }

                        if (!String.IsNullOrEmpty(o.FileName))
                        {
                            string file = o.FileName;

                            using (StreamWriter sw = new StreamWriter(file))
                            {
                                sw.Write(sb.ToString());
                            }
                        }
                    };
                    SaveFileDialog sfd = new SaveFileDialog("Save scene", fileParam, new string[] { "*.xsn" });
                    sfd.Show(((StudioWindow)StudioWindow.Instance).Screen2D, sfdr);
                };
                // Close
                MenuStripItem menuMain_ProjectClose = new MenuStripItem("Close") { Name = "Close" };
                menuMain_ProjectScene.MenuItems.Add(menuMain_ProjectClose);
                // -----
                menuMain_ProjectScene.MenuItems.Add(new MenuStripItemSeparator());
                MenuStripItem menuMain_ProjectImport = new MenuStripItem("Import", true) { Name = "Import" };
                {
                    // Import 
                    // Package
                    MenuStripItem menuMain_Project_ImportPackage = new MenuStripItem("Package (.npk)") { Name = "Package" };
                    menuMain_ProjectImport.MenuItems.Add(menuMain_Project_ImportPackage);
                    // Library
                    MenuStripItem menuMain_Project_ImportLibrary = new MenuStripItem("Library (.dll, .sln, .csproj)") { Name = "Library" };
                    menuMain_ProjectImport.MenuItems.Add(menuMain_Project_ImportLibrary);
                    menuMain_ProjectImport.MenuItems.Add(new MenuStripItemSeparator());
                    // Model
                    MenuStripItem menuMain_Project_ImportModel = new MenuStripItem("Geometry (.xmd, .dae, .obj)") { Name = "Geometry" };
                    menuMain_ProjectImport.MenuItems.Add(menuMain_Project_ImportModel);
                    menuMain_Project_ImportModel.ItemClickedEvent += delegate
                    {
                        openSceneDialog = new OpenFilesDialog("Import Geometry File", new string[] { "obj" });
                        openSceneHandler = delegate (OpenFilesDialog o, EDialogResult result)
                        {
                            if (result == EDialogResult.Accept)
                            {
                                if (string.IsNullOrEmpty(o.FilesNames[0]))
                                    throw new Exception("File name cannot be null");

                                string file = (o.FilesNames[0]);

                                if (!File.Exists(file))
                                    throw new FileNotFoundException(file);

                                NthDimension.Rendering.Utilities.ConsoleUtil.log("found object file: " + file);
                                ApplicationBase.Instance.MeshLoader.FromObj(file);
                                ApplicationBase.Instance.MeshLoader.LoadMeshes(null);
                                NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format("File {0} loaded :) " , file));                               

                            }
                        };
                        openSceneDialog.StartupPath = NthDimension.Utilities.DirectoryUtil.Documents;
                        openSceneDialog.Show(WindowHUD, openSceneHandler);
                    };
                    //           Bitmap
                    MenuStripItem menuMain_Project_ImportBitmap = new MenuStripItem("Bitmap (.dds, .png, .tif, .bmp)") { Name = "Bitmap" };
                    menuMain_ProjectImport.MenuItems.Add(menuMain_Project_ImportBitmap);
                    menuMain_Project_ImportBitmap.ItemClickedEvent += delegate
                    {
                        openSceneDialog = new OpenFilesDialog("Import Png Image", new string[] { "png" });
                        openSceneHandler = delegate (OpenFilesDialog o, EDialogResult result)
                        {
                            if (result == EDialogResult.Accept)
                            {
                                if (string.IsNullOrEmpty(o.FilesNames[0]))
                                    throw new Exception("File name cannot be null");

                                string file = (o.FilesNames[0]);

                                if (!File.Exists(file))
                                    throw new FileNotFoundException(file);

                                NthDimension.Rendering.Utilities.ConsoleUtil.log("found image file: " + file);
                                ApplicationBase.Instance.TextureLoader.fromPng(file);
                                ApplicationBase.Instance.TextureLoader.LoadTextures(null);
                                NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format("File {0} loaded :) ", file));

                            }
                        };
                        openSceneDialog.StartupPath = NthDimension.Utilities.DirectoryUtil.Documents;
                        openSceneDialog.Show(WindowHUD, openSceneHandler);


                    };
                    //           Material
                    MenuStripItem menuMain_Project_ImportMaterial = new MenuStripItem("Material (.xmf)") { Name = "Material" };
                    menuMain_ProjectImport.MenuItems.Add(menuMain_Project_ImportMaterial);
                    menuMain_Project_ImportMaterial.ItemClickedEvent += delegate
                    {
                        openSceneDialog = new OpenFilesDialog("Import Material File", new string[] { "xmf" });
                        openSceneHandler = delegate (OpenFilesDialog o, EDialogResult result)
                        {
                            if (result == EDialogResult.Accept)
                            {
                                if (string.IsNullOrEmpty(o.FilesNames[0]))
                                    throw new Exception("File name cannot be null");

                                string file = (o.FilesNames[0]);

                                if (!File.Exists(file))
                                    throw new FileNotFoundException(file);

                                NthDimension.Rendering.Utilities.ConsoleUtil.log("found material file: " + file);
                                ApplicationBase.Instance.MaterialLoader.FromXmlFile(file);
                                ApplicationBase.Instance.MaterialLoader.LoadMaterials(null);
                                NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format("File {0} loaded :) ", file));

                            }
                        };
                        openSceneDialog.StartupPath = NthDimension.Utilities.DirectoryUtil.Documents;
                        openSceneDialog.Show(WindowHUD, openSceneHandler);
                    };
                    //           Shader
                    MenuStripItem menuMain_Project_ImportShader = new MenuStripItem("Shader (.xsp)") { Name = "Shader" };
                    menuMain_ProjectImport.MenuItems.Add(menuMain_Project_ImportShader);
                    menuMain_Project_ImportShader.ItemClickedEvent += delegate
                    {
                        openSceneDialog = new OpenFilesDialog("Import Shader File", new string[] { "xsp" });
                        openSceneHandler = delegate (OpenFilesDialog o, EDialogResult result)
                        {
                            if (result == EDialogResult.Accept)
                            {
                                if (string.IsNullOrEmpty(o.FilesNames[0]))
                                    throw new Exception("File name cannot be null");

                                string file = (o.FilesNames[0]);

                                if (!File.Exists(file))
                                    throw new FileNotFoundException(file);

                                NthDimension.Rendering.Utilities.ConsoleUtil.log("found shader file: " + file);
                                ApplicationBase.Instance.ShaderLoader.FromXmlFile(file);
                                ApplicationBase.Instance.ShaderLoader.LoadShaders(null);
                                NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format("File {0} loaded :) ", file));

                            }
                        };
                        openSceneDialog.StartupPath = NthDimension.Utilities.DirectoryUtil.Documents;
                        openSceneDialog.Show(WindowHUD, openSceneHandler);
                    };
                    //           Prefab
                    MenuStripItem menuMain_Project_ImportTemplate = new MenuStripItem("Prefabricated (.xmt)") { Name = "Prefabricated" };
                    menuMain_ProjectImport.MenuItems.Add(menuMain_Project_ImportTemplate);
                    //           ------

                }
                menuMain_ProjectScene.MenuItems.Add(menuMain_ProjectImport);

                //           Library 
                menuMain_ProjectScene.MenuItems.Add(new MenuStripItemSeparator());
                MenuStripItem menuMain_ProjectSettings = new MenuStripItem("Settings", true) { Name = "Settings" };
                {
                    // Settings->Video
                    MenuStripItem menuMain_Project_SettingsVideo = new MenuStripItem("Video") { Name = "Video" };
                    menuMain_ProjectSettings.MenuItems.Add(menuMain_Project_SettingsVideo);
                    //           -> Audio
                    MenuStripItem menuMain_Project_SettingsAudio = new MenuStripItem("Audio") { Name = "Audio" };
                    menuMain_ProjectSettings.MenuItems.Add(menuMain_Project_SettingsAudio);
                    //           -> Input
                    MenuStripItem menuMain_Project_SettingsInput = new MenuStripItem("Input") { Name = "Input" };
                    menuMain_ProjectSettings.MenuItems.Add(menuMain_Project_SettingsInput);
                    //           -> Fonts
                    MenuStripItem menuMain_Project_SettingsFonts = new MenuStripItem("True-Type Fonts") { Name = "True-Type Fonts" };
                    menuMain_ProjectSettings.MenuItems.Add(menuMain_Project_SettingsFonts);
                    //           -> Project
                    MenuStripItem menuMain_Project_SettingsEnvironment = new MenuStripItem("Environment") { Name = "Environment" };
                    menuMain_ProjectSettings.MenuItems.Add(menuMain_Project_SettingsEnvironment);
                    menuMain_Project_SettingsEnvironment.ItemClickedEvent += delegate
                    {
                        new Dialogs.EnvironmentSettings().Show(this.m_window.Screen2D);
                    };
                }

                menuMain_ProjectScene.MenuItems.Add(menuMain_ProjectSettings);
                menuMain_ProjectScene.MenuItems.Add(new MenuStripItemSeparator());

            }

            menuMain_Network = new MenuStripItem("Network");
            menuMain_Network.Name = "Network";
            menuMain_Network_Server = new MenuStripItem("Host Session", true); menuMain_Network_Server.Name = "Host Session";
            menuMainNetwork_Client = new MenuStripItem("Login", true); menuMainNetwork_Client.Name = "Login";
            menuMain_Network_Server.ItemClickedEvent += delegate (object sender, EventArgs e)
            {
                ServerPanel svp = new ServerPanel();
                svp.Show(this);
            };
            menuMainNetwork_Client.ItemClickedEvent += delegate (object sender, EventArgs e)
            {
                ClientPanel cvp = new ClientPanel();
                cvp.Show(this);
            };
            menuMain_Network.MenuItems.Add(menuMainNetwork_Client);
            menuMain_Network.MenuItems.Add(new MenuStripItemSeparator());
            menuMain_Network.MenuItems.Add(menuMain_Network_Server);
            

            menuMain_Select = new MenuStripItem("Edit");
            menuMain_Select.Name = "Edit";

            MenuStripItem miPhysicsSettings = new MenuStripItem("Physics Engine");
            miPhysicsSettings.Name = "Physics Engine";
            miPhysicsSettings.ItemClickedEvent += delegate
            {
                if (null == ((StudioWindow)StudioWindow.Instance).Scene.PhysicsSettings)
                { System.Windows.Forms.MessageBox.Show("No physics world active"); return; }

                Forms.PhysicsSettingsForm physForm = new Forms.PhysicsSettingsForm();
                physForm.Show();
            };
            

            menuMain_Select.MenuItems.Add(miPhysicsSettings);



            miView = new MenuStripItem("View");
            miView.Name = "View";

            miWindow = new MenuStripItem("Window");
            miWindow.Name = "Window";

            miSimulation = new MenuStripItem("Program");
            miSimulation.Name = "Program";

            miSimulation_Crowd = new MenuStripItem("Crowd");
            miSimulation_Crowd.Name = "Crowd";
            miSimulation_Crowd.ItemClickedEvent += delegate
            {
                miSimulation_Crowd.Enabled = false;
                ((StudioWindow)StudioWindow.Instance).createCrowd();
            };

            MenuStripItem miSimulation_Dungeon = new MenuStripItem("Dungeon", true);
            miSimulation_Dungeon.Name = "Dungeon";

            MenuStripItem miSimulation_DungeonAbyss = new MenuStripItem("Abyss");
            miSimulation_DungeonAbyss.Name = "Abyss";
            miSimulation_Dungeon.MenuItems.Add(miSimulation_DungeonAbyss);
            miSimulation_DungeonAbyss.ItemClickedEvent += delegate
            {
                NthStudio.Game.Procedural.DungeonFactory lgen = new Game.Procedural.DungeonFactory(new NthDimension.Procedural.Dungeon.Templates.Abyss.AbyssTemplate());
                //NthDimension.Rendering.Scenegraph.Scene scene = StudioWindow.Instance.Scene;

                Vector3 spawn = this.createDungeon(lgen, 2f, StudioWindow.Instance.Scene);
                if (null != StudioWindow.Instance.Player)
                {
                    //StudioWindow.Instance.Player.Position = spawn;
                    //StudioWindow.Instance.Player.Body.Position = new NthDimension.Physics.LinearMath.JVector(spawn);
                    //StudioWindow.Instance.Player.FirstPersonView.Position = spawn;
                    //StudioWindow.Instance.Player.ThirdPersonView.Position = spawn;

                    StudioWindow.Instance.Scene.SpawnAt_REFACTOR_TODO = spawn;
                    Vector3 res = StudioWindow.Instance.Player.SetPosition(spawn);
                    ConsoleUtil.log(string.Format("Player Spawn at {0} now at {1}", spawn, res));
                }
            };

            MenuStripItem miSimulation_DungeonCave = new MenuStripItem("Cave");
            miSimulation_DungeonCave.Name = "Cave";
            miSimulation_Dungeon.MenuItems.Add(miSimulation_DungeonCave);
            miSimulation_DungeonCave.ItemClickedEvent += delegate
            {
                NthStudio.Game.Procedural.DungeonFactory lgen = new Game.Procedural.DungeonFactory(new NthDimension.Procedural.Dungeon.Templates.Cave.CaveTemplate());
                //NthDimension.Rendering.Scenegraph.Scene scene       = StudioWindow.Instance.Scene;

                Vector3 spawn = this.createDungeon(lgen, 2f, StudioWindow.Instance.Scene);

                if (null != StudioWindow.Instance.Player)
                {
                    //StudioWindow.Instance.Player.Position       = spawn;
                    //StudioWindow.Instance.Player.Body.Position  = new NthDimension.Physics.LinearMath.JVector(spawn);
                    StudioWindow.Instance.Scene.SpawnAt_REFACTOR_TODO = spawn;
                    Vector3 res = StudioWindow.Instance.Player.SetPosition(spawn);
                    ConsoleUtil.log(string.Format("Player Spawn at {0} now at {1}", spawn, res));
                }
            };

            MenuStripItem miSimulation_DungeonLab = new MenuStripItem("Lab");
            miSimulation_DungeonLab.Name = "Lab";
            miSimulation_Dungeon.MenuItems.Add(miSimulation_DungeonLab);
            miSimulation_DungeonLab.ItemClickedEvent += delegate
            {
                NthStudio.Game.Procedural.DungeonFactory lgen = new Game.Procedural.DungeonFactory(new NthDimension.Procedural.Dungeon.Templates.Lab.LabTemplate());
                //NthDimension.Rendering.Scenegraph.Scene scene = StudioWindow.Instance.Scene;

                Vector3 spawn = this.createDungeon(lgen, 2f, StudioWindow.Instance.Scene);

                if (null != StudioWindow.Instance.Player)
                {
                    //StudioWindow.Instance.Player.Position = spawn;
                    //StudioWindow.Instance.Player.Body.Position = new NthDimension.Physics.LinearMath.JVector(spawn);
                    StudioWindow.Instance.Scene.SpawnAt_REFACTOR_TODO = spawn;
                    Vector3 res = StudioWindow.Instance.Player.SetPosition(spawn);
                    ConsoleUtil.log(string.Format("Player Spawn at {0} now at {1}", spawn, res));
                }
            };

            MenuStripItem miSimulation_Floor = new MenuStripItem("Floor-plan");
            miSimulation_Floor.Name = "Floor";
            miSimulation_Floor.ItemClickedEvent += delegate
            {
                FloorFactory fg = new FloorFactory();
            };

            MenuStripItem miSimulation_ProceduralPlanet = new MenuStripItem("Planet (6xTerrain)");
            miSimulation_ProceduralPlanet.Name = "Planet";
            miSimulation_ProceduralPlanet.ItemClickedEvent += delegate
            {
                NthDimension.Procedural.SolarGenerator solar = new NthDimension.Procedural.SolarGenerator(ApplicationBase.Instance.Scene);
            };

            MenuStripItem miSimulation_Terrain = new MenuStripItem("Terrain");
            miSimulation_Terrain.Name = "Terrain";
            miSimulation_Terrain.ItemClickedEvent += delegate
            {
                float size = 2048;
                int subdv = 32;
                float heightScale = 2.0f;
                float textureScale = 1.0f;
                Terrain terrain = new Terrain(size, size, //4096, 4096, //512, 512  //1024, 1024,
                                              subdv, subdv,  //128, 128,
                                              @"D:\terrain\heightmap.png", heightScale, //350.00f,
                                              @"D:\terrain\colormap.png", textureScale)
                {
                    Size = new Vector3(1f, 1f, 1f)
                };
                
                ApplicationBase.Instance.Scene.AddDrawable(terrain);
                ApplicationBase.Instance.Scene.CreatePhysics(terrain, new ContactSettings()
                {
                    AllowedPenetration = .01f,
                    BiasFactor = .001f,
                    BreakThreshold = .01f,
                    MaximumBias = 10.0f,
                    MinimumVelocity = .01f
                });                

            };

            MenuStripItem miSimulation_City             = new MenuStripItem("City") { Name = "City" };
            MenuStripItem miSimulation_Forest           = new MenuStripItem("Forest", true) { Name = "Forest" };
            MenuStripItem miSimulation_ForestTree       = new MenuStripItem("Tree") { Name = "Tree" };
            miSimulation_ForestTree.ItemClickedEvent    += delegate
            {

            };
            miSimulation_Forest.MenuItems.Add(miSimulation_ForestTree);
            MenuStripItem miSimulation_Road = new MenuStripItem("Road") { Name = "Road" };
            miSimulation_Road.ItemClickedEvent += delegate
            {
                ApplicationBase.Instance.Scene.AddDrawable(
                    new RoadModel(
                        new Vector2[]
                        {
                            new Vector2(1.74f, 1.93f),
                            new Vector2(23.9f, 6.26f),
                            new Vector2(55.7f, 14.6f),
                            new Vector2(82.6f, 55f)
                        },
                        5,
                        2,
                        @"road\road.xmf",
                        3.5f));
                
            };

            miSimulation.MenuItems.Add(miSimulation_Terrain);
            miSimulation.MenuItems.Add(miSimulation_ProceduralPlanet);
            miSimulation.MenuItems.Add(miSimulation_Forest);
            miSimulation.MenuItems.Add(miSimulation_City);
            miSimulation.MenuItems.Add(miSimulation_Road);
            miSimulation.MenuItems.Add(miSimulation_Dungeon);
            miSimulation.MenuItems.Add(miSimulation_Floor);
            miSimulation.MenuItems.Add(miSimulation_Crowd);

            menuMain.Widgets.Add(menuMain_ProjectScene);

            menuMain.Widgets.Add(menuMain_Select);

            menuMain.Widgets.Add(miView);

            menuMain.Widgets.Add(miWindow);

            menuMain.Widgets.Add(miSimulation);

            menuMain.Widgets.Add(menuMain_Network);

            var miExit = new MenuStripItem("Exit", true);
            menuMain_ProjectScene.MenuItems.Add(miExit);

            miExit.ItemClickedEvent += delegate
            {
                StudioWindow.Instance.exitGame();
            };

            var miUndo = new MenuStripItem("Undo", true);
            miUndo.Name = "Undo";
            var miRedo = new MenuStripItem("Redo", true);
            miRedo.Name = "Redo";
            menuMain_Select.MenuItems.Add(miUndo);
            menuMain_Select.MenuItems.Add(miRedo);

            #region Window
            var miwindowmap = new MenuStripItem("World Map");
            miwindowmap.Name = "World Map";
            miWindow.MenuItems.Add(miwindowmap);
            miwindowmap.ItemClickedEvent += delegate
            {
                new WorldMapViewer().Show(this);
            };

            MenuStripItem miWindowMusicPlayer = new MenuStripItem("Mp3 player");
            miWindowMusicPlayer.Name = "Mp3 player";
            miWindow.MenuItems.Add(miWindowMusicPlayer);
            miWindowMusicPlayer.ItemClickedEvent += delegate
            {
                new MusicPlayer().Show(this);
            };

            miWindowAssets = new MenuStripItem("Assets Viewer");
            miWindowAssets.Name = "Assets Viewer";
            miWindow.MenuItems.Add(miWindowAssets);
            miWindowAssets.ItemClickedEvent += delegate
            {
                new AssetsViewer().Show(this);
            };

            MenuStripItem miWindowTimeline = new MenuStripItem("Keyframes");
            miWindowTimeline.Name = "Keyframes";
            miWindow.MenuItems.Add(miWindowTimeline);
            miWindowTimeline.ItemClickedEvent += delegate
            {
                new KeyFrameView()
                {
                    Location = new Point(this.Width / 2 - Size.Width / 2, this.Height / 2 - Size.Height / 2),
                }.Show(this);
            };

            miWindowMaterials = new MenuStripItem("Material");
            miWindowMaterials.Name = "Material";
            miWindow.MenuItems.Add(miWindowMaterials);
            miWindowMaterials.ItemClickedEvent += delegate
            {
                //new NodeGraphDialog().Show(this);
                new NodeGraph.NodeGraphWindow()
                {
                    Size = new Size(StudioWindow.Instance.ClientRectangle.Width - 50,
                                    ClientRect.Height - 50),
                    Location = new Point(this.Width / 2 - Size.Width / 2, this.Height / 2 - Size.Height / 2),
                    //VisibleAfterClipping = true
                }
                .Show(this);
            };

            miWindowDotNet = new MenuStripItem("Source Code", true);
            miWindowDotNet.Name = "Source Code";
            miWindow.MenuItems.Add(miWindowDotNet);

            //miWindowDotNet.MouseClickEvent += delegate
            //{
            //    // TODO:: Display Window
            //};
           
            MenuStripItem miWindowSourceCS = new MenuStripItem("C# Interactive (64-bit)");
            miWindowSourceCS.Name = "C# Interactive (64-bit)";
            miWindowDotNet.MenuItems.Add(miWindowSourceCS);
            miWindowSourceCS.ItemClickedEvent += delegate 
            {
                CodeViewer cdv = new CodeViewer();
                cdv.Show(WindowHUD);
            };

            miWindowDotNet.MenuItems.Add(new MenuStripItemSeparator());

            MenuStripItem miWindowSourceGLSL = new MenuStripItem("GLSL Shader");
            miWindowSourceGLSL.Name = "GLSL Shader";
            miWindowDotNet.MenuItems.Add(miWindowSourceGLSL);
            miWindowSourceGLSL.ItemClickedEvent += delegate { };



            #region Framebuffers
            miWindow_FBO = new MenuStripItem("Framebuffers", true);
            miWindow_FBO.Name = "Framebuffers";
            {
                miWindow_FBOView = new MenuStripItem("Output (main)");
                miWindow_FBOView.Name = "Framebuffer Composite";
                miWindow_FBOView.ItemClickedEvent += delegate
                {
                    try { 
                    new FboViewer("Composite", StudioWindow.Instance.FBO_Scene.OutputFb.ColorTexture).Show(this);
                    }
                    catch
                    {
                        if (null == ApplicationBase.Instance.Scene)
                        {
                            MessageDialog msgDlg = new MessageDialog() { Title = "No active scene(s)" };
                            msgDlg.Show(this);
                            return;
                        }
                    }
                };


                var miWindow_FBONormalsView = new MenuStripItem("SS Normals");
                miWindow_FBONormalsView.Name = "framebuffer normals";
                miWindow_FBONormalsView.ItemClickedEvent += delegate
                {
                    try { 
                    new FboViewer("SSNormals", StudioWindow.Instance.FBO_Scene.ScreenSpaceNormalFb.ColorTexture).Show(this);
                    }
                    catch
                    {
                        if (null == ApplicationBase.Instance.Scene)
                        {
                            MessageDialog msgDlg = new MessageDialog() { Title = "No active scene(s)" };
                            msgDlg.Show(this);
                            return;
                        }
                    }
                };

                var miWindow_FBOSunView = new MenuStripItem("Sunlight ", false, "Ctrl-L");
                miWindow_FBOSunView.Name = "framebuffer sunlight";
                miWindow_FBOSunView.ItemClickedEvent += delegate
                {
                    try
                    {
                        new FboViewer("PSSM 2/4 (near far)", StudioWindow.Instance.Scene.SunFrameBufferFar1.DepthTexture, true).Show(this);
                    }
                    catch
                    {
                        if (null == ApplicationBase.Instance.Scene)
                        {
                            MessageDialog msgDlg = new MessageDialog() { Title = "No active scene(s)" };
                            msgDlg.Show(this);
                            return;
                        }
                    }
                };

                var miWindow_FBOSunInnerView = new MenuStripItem("Sunlight Inner (Disabled)", false, "Ctrl-Shift-L");
                miWindow_FBOSunInnerView.Name = "sunlight inner";
                miWindow_FBOSunInnerView.ItemClickedEvent += delegate
                {
                    try { 
                    new FboViewer("PSSM 1/4 (near)", StudioWindow.Instance.Scene.SunFrameBufferNear.DepthTexture, true).Show(this);
                    }
                    catch
                    {
                        if (null == ApplicationBase.Instance.Scene)
                        {
                            MessageDialog msgDlg = new MessageDialog() { Title = "No active scene(s)" };
                            msgDlg.Show(this);
                            return;
                        }
                    }
                };

                var miWindow_FBOShadowView = new MenuStripItem("Shadow");
                miWindow_FBOShadowView.Name = "Shadow";
                miWindow_FBOShadowView.ItemClickedEvent += delegate
                {
                    try { 
                    new FboViewer("Shadow map", StudioWindow.Instance.FBO_Shadow.DepthTexture, true).Show(this);
                    }
                    catch
                    {
                        if (null == ApplicationBase.Instance.Scene)
                        {
                            MessageDialog msgDlg = new MessageDialog() { Title = "No active scene(s)" };
                            msgDlg.Show(this);
                            return;
                        }
                    }
                };

                var miWindow_FBOLightmapView = new MenuStripItem("Light");
                miWindow_FBOLightmapView.Name = "light";
                miWindow_FBOLightmapView.ItemClickedEvent += delegate
                {
                    try
                    {
                        new FboViewer("Light map", StudioWindow.Instance.FBO_Scene.LightFramebuffer.DepthTexture, true).Show(this);
                    }
                    catch
                    {
                        if (null == ApplicationBase.Instance.Scene)
                        {
                            MessageDialog msgDlg = new MessageDialog() { Title = "No active scene(s)" };
                            msgDlg.Show(this);
                            return;
                        }
                    }

                };

                var miWindow_FBOReflectionView = new MenuStripItem("Reflection");
                miWindow_FBOReflectionView.Name = "reflection";
                miWindow_FBOReflectionView.ItemClickedEvent += delegate
                {
                    try { 
                    new FboViewer("Reflection map", StudioWindow.Instance.FBO_Scene.ReflectionFramebuffer.ColorTexture).Show(this);
                    }
                    catch
                    {
                        if (null == ApplicationBase.Instance.Scene)
                        {
                            MessageDialog msgDlg = new MessageDialog() { Title = "No active scene(s)" };
                            msgDlg.Show(this);
                            return;
                        }
                    }
                };

                var miWindow_FBOLightmapBlurView = new MenuStripItem("Light Blur"); // Lighblur
                miWindow_FBOLightmapBlurView.Name = "lightmapBlur";
                miWindow_FBOLightmapBlurView.ItemClickedEvent += delegate
                {
                    try { 
                    new FboViewer("Light map blur", StudioWindow.Instance.FBO_Scene.LightBlurFramebuffer.ColorTexture).Show(this);
                    }
                    catch
                    {
                        if (null == ApplicationBase.Instance.Scene)
                        {
                            MessageDialog msgDlg = new MessageDialog() { Title = "No active scene(s)" };
                            msgDlg.Show(this);
                            return;
                        }
                    }
                };

                var miWindow_FBOSceneView = new MenuStripItem("Scene");
                miWindow_FBOSceneView.Name = "scene";
                miWindow_FBOSceneView.ItemClickedEvent += delegate
                {
                    try { 
                    new FboViewer("Scene", StudioWindow.Instance.FBO_Scene.SceneFramebuffer.ColorTexture).Show(this);
                    }
                    catch
                    {
                        if (null == ApplicationBase.Instance.Scene)
                        {
                            MessageDialog msgDlg = new MessageDialog() { Title = "No active scene(s)" };
                            msgDlg.Show(this);
                            return;
                        }
                    }
                };

                //var miWindow_FBOSceneDefInfoView = new MenuStripItem("Scene (deffered)");
                //miWindow_FBOSceneDefInfoView.Name = "defInfo";
                //miWindow_FBOSceneDefInfoView.ItemClickedEvent += delegate
                //{
                //    new FboViewer(StudioWindow.Instance.FBO_Scene.SceneDefInfoFb.ColorTexture, true).Show(this);
                //};

                var miWindow_FBOSceneBackdropView = new MenuStripItem("Scene (backdrop)");
                miWindow_FBOSceneBackdropView.Name = "backdrop";
                miWindow_FBOSceneBackdropView.ItemClickedEvent += delegate
                {
                    try { 
                    new FboViewer("Scene backdrop", StudioWindow.Instance.FBO_Scene.SceneBackdropFb.ColorTexture).Show(this);
                    }
                    catch(Exception fboE)
                    {
                        if (null == ApplicationBase.Instance.Scene)
                        {
                            MessageDialog msgDlg = new MessageDialog() { Title = "No active scene(s)" };
                            msgDlg.Show(this);
                            return;
                        }
                    }
                };


                // TODO:: Scene->Skybox foreach CBOSet_Sky.FrameBufferSets[CurrentCBOSide] 

                miWindow_FBO.MenuItems.Add(miWindow_FBOSunView);
                miWindow_FBO.MenuItems.Add(miWindow_FBOSunInnerView);
                miWindow_FBO.MenuItems.Add(miWindow_FBOShadowView);
                miWindow_FBO.MenuItems.Add(miWindow_FBOLightmapView);
                miWindow_FBO.MenuItems.Add(miWindow_FBOLightmapBlurView);
                miWindow_FBO.MenuItems.Add(new MenuStripItemSeparator());
                miWindow_FBO.MenuItems.Add(miWindow_FBONormalsView);
                miWindow_FBO.MenuItems.Add(miWindow_FBOReflectionView);
                miWindow_FBO.MenuItems.Add(new MenuStripItemSeparator());
                miWindow_FBO.MenuItems.Add(miWindow_FBOView);
                miWindow_FBO.MenuItems.Add(miWindow_FBOSceneView);
                //miWindow_FBO.MenuItems.Add(miWindow_FBOSceneDefInfoView);
                miWindow_FBO.MenuItems.Add(miWindow_FBOSceneBackdropView);
                //miWindow_FBO.MenuItems.Add();

                #region todo create material
                //Label lbVer = new Label("NthDimension 2018 Rev.0.2a");
                //lbVer.Size = new Size(100, 24);
                //lbVer.Location = new Point(200, 0);
                //lbVer.Anchor = EAnchorStyle.Right;

                //menuMain.Widgets.Add(lbVer);


                //miFileOpen = new MenuStripItem("Open", true);
                //miFileClose = new MenuStripItem("Close", true);

                //menuMain_Project.MenuItems.Add(miFileOpen);
                //menuMain_Project.MenuItems.Add(miFileClose);

                //miOpenFile = new MenuStripItem("File", true);
                //pCentralManager.MsiOpenFile = miOpenFile;
                //miOpenProSol = new MenuStripItem("Project/Solution", true);
                //miFileOpen.MenuItems.Add(miOpenFile);
                //miFileOpen.MenuItems.Add(miOpenProSol);
                //miCloseFile = new MenuStripItem("File", true);
                //pCentralManager.MsiCloseFile = miCloseFile;
                //pCentralManager.MsiOpenProSolFile = miOpenProSol;

                //miCloseProSol = new MenuStripItem("Solution", true);
                //miFileClose.MenuItems.Add(miCloseFile);
                //miFileClose.MenuItems.Add(miCloseProSol);

                //var miNewFile = new MenuStripItem("Source", true);
                //var miNewFileItem1 = new MenuStripItem("C# Project", true);
                //var miNewFileItem2 = new MenuStripItem("C# Script", true);
                //miNewFile.MenuItems.Add(miNewFileItem1);
                //miNewFile.MenuItems.Add(miNewFileItem2);

                //var miNewModel      = new MenuStripItem("Model", true);
                //var miNewMaterial   = new MenuStripItem("Material", true);
                //menuMain_ProjectNew.MenuItems.Add(miNewMaterial);
                //menuMain_ProjectNew.MenuItems.Add(miNewModel);
                //var miNewTexture                    = new MenuStripItem("Texture");
                //var miNewTextureDiffuse             = new MenuStripItem("Diffuse (base)");
                //var miNewTextureSSNormal            = new MenuStripItem("Normal (SSN)");
                //var miNewTextureAmbientOcclusion    = new MenuStripItem("Ambient Occlusion (AO)");
                //var miNewTextureCavity              = new MenuStripItem("Cavity");
                //var miNewTextureSpecular            = new MenuStripItem("Specular");
                //var miNewTextureEmissive            = new MenuStripItem("Emissive");
                //var miNewTextureShininess           = new MenuStripItem("Metallic");
                //miNewTexture.MenuItems.Add(miNewTextureDiffuse);
                //miNewTexture.MenuItems.Add(miNewTextureSSNormal);
                //miNewTexture.MenuItems.Add(miNewTextureAmbientOcclusion);
                //miNewTexture.MenuItems.Add(miNewTextureCavity);
                //miNewTexture.MenuItems.Add(miNewTextureSpecular);
                //miNewTexture.MenuItems.Add(miNewTextureEmissive);
                //miNewTexture.MenuItems.Add(miNewTextureShininess);
                //menuMain_ProjectNew.MenuItems.Add(miNewTexture);



                //var miSep1 = new MenuStripItemSeparator();
                //menuMain_Project.MenuItems.Add(miSep1);

                ////var miFileSave = new MenuStripItem("Save", true);
                //pCentralManager.MsiSaveFile = miFileSave;
                ////var miFileSaveAs = new MenuStripItem("Save as...", true);
                ////var miFileSaveAll = new MenuStripItem("Save all", true);
                //menuMain_Project.MenuItems.Add(miFileSave);
                //menuMain_Project.MenuItems.Add(miFileSaveAs);
                //menuMain_Project.MenuItems.Add(miFileSaveAll);


                ////var miSep2 = new MenuStripItemSeparator();
                ////menuMain_Project.MenuItems.Add(miSep2);
                #endregion

            }
            #endregion Framebuffers

            miWindow.MenuItems.Add(miWindow_FBO);
#endregion



        }
        #endregion

        public void SetProgress(float perCent, string text = "")
        {
            this.m_statusLabel.ProgressPerCent = perCent;
            if (!String.IsNullOrEmpty(text))
                this.m_statusLabel.Text = text;
        }

        private Vector3 createDungeon(DungeonFactory lgen, float scaleFactor, NthDimension.Rendering.Scenegraph.SceneGame scene)
        {
            lgen.FurnishAllRooms();

            foreach (var d in roomModels)
            {
                scene.RemoveDrawable(d);
                scene.Models.Remove(d);
            }

            if (lgen.Rooms.Count <= 0) return new Vector3();

            float roomHeight = 10f;
            Matrix3 scaMat = Matrix3.CreateScale(scaleFactor);
            NthDimension.Procedural.Dungeon.Rasterizer rs = new NthDimension.Procedural.Dungeon.Rasterizer(rand.Next(), lgen.Graph);


            #region Rooms
            foreach (var r in lgen.Rooms)
            {
                Vector3 pos = Vector3.Transform(new Vector3(r.Pos.X, 0, r.Pos.Y), scaMat);
                Vector3 sca = Vector3.Transform(new Vector3(r.Width / scaleFactor, roomHeight / scaleFactor, r.Length / scaleFactor), scaMat);

                RoomModel modelRoom = new RoomModel(scene, r.Width, r.Length, (int)roomHeight);

                if (r == lgen.Rooms[0])
                    modelRoom.setMaterial(@"uvboxred.xmf");
                else if (r == lgen.Rooms[lgen.Rooms.Count - 1])
                    modelRoom.setMaterial(@"uvboxgreen.xmf");
                else
                    modelRoom.setMaterial(@"uvbox.xmf");

                modelRoom.setMesh(@"base\unitboxinv_1mm.obj");
                modelRoom.Position = pos;
                modelRoom.Size = sca;
                modelRoom.Renderlayer = NthDimension.Rendering.Drawables.Drawable.RenderLayer.Solid;
                modelRoom.IgnoreCulling = true;
                modelRoom.IsVisible = true;
                scene.Models.Add(modelRoom);
                modelRoom.Scene = scene;

                roomModels.Add(modelRoom);

                ////var edges = r.Edges;
                ////edges[0].RoomA
                //if(r.Edges.Count > 0)
                //{
                //    ConsoleUtil.log(string.Format(" -Room Edge Depth {0}\tLength {1}\tEdges {2}\tBranches {3}", r.Depth, r.Length, r.Edges.Count, r.NumBranches));

                //    foreach(var e in r.Edges)
                //    {
                //        e.RoomA.
                //    }

                //}
                foreach (var edge in r.Edges)
                {
                    if (edge.RoomA != r)
                        continue;
                    RasterizeCorridor(scene, /* new LevelMapCorridor(),*/ edge, scaMat, scaleFactor);
                }
            }
            #endregion Rooms

            var map = rs.ExportMap();
            int rank = map.Rank;

          

            //foreach (var m in map)
            //    ConsoleUtil.log(string.Format("\tTile: {0} {1} {2}", m.Region, m.TileType, m.Object.ObjectType));

            return Vector3.Transform(new Vector3(roomModels[0].Position.X, 0f, roomModels[0].Position.Y), scaMat);
        }
        void RasterizeCorridor(NthDimension.Rendering.Scenegraph.SceneGame scene, /*LevelMapCorridor corridor,*/ Edge edge, Matrix3 scale, float scaleFactor)
        {
            Vector3 srcPos, dstPos;
            createCorridor(edge.RoomA, edge.RoomB, out srcPos, out dstPos);
            //corridor.Rasterize(edge.RoomA, edge.RoomB, srcPos, dstPos);


            Vector3 src = Vector3.Transform(new Vector3(srcPos.X, 0, srcPos.Z), scale);
            Vector3 dst = Vector3.Transform(new Vector3(dstPos.X, 0, dstPos.Z), scale);

            int width = (int)Math.Abs((decimal)(dst.X - src.X));
            if (width < 3)
                width = 3;
            int length = (int)Math.Abs((decimal)(dst.Z - src.Z));
            if (length < 3)
                length = 3;
            int height = 10;

            Vector3 sca = new Vector3(width / scaleFactor, height / scaleFactor, length / scaleFactor);

            RoomModel modelRoom = new RoomModel(scene, width, length, 10);
            Vector3 pPos = Vector3.Transform(new Vector3(edge.RoomA.Pos.X, 0, edge.RoomA.Pos.Y), scale);

            modelRoom.setMaterial(@"uvboxyellow.xmf");

            modelRoom.setMesh(@"base\unitboxinv_1mm.obj");
            modelRoom.Position = pPos + (dst - src) / 2;
            modelRoom.Size = sca;
            modelRoom.Renderlayer = NthDimension.Rendering.Drawables.Drawable.RenderLayer.Solid;
            modelRoom.IgnoreCulling = true;
            modelRoom.IsVisible = true;
            scene.Models.Add(modelRoom);
            modelRoom.Scene = scene;

            roomModels.Add(modelRoom);


        }
        //void RasterizeCorridors()
        //{
        //    var corridor = graph.Template.CreateCorridor();
        //    corridor.Init(rasterizer, graph, rand);

        //    foreach (var room in graph.Rooms)
        //        foreach (var edge in room.Edges)
        //        {
        //            if (edge.RoomA != room)
        //                continue;
        //            RasterizeCorridor(corridor, edge);
        //        }
        //}
        void createCorridor(LevelRoom src, LevelRoom dst, out Vector3 srcPos, out Vector3 dstPos)
        {
            var edge = src.Edges.Single(ed => ed.RoomB == dst);
            var link = edge.Linkage;

            if (link.Direction == NthDimension.Procedural.Dungeon.Direction.South || link.Direction == NthDimension.Procedural.Dungeon.Direction.North)
            {
                srcPos = new Vector3(link.Offset, 0, src.Pos.Y + src.Length / 2);
                dstPos = new Vector3(link.Offset, 0, dst.Pos.Y + dst.Length / 2);
            }
            else if (link.Direction == NthDimension.Procedural.Dungeon.Direction.East || link.Direction == NthDimension.Procedural.Dungeon.Direction.West)
            {
                srcPos = new Vector3(src.Pos.X + src.Width / 2, 0, link.Offset);
                dstPos = new Vector3(dst.Pos.X + dst.Width / 2, 0, link.Offset);
            }
            else
                throw new ArgumentException();
        }

        //public void CreateTexture()
        //{
        //    FBOPic = new Picture(StudioWindow.Instance.FBOSet_Main.SceneFramebuffer.ColorTexture);
        //    FBOPic.Size = new Size(256, 256);
        //    FBOPic.Location = new Point(0, 50);

        //    Widgets.Add(FBOPic);
        //}
    }
}
