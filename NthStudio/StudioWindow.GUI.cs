using NthDimension.Algebra;
using NthDimension.Forms;
using NthDimension.Physics.Dynamics;
using NthDimension.Physics.LinearMath;
using NthDimension.Rasterizer.NanoVG;
using NthDimension.Rasterizer.Windows;
using NthDimension.Rendering;
using NthDimension.Rendering.Utilities;
using NthStudio.Gui.Displays;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NthStudio.Gui.Displays.NanoGContext;

namespace NthStudio
{
    public partial class StudioWindow
    {
        public static bool DrawPerformance = false;

        class MouseCursor3D
        {
            public Vector2i oldmouse;
            public Vector3 mouse3d;
            public Vector3 X0mouse;
            public Vector3 X1mouse;
            public Vector3 Y0mouse;
            public Vector3 Y1mouse;
            public Vector3 Z0mouse;
            public Vector3 Z1mouse;
            public MouseCursor3D()
            {
                oldmouse = new Vector2i();
                mouse3d = new Vector3();
                X0mouse = new Vector3();
                X1mouse = new Vector3();
                Y0mouse = new Vector3();
                Y1mouse = new Vector3();
                Z0mouse = new Vector3();
                Z1mouse = new Vector3();
            }
        }

        private MouseCursor3D mouseCursor3d = new MouseCursor3D();
        private void debugDrawDesignAxes(int mouseX, int mouseY)
        {
            Vector2i imouse = new Vector2i(mouseX, mouseY);
            Vector3 mouse = new Vector3();

            if (mouseCursor3d.oldmouse != imouse)
            {

                mouseCursor3d.oldmouse = new Vector2i(mouseX, mouseY);

                mouse = Project(new Vector2(imouse.X, imouse.Y), Player.ViewInfo);

                if (float.IsInfinity(mouse.X) ||
                    float.IsInfinity(mouse.Y) ||
                    float.IsInfinity(mouse.Z)) { mouse = mouseCursor3d.mouse3d; }

                float axisLength = 2f;

                mouseCursor3d.X0mouse = new Vector3(mouse.X - axisLength, mouse.Y, mouse.Z);
                mouseCursor3d.X1mouse = new Vector3(mouse.X + axisLength, mouse.Y, mouse.Z);

                mouseCursor3d.Y0mouse = new Vector3(mouse.X, mouse.Y - axisLength, mouse.Z);
                mouseCursor3d.Y1mouse = new Vector3(mouse.X, mouse.Y + axisLength, mouse.Z);

                mouseCursor3d.Z0mouse = new Vector3(mouse.X, mouse.Y, mouse.Z - axisLength);
                mouseCursor3d.Z1mouse = new Vector3(mouse.X, mouse.Y, mouse.Z + axisLength);

            }

            Vector2 x0 = UnProject(mouseCursor3d.X0mouse, Player.ViewInfo.modelviewMatrix, Player.ViewInfo.projectionMatrix, Width, Height);
            Vector2 x1 = UnProject(mouseCursor3d.X1mouse, Player.ViewInfo.modelviewMatrix, Player.ViewInfo.projectionMatrix, Width, Height);
            debug_aid_line(x0.X, x0.Y, x1.X, x1.Y, NanoVG.nvgRGBA(0, 255, 0, 70), 1f);

            Vector2 z0 = UnProject(mouseCursor3d.Z0mouse, Player.ViewInfo.modelviewMatrix, Player.ViewInfo.projectionMatrix, Width, Height);
            Vector2 z1 = UnProject(mouseCursor3d.Z1mouse, Player.ViewInfo.modelviewMatrix, Player.ViewInfo.projectionMatrix, Width, Height);
            debug_aid_line(z0.X, z0.Y, z1.X, z1.Y, NanoVG.nvgRGBA(255, 0, 0, 70), 1f);

            Vector2 y0 = UnProject(mouseCursor3d.Y0mouse, Player.ViewInfo.modelviewMatrix, Player.ViewInfo.projectionMatrix, Width, Height);
            Vector2 y1 = UnProject(mouseCursor3d.Y1mouse, Player.ViewInfo.modelviewMatrix, Player.ViewInfo.projectionMatrix, Width, Height);
            debug_aid_line(y0.X, y0.Y, y1.X, y1.Y, NanoVG.nvgRGBA(0, 0, 255, 70), 1f);

            mouseCursor3d.mouse3d = mouse;

            NthDimension.Rendering.Utilities.ConsoleUtil.log(string.Format("Mouse X: {0} Point3D: {1}", imouse, mouse), false);

            Mouse3DPickResult res = PickMouseCursor3D(mouseX, mouseY, Player.ViewInfo.projectionMatrix, Player.ViewInfo.modelviewMatrix);

            string printCoords = res.Body == null ? string.Format(mouseCursor3d.mouse3d.ToString("###0.###")) :
                                                    string.Format("{1} {0}", res.Body.Tag is ApplicationObject ? ((ApplicationObject)res.Body.Tag).Name : res.Body.Tag, res.HitCoordinates.ToString("###0.###"));

            NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(255, 255, 255, 255));
            NanoVG.nvgTextAlign(vg, (int)(EAlign.ALIGN_LEFT | EAlign.ALIGN_MIDDLE));
            float txtX = mouseX + 15f; //- (x1.X - x0.X) / 2f;
            float txtY = mouseY + 25f; //+ (y1.Y - y0.Y) / 2f;
            NanoVG.nvgText(vg, txtX, txtY, printCoords);
        }
        class Mouse3DPickResult
        {
            public RigidBody Body;
            public Vector3 HitCoordinates;

            public Mouse3DPickResult(RigidBody body, Vector3 hitCoordinates)
            {
                this.Body = body;
                this.HitCoordinates = hitCoordinates;
            }
        }
        private Mouse3DPickResult PickMouseCursor3D(int mouseX, int mouseY, Matrix4 projection, Matrix4 modelview)
        {
            Point p = new Point(mouseX, mouseY);
            Point biasp = new Point(p.X,
                                    p.Y);   // TODO:: Y appears lower than cursor

            Vector3 v1 = NthDimension.Rendering.Utilities.RayHelper.UnProject(projection,
                                                modelview,
                                                new Size(ApplicationBase.Instance.Width, ApplicationBase.Instance.Height),
                                                new Vector3(biasp.X, biasp.Y, -1.5f));
            //new Vector3(biasp.X, biasp.Y, 0f));

            Vector3 v2 = NthDimension.Rendering.Utilities.RayHelper.UnProject(projection,
                                                modelview,
                                                new Size(ApplicationBase.Instance.Width, ApplicationBase.Instance.Height),
                                                new Vector3(biasp.X, biasp.Y, 1f));

            RigidBody body; JVector normal; float frac;

            bool result = Scene.CollisionRaycast(GenericMethods.FromOpenTKVector(v1), GenericMethods.FromOpenTKVector(v2),
                null, out body, out normal, out frac);

            Vector3 hitCoords = v1 + v2 * frac;

            if (result)
                return new Mouse3DPickResult(body, hitCoords);

            return new Mouse3DPickResult(null, new Vector3());
        }

        //private void drawAvatarInfo(PlayerModel model, UserInfoDesc info)
        //{
        //    //if (null != AvatarSelection)
        //    //    if (AvatarSelection.Info.UserId == info.UserId) return;

        //    //if (null != model)
        //    //{
        //    //    Vector3 topVertex = model.GetTopVertex();

        //    //    Matrix4 scl = Matrix4.CreateScale(model.Size);
        //    //    scl.TransformVector(ref topVertex);

        //    //    Vector3 avatarPos = Vector3.Zero;
        //    //    Vector3 avatarTopVertex = model.Position + topVertex;
        //    //    Vector3 avatarPoint = new Vector3(avatarPos.X + avatarTopVertex.X,
        //    //                                      avatarPos.Y + avatarTopVertex.Y,
        //    //                                      avatarPos.Z + avatarTopVertex.Z);

        //    //    Vector2 infoPos = UnProject(avatarPoint, Player.ViewInfo.modelviewMatrix, Player.ViewInfo.projectionMatrix, this.Width, this.Height);
        //    //    infoPos.Y += 15;

        //    //    NanoVG.nvgSave(vg);

        //    //    #region NanoVg draw
        //    //    NanoVG.nvgScissor(vg, this.m_screen.WorkspaceRectangle.X, this.m_screen.WorkspaceRectangle.Y,
        //    //                          this.m_screen.WorkspaceRectangle.Width, this.m_screen.WorkspaceRectangle.Height);



        //    //    string avatarName = string.Format("{0} {1}", info.FirstName, info.LastName);
        //    //    var bounds = new float[4];

        //    //    NanoVG.nvgFontSize(vg, 20);
        //    //    NanoVG.nvgFontFace(vg, "PlayRegular");
        //    //    NanoVG.nvgTextBounds(vg, 0, 0, avatarName, bounds);

        //    //    var s = new Size((int)(bounds[2] - bounds[0]), (int)(bounds[3] - bounds[1]));

        //    //    NanoVG.nvgBeginPath(vg);
        //    //    NanoVG.nvgRoundedRect(vg, infoPos.X - s.Width / 2 - 5, this.Height - infoPos.Y - s.Height / 2 - 3, s.Width + 10, s.Height + 5, 5f);
        //    //    NanoVG.nvgClosePath(vg);

        //    //    NVGcolor print = vrColorUnselected;//vrColorDefault;

        //    //    //if (info.Gender.ToLower() == "male") print = vrColorMale;
        //    //    //if (info.Gender.ToLower() == "female") print = vrColorFemale;

        //    //    NanoVG.nvgFillColor(vg, print);
        //    //    NanoVG.nvgFill(vg);

        //    //    NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(255, 255, 255, 255));
        //    //    NanoVG.nvgTextAlign(vg, (int)(EAlign.ALIGN_LEFT | EAlign.ALIGN_MIDDLE));
        //    //    NanoVG.nvgText(vg, infoPos.X - s.Width / 2, this.Height - infoPos.Y, avatarName);
        //    //    #endregion

        //    //    NanoVG.nvgRestore(vg);
        //    //}
        //}
        //private void drawSelectedAvatarInfo()
        //{
        //    if (null != AvatarSelection)
        //    {
        //        int idExists = Scene.RemotePlayers.FindIndex(r => r.UserId == AvatarSelection.Info.UserId);
        //        if (idExists < 0)
        //        {
        //            AvatarSelection = null;
        //            return;
        //        }

        //        Vector3 topVertex = AvatarSelection.Model.GetTopVertex();

        //        Matrix4 scl = Matrix4.CreateScale(AvatarSelection.Model.Size);
        //        scl.TransformVector(ref topVertex);

        //        Vector3 avatarPos = Vector3.Zero;
        //        Vector3 avatarTopVertex = AvatarSelection.Model.Position + topVertex;
        //        Vector3 avatarPoint = new Vector3(avatarPos.X + avatarTopVertex.X,
        //                                          avatarPos.Y + avatarTopVertex.Y,
        //                                          avatarPos.Z + avatarTopVertex.Z);

        //        int yMargin = 15;

        //        Vector2 infoPos = Convert(avatarPoint, Player.ViewInfo.modelviewMatrix, Player.ViewInfo.projectionMatrix, this.Width, this.Height);
        //        infoPos.Y += yMargin;
        //        NanoVG.nvgSave(vg);
        //        NanoVG.nvgScissor(vg, this.m_screen.WorkspaceRectangle.X, this.m_screen.WorkspaceRectangle.Y,
        //            this.m_screen.WorkspaceRectangle.Width, this.m_screen.WorkspaceRectangle.Height);

        //        string avatarName = string.Format("{0} {1}", AvatarSelection.Info.FirstName, AvatarSelection.Info.LastName);
        //        var bounds = new float[4];

        //        NanoVG.nvgFontSize(vg, 20);
        //        NanoVG.nvgFontFace(vg, "PlayRegular");
        //        NanoVG.nvgTextBounds(vg, 0, 0, avatarName, bounds);

        //        var s = new Size((int)(bounds[2] - bounds[0]), (int)(bounds[3] - bounds[1]));

        //        // Background
        //        NanoVG.nvgBeginPath(vg);
        //        NanoVG.nvgRoundedRect(vg, infoPos.X - s.Width / 2 - 5, this.Height - infoPos.Y - s.Height / 2 - 3, s.Width + 10, s.Height + 5, 5f);
        //        NanoVG.nvgClosePath(vg);
        //        NVGcolor print = vrColorDefault;
        //        if (AvatarSelection.Info.Gender.ToLower() == "male") print = vrColorMale;
        //        if (AvatarSelection.Info.Gender.ToLower() == "female") print = vrColorFemale;
        //        NanoVG.nvgFillColor(vg, print);
        //        NanoVG.nvgFill(vg);
        //        // Text                
        //        NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(255, 255, 255, 255));
        //        NanoVG.nvgTextAlign(vg, (int)(EAlign.ALIGN_LEFT | EAlign.ALIGN_MIDDLE));
        //        NanoVG.nvgText(vg, infoPos.X - s.Width / 2 + 10, this.Height - infoPos.Y, avatarName);

        //        #region button Add Friend
        //        // Button Friend
        //        m_btnSelAvatarFriend = new RectangleF(infoPos.X - s.Width / 2 + s.Width + 10 + 5, this.Height - infoPos.Y - s.Height / 2 - 3, s.Height + 5, s.Height + 5);
        //        NanoVG.nvgBeginPath(vg);
        //        NanoVG.nvgRoundedRect(vg, m_btnSelAvatarFriend.X, m_btnSelAvatarFriend.Y, m_btnSelAvatarFriend.Width, m_btnSelAvatarFriend.Height, 5f);
        //        NanoVG.nvgClosePath(vg);
        //        print = m_btnSelAvatarFriend.Contains(new PointF(PlayerInput.MouseX, PlayerInput.MouseY)) ? print : vrColorDefault;
        //        NanoVG.nvgFillColor(vg, print);
        //        NanoVG.nvgFill(vg);
        //        // Button Friend Icon
        //        NanoVG.nvgFontSize(vg, 26);
        //        NanoVG.nvgFontFace(vg, "icons");
        //        NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(255, 255, 255, 255));
        //        NanoVG.nvgTextAlign(vg, (int)(EAlign.ALIGN_LEFT | EAlign.ALIGN_MIDDLE));
        //        string sts = VRWindows.UITool.cpToUTF8((int)Entypo.ICON_ADD_USER);
        //        NanoVG.nvgText(vg, m_btnSelAvatarFriend.X + 4, m_btnSelAvatarFriend.Y + m_btnSelAvatarFriend.Height / 2, sts);
        //        #endregion

        //        // Restore print
        //        if (AvatarSelection.Info.Gender.ToLower() == "male") print = vrColorMale;
        //        if (AvatarSelection.Info.Gender.ToLower() == "female") print = vrColorFemale;

        //        #region button Message
        //        // Button Message
        //        m_btnSelAvatarMessage = new RectangleF(m_btnSelAvatarFriend.X + m_btnSelAvatarFriend.Width + 5, m_btnSelAvatarFriend.Y, m_btnSelAvatarFriend.Width, m_btnSelAvatarFriend.Height);
        //        NanoVG.nvgBeginPath(vg);
        //        NanoVG.nvgRoundedRect(vg, m_btnSelAvatarMessage.X, m_btnSelAvatarMessage.Y, m_btnSelAvatarMessage.Width, m_btnSelAvatarMessage.Height, 5f);
        //        NanoVG.nvgClosePath(vg);
        //        print = m_btnSelAvatarMessage.Contains(new PointF(PlayerInput.MouseX, PlayerInput.MouseY)) ? print : vrColorDefault;
        //        NanoVG.nvgFillColor(vg, print);
        //        NanoVG.nvgFill(vg);


        //        // Button Message Icon
        //        NanoVG.nvgFontSize(vg, 26);
        //        NanoVG.nvgFontFace(vg, "icons");
        //        NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(255, 255, 255, 255));
        //        NanoVG.nvgTextAlign(vg, (int)(EAlign.ALIGN_LEFT | EAlign.ALIGN_MIDDLE));
        //        string mts = VRWindows.UITool.cpToUTF8((int)Entypo.ICON_MAIL);
        //        NanoVG.nvgText(vg, m_btnSelAvatarMessage.X + 4, m_btnSelAvatarMessage.Y + m_btnSelAvatarMessage.Height / 2, mts);
        //        #endregion button Message


        //        #region Gender Sign Icon (Disabled)

        //        if (false)
        //        {
        //            if (femaleSignImage == -1)
        //                femaleSignImage = NanoVG.CreateImage(vg, femaleSign, 0);

        //            if (maleSignImage == -1)
        //                maleSignImage = NanoVG.CreateImage(vg, maleSign, 0);

        //            int iImg = 0;
        //            switch (AvatarSelection.Info.Gender.ToLower())
        //            {
        //                case "male":
        //                    iImg = maleSignImage;
        //                    break;
        //                case "female":
        //                    iImg = femaleSignImage;
        //                    break;
        //            }

        //            float picX = infoPos.X - s.Width / 2 - 5 - s.Height;
        //            float picY = this.Height - infoPos.Y - s.Height / 2 - 3;

        //            NVGpaint imgPaint = NanoVG.nvgImagePattern(vg, picX, picY, s.Height, s.Height, iImg);
        //            NanoVG.nvgBeginPath(vg);
        //            NanoVG.nvgRect(vg, picX, picY, s.Height, s.Height);
        //            NanoVG.nvgClosePath(vg);
        //            NanoVG.nvgFillPaint(vg, imgPaint);
        //            NanoVG.nvgFill(vg);
        //        }

        //        #endregion

        //        NanoVG.nvgRestore(vg);
        //        //NanoVG.nvgEndFrame(vg);
        //    }
        //}
        //private void drawMyAvatarInfo()
        //{
        //    if (null != Player.ThirdPersonView.Avatar)
        //    {
        //        Vector3 topVertex = Player.ThirdPersonView.Avatar.GetTopVertex();

        //        Matrix4 scl = Matrix4.CreateScale(Player.ThirdPersonView.Avatar.Size);
        //        scl.TransformVector(ref topVertex);

        //        Vector3 avatarPos = Vector3.Zero;// AvatarSelection.Model.AvatarBounds.Center;
        //        Vector3 avatarTopVertex = Player.ThirdPersonView.Avatar.Position + topVertex;
        //        Vector3 avatarPoint = new Vector3(avatarPos.X + avatarTopVertex.X,
        //                                          avatarPos.Y + avatarTopVertex.Y,
        //                                          avatarPos.Z + avatarTopVertex.Z);

        //        Vector2 infoPos = UnProject(avatarPoint, Player.ViewInfo.modelviewMatrix, Player.ViewInfo.projectionMatrix, this.Width, this.Height);
        //        infoPos.Y += 15;

        //        //NanoVG.nvgBeginFrame(vg, Width, Height, 1f);
        //        NanoVG.nvgSave(vg);
        //        NanoVG.nvgScissor(vg, this.m_screen.WorkspaceRectangle.X, this.m_screen.WorkspaceRectangle.Y,
        //                              this.m_screen.WorkspaceRectangle.Width, this.m_screen.WorkspaceRectangle.Height);



        //        string avatarName = string.Format("{0} {1}", Program.FirstName, Program.LastName);
        //        var bounds = new float[4];

        //        NanoVG.nvgFontSize(vg, 20);
        //        NanoVG.nvgFontFace(vg, "PlayRegular");
        //        NanoVG.nvgTextBounds(vg, 0, 0, avatarName, bounds);

        //        var s = new Size((int)(bounds[2] - bounds[0]), (int)(bounds[3] - bounds[1]));

        //        //s = new Size((int) (s.Width * 1.6f), (int) (s.Height * 1.6f));

        //        NanoVG.nvgBeginPath(vg);
        //        NanoVG.nvgRoundedRect(vg, infoPos.X - s.Width / 2 - 5, this.Height - infoPos.Y - s.Height / 2 - 3, s.Width + 10, s.Height + 5, 5f);
        //        NanoVG.nvgClosePath(vg);
        //        NVGcolor print = vrColorDefault;

        //        if (null != Program.UserInfo)
        //        {
        //            if (Program.UserInfo.Gender.ToLower() == "male") print = vrColorMale;
        //            if (Program.UserInfo.Gender.ToLower() == "female") print = vrColorFemale;
        //        }

        //        NanoVG.nvgFillColor(vg, print);
        //        NanoVG.nvgFill(vg);

        //        NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(255, 255, 255, 255));
        //        NanoVG.nvgTextAlign(vg, (int)(EAlign.ALIGN_LEFT | EAlign.ALIGN_MIDDLE));
        //        NanoVG.nvgText(vg, infoPos.X - s.Width / 2, this.Height - infoPos.Y, avatarName);

        //        NanoVG.nvgResetScissor(vg);
        //        NanoVG.nvgRestore(vg);
        //        //NanoVG.nvgEndFrame(vg);



        //    }
        //}

        #region GUI
        public override void GuiCreate()
        {
            #region NanoVG

           
            GlNanoVG.nvgCreateGL(ref vg, (int)NVGcreateFlags.NVG_ANTIALIAS | (int)NVGcreateFlags.NVG_STENCIL_STROKES |
                                            (int)NVGcreateFlags.NVG_DEBUG);

            if (!loadUIFonts(ref vg))
                throw new Exception("Not loaded data ...");

            this.initializePerformanceTrends();


            #endregion            

            this.m_screen = new WorkspaceDisplay(this, true);
            this.m_screen.Initialize();
        }
        public override void GuiDraw(double time)
        {
            //return;
            NanoVG.nvgBeginFrame(vg, Width, Height, 1f);
            if (null != m_screen)
            {
                if (m_screen.IsHide)
                    m_screen.Show();
                m_screen.Render();


            }
            NanoVG.nvgEndFrame(vg);
            Draw3DCursor();

            if(DrawPerformance)
                this.drawPerformanceTrends();

            DrawMeshDrawTimes();
            DrawDrawablesDrawTimes();
            DrawBounds();

            //SciterWnd.


        }
        private void Draw3DCursor()
        {
            if (VAR_AppState == ApplicationState.Playing &&
               (((GameInput)AppInput).Keyboard[OpenTK.Input.Key.LControl] ||
               ((GameInput)AppInput).Keyboard[OpenTK.Input.Key.RControl]))
            {
                NanoVG.nvgBeginFrame(vg, Width, Height, 1f);
                debugDrawDesignAxes(AppInput.MouseX, AppInput.MouseY);
                NanoVG.nvgEndFrame(vg);
            }
        }

        public override bool CursorOverGUI()
        {
            //if (m_screen.FocusedWidget != null) return true;

            if (CursorOverDialog())
                return true;

            int lx, ly = 0;

            Point M = this.MousePosition;

            Widget m = m_screen.GetControlAt(M.X, M.Y, out lx, out ly);

            if (m != null && !(m is WorkspaceDisplay))     // Added May-30-18 -> This should be the only function here
                return true;

            //if (null != m_btnSelAvatarMessage && null != m_btnSelAvatarFriend)
            //{
            //    m_btnSelAvatarFriendHover = cursorOverAvatarFriendButton();
            //    m_btnSelAvatarMessageHover = cursorOverAvatarMessageButton();
            //    if (m_btnSelAvatarFriendHover ||
            //        m_btnSelAvatarMessageHover)
            //    {
            //        ((GameInput)PlayerInput).CursorOverAvatarButton = true;

            //        if (PlayerInput.MouseButtonLeft == true)
            //        {
            //            if (m_btnSelAvatarFriendHover && m_nextAvatarFriendClick < DateTime.Now)
            //            {
            //                ConsoleUtil.log("+Avatar Friend Clicked()");
            //                m_nextAvatarFriendClick = DateTime.Now.AddMilliseconds(2000);
            //                ((WindowsGame)WindowsGame.Instance).AddFriendRequest(AvatarSelection.Info.UserId);
            //                DialogSimple fc = new DialogSimple("Friend request sent", string.Format("You have added {0} {1} to your friends list", AvatarSelection.Info.FirstName, AvatarSelection.Info.LastName), 10, 10);
            //                fc.Size = new Size(450, 130);
            //                fc.Show(this.Screen2D);
            //                fc.BringToFront(this.Screen2D);
            //                fc.Focus();
            //            }
            //            else if (m_btnSelAvatarMessageHover && m_nextAvatarMessageClick < DateTime.Now)
            //            {
            //                ConsoleUtil.log("+Avatar Message Clicked()");
            //                m_nextAvatarMessageClick = DateTime.Now.AddMilliseconds(2000);
            //                DialogSimple fc = new DialogSimple("Send Message", "Feature availalbe on the next update", 10, 10);
            //                fc.Size = new Size(450, 130);
            //                fc.Show(this.Screen2D);
            //                fc.BringToFront(this.Screen2D);
            //                fc.Focus();
            //            }
            //        }

            //        return true;
            //    }
            //    else
            //    {
            //        ((GameInput)PlayerInput).CursorOverAvatarButton = false;
            //    }
            //}

            //if (m is ScreenUI && m_screen.WorkspaceRectangle.Contains(new Point(M.X, M.Y)))
            //    if (m_screen.WindowActive) return true;

            //return !(m is ScreenUI) && !m_screen.WorkspaceRectangle.Contains(new Point(M.X, M.Y));

            return M.Y <= Screen2D.Widgets[0].Height;
        }
        public override bool CursorOverDialog()
        {
            try
            {
                if (null != m_screen)
                    if (null != m_screen.OverlayCurrent)
                        return m_screen.OverlayCurrent.IsVisible;
                //if (null != m_screen.FocusedWidget)
                //{
                //    if (ScreenUI.FocusedWidget is Overlay) return true;

                //    try
                //    {
                //        if (ScreenUI.FocusedWidget.Parent != null)
                //            if (ScreenUI.FocusedWidget.Parent is Overlay) return true;
                //    }
                //    catch { }

                //    try
                //    {
                //        if (ScreenUI.FocusedWidget.Parent.Parent != null)
                //            if (ScreenUI.FocusedWidget.Parent.Parent is Overlay) return true;
                //    }
                //    catch { }

                //    try
                //    {
                //        if (ScreenUI.FocusedWidget.Parent.Parent.Parent != null)
                //            if (ScreenUI.FocusedWidget.Parent.Parent.Parent is Overlay) return true;
                //    }
                //    catch { }

                //    try
                //    {
                //        if (ScreenUI.FocusedWidget.Parent.Parent.Parent.Parent != null)
                //            if (ScreenUI.FocusedWidget.Parent.Parent.Parent.Parent is Overlay) return true;
                //    }
                //    catch { }

                //    try
                //    {
                //        if (ScreenUI.FocusedWidget.Parent.Parent.Parent.Parent.Parent != null)
                //            if (ScreenUI.FocusedWidget.Parent.Parent.Parent.Parent.Parent is Overlay) return true;
                //    }
                //    catch { }

                //    try
                //    {
                //        if (ScreenUI.FocusedWidget.Parent.Parent.Parent.Parent.Parent.Parent != null)
                //            if (ScreenUI.FocusedWidget.Parent.Parent.Parent.Parent.Parent.Parent is Overlay) return true;
                //    }
                //    catch { }
                //}
            }
            catch
            {
                return false;
            }
            return false;
        }
        #endregion GUI

    }
}
