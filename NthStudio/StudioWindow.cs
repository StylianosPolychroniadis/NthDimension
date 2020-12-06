#define SCITER

using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Drawing;

using NthDimension.Rendering;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Utilities;
using NthDimension.Rasterizer;
using NthDimension.Rasterizer.NanoVG;
using NthDimension.Rasterizer.Windows;
using NthDimension.Forms.Events;
using NthDimension.Forms;
using NthDimension.Algebra;
using NthDimension.Rendering.Drawables.Framebuffers;
using NthDimension.Rendering.Drawables;
using NthDimension.Network;
using NthDimension.Utilities;

using NthStudio.Gui;
using NthStudio.Gui.Displays;
using NthStudio.Game.Crowd;

using NthDimension.Rendering.Drawables.Models;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Serialization;
using System.Collections.Generic;

using NthDimension.Procedural;
using NthStudio.Game.Procedural;
using NthDimension.Rendering.GameViews;
using NthDimension.Rendering.Drawables.Lights;
using NthDimension.Rendering.Sound;
using NthDimension.Rendering.Scenegraph;

namespace NthStudio
{
    public partial class StudioWindow : ApplicationBase
    {
        #region API Fields and Properties
        // NanoVG 
        public static NVGcontext                    vg                      = new NVGcontext();
        private WorkspaceDisplay                    m_screen;
        //private DisplaySocialNet m_screen;
        protected static int                        AntiAliasingSamples     = 0;
        private const string                        timeFormat              = @"dd/MM/yy hh:mm:ss.fff";
        private const string                        chacheFileExtension     = "ucf";
        private readonly Size                       minWindowSize           = new Size(800, 600);


        
        
        protected SceneItem                         m_SelectedItem;
        //private SceneItem                           m_sceneAppartment;

        private DateTime                            tSceneLoad;
        private DateTime                            escTimeout              = DateTime.Now;

        public SceneItem                            ActiveSceneFile { get; set; }

        public NthDimension.Forms.Widget.WHUD       Screen2D
        {
            get { return this.m_screen; }
        }
        #endregion

        #region Game Fields and Properties
        private CrowdSimulator crowd;

      
        #endregion

        #region Ctor
        public StudioWindow(  string                                gamePath, 
                              string                                gameTitle, 
                              RendererBaseGL3                          rendererApi,
                              AudioBase                             audioApi,
                              int                                   pWidth, 
                              int                                   pHeight, 
                              bool                                  fullScr,
                              OpenTK.Graphics.GraphicsMode          gmode,
                              OpenTK.DisplayDevice                  device, 
                              int                                   major, 
                              int                                   minor,
                              OpenTK.Graphics.GraphicsContextFlags  contextFlags, 
                              bool                                  offline) 
            : base(gamePath, gameTitle, rendererApi, audioApi, pWidth, pHeight, fullScr, gmode, device, major, minor, contextFlags, offline)
        {
            VSync                       = OpenTK.VSyncMode.Off;
            this.Visible                = true;
            this.VAR_ScreenColor        = new Vector4(0.1f, 0.1f, 0.1f, 1f);

            this.Icon                   = Properties.Resources.nSphere;

            
        }

         ~StudioWindow()
        {
            Audio.Destroy();

            base.Dispose();

            // TODO:: Destroy AL audio
        }
        #endregion Ctor

        #region Register Input Handlers
        public override void RegisterInputHandlers()
        {
            #region Keyboard
            //this.KeyDown += delegate (object sender, OpenTK.Input.KeyboardKeyEventArgs e)
            //{
            //    ////if (null != AppInput)
            //    ////    ((GameInput)AppInput).KeyDown(sender, e);
                
            //    ////updateKeyboard();


            //    //if (!AudioSources.ContainsKey("blip"))
            //    //    AudioSources.Add("blip", new WavSound(File.Open(Path.Combine(GameSettings.AudioFolder, "blip.wav"), FileMode.Open)));
                
            //    //AudioSources.FirstOrDefault(a => a.Key == "blip").Value.Play();
                
                
            //};
            this.KeyPress += delegate (object sender, OpenTK.KeyPressEventArgs e)
            {
                //if (m_screen != null && !resettingUserInterface)
                //    m_screen.KeyPress(sender, e);

                updateKeyboard();
            };
            this.KeyUp += delegate (object sender, OpenTK.Input.KeyboardKeyEventArgs e)
            {
                //if (m_screen != null && !resettingUserInterface)
                //    m_screen.KeyUp(sender, e);
                updateKeyboard();


            };
            #endregion Keyboard

            #region Mouse
            this.MouseMove += delegate (object sender, OpenTK.Input.MouseMoveEventArgs e)
            {
                this.MousePosition = new Point(e.X, e.Y);
                if (null != AppInput)
                    if (!CursorOverGUI())
                        ((GameInput)AppInput).MouseMove(sender, e);
                //if (m_screen != null && !resettingUserInterface)
                //    m_screen.MouseMove(sender, e);
            };
            this.MouseDown += delegate (object sender, OpenTK.Input.MouseButtonEventArgs e)
            {
                if (null != AppInput)
                    if (!CursorOverGUI())
                        ((GameInput)AppInput).MouseDown(sender, new MouseButtonEventArgs(e.X, e.Y, e.Button.ToNth(), e.IsPressed));
                //if (m_screen != null && !resettingUserInterface)
                //    m_screen.MouseDown(sender, e);
            };
            this.MouseUp += delegate (object sender, OpenTK.Input.MouseButtonEventArgs e)
            {
                if (null != AppInput)
                    if (!CursorOverGUI())
                        ((GameInput)AppInput).MouseUp(sender, new MouseButtonEventArgs(e.X, e.Y, e.Button.ToNth(), e.IsPressed));
                //if (m_screen != null && !resettingUserInterface)
                //    m_screen.MouseUp(sender, e);
            };
            this.MouseWheel += delegate (object sender, OpenTK.Input.MouseWheelEventArgs e)
            {
                if (null != AppInput)
                {
                    ((GameInput)AppInput).WheelValue = e.Value;
                    ((GameInput)AppInput).WheelValuePrecise = e.ValuePrecise;
                    ((GameInput)AppInput).WheelDelta = e.Delta;
                    ((GameInput)AppInput).WheelDeltaPrecise = e.DeltaPrecise;
                }
                //if (m_screen != null && !resettingUserInterface)
                //    m_screen.MouseWheel(sender, e);
            };
            #endregion Mouse
        }
        #endregion Register Input Handlers

        #region Keyboard Input
        private void updateKeyboard()
        {
            #region Key ESC

            bool ESC = OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.Escape);
            if (ESC && !prevESC && DateTime.Now > escTimeout)
            {
                escTimeout = DateTime.Now.AddMilliseconds(700);
                //if (GameState == GameState.Playing)
                //    enterMenu();
                if (this.Focused)
                {
                    //if (null == m_confirmExitDialog)
                    //    m_confirmExitDialog = new ExitMessageDialog();

                    //if (!m_confirmExitDialog.IsVisible)
                    //{
                    //    m_confirmExitDialog.OnButtonPressed +=
                    //        delegate (enuExitDialogResponce responce)
                    //        {
                    //            switch (responce)
                    //            {
                    //                case enuExitDialogResponce.Cancel:

                    //                    break;
                    //                case enuExitDialogResponce.Yes:
                    //                    base.Close();
                    //                    WindowsGame.Instance.exitGame();
                    //                    break;
                    //                case enuExitDialogResponce.No:

                    //                    break;
                    //            }
                    //        };
                    //    //m_confirmExitDialog.Show(((WindowsGame)WindowsGame.Instance).Screen2D.WindowHUD);
                    //    //m_confirmExitDialog.BringToFront(Screen2D.WindowHUD);

                    //    m_confirmExitDialog.Show(Screen2D);
                    //    m_confirmExitDialog.BringToFront(Screen2D);
                    //}
                }

            }
            prevESC = ESC;

            #endregion

            #region Key Tilde ~

            //bool Tilde = ((GameInput)PlayerInput).Keyboard[OpenTK.Input.Key.Tilde];
            bool Tilde = OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.Tilde);
            if (Tilde & !prevTilde)
                if (VAR_AppState == ApplicationState.Playing)
                {
                    //if (!debugConsole.IsOpen)
                    //    debugConsole.Open();
                    //else
                    //    debugConsole.Close();
                    ////this.ReceiveChatMessage(new PacketChat.Event(DateTime.Now, new Guid(), "TEST_AVATAR", "Hello World"));
                }
            prevTilde = Tilde;

            #endregion

            #region Key F6
            bool F6 = OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.F6);
            if (F6 && !prevF6)
            {
                foreach (NthDimension.Rendering.Shaders.Shader shader in NthDimension.Rendering.Factories.ShaderLoader.UsedShaders)
                {
                    using (StreamWriter outfile =
                        new StreamWriter(string.Format(@"logshaders\{0}.vs", shader.Name)))
                        outfile.Write(shader.VertexShader.ToString());

                    using (StreamWriter outfile =
                        new StreamWriter(string.Format(@"logshaders\{0}.fs", shader.Name)))
                        outfile.Write(shader.FragmentShader.ToString());
                }
            }
            prevF6 = F6;
            #endregion F6

            #region Key F12

            //bool F12 = ((GameInput)PlayerInput).Keyboard[OpenTK.Input.Key.F12];
            bool F12 = OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.F12);
            if (F12 && !prevF12)
                if (VAR_AppState == ApplicationState.Playing)
                    NthDimension.Rendering.Loaders.TextureLoader.GrabScreenshot();
            prevF12 = F12;

            #endregion

            #region Ctrl-F1 (Re-load Settings)

            //bool CtrlF1 = (((GameInput) PlayerInput).Keyboard[OpenTK.Input.Key.ControlLeft] ||
            //               ((GameInput) PlayerInput).Keyboard[OpenTK.Input.Key.LControl] &&
            //               ((GameInput) PlayerInput).Keyboard[OpenTK.Input.Key.F1]);
            //if (CtrlF1 && !prevCtrlF1)
            //{
            //    Program.LoadSettings();
            //    ConsoleUtil.log("Settings Loaded");
            //}
            //prevCtrlF1 = CtrlF1;



            #endregion

            #region Key PGUP
            bool PGUP = OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.PageUp);
            if (PGUP && Player != null)
            {
                //Player.ThirdPersonView.Avatar.Position = new Vector3(Player.ThirdPersonView.Avatar.Position.X, Player.ThirdPersonView.Avatar.Position.Y + 1.0f, Player.ThirdPersonView.Avatar.Position.Z);
                //Player.ThirdPersonView.Avatar.BodyPosition = Player.ThirdPersonView.Avatar.Position;
                //Player.Body.Position = new NthDimension.Physics.LinearMath.JVector(Player.ThirdPersonView.Avatar.Position.X, Player.ThirdPersonView.Avatar.Position.Y, Player.ThirdPersonView.Avatar.Position.Z);

            }
            prevPGUP = PGUP;

            #endregion Key PGUP

            #region Key PGDN
            bool PGDN = OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.PageDown);
            if (PGDN && Player != null)
            {
                //Player.ThirdPersonView.Avatar.Position = new Vector3(Player.ThirdPersonView.Avatar.Position.X, Player.ThirdPersonView.Avatar.Position.Y - 1.0f, Player.ThirdPersonView.Avatar.Position.Z);
                //Player.ThirdPersonView.Avatar.BodyPosition = Player.ThirdPersonView.Avatar.Position;
                //Player.Body.Position = new NthDimension.Physics.LinearMath.JVector(Player.ThirdPersonView.Avatar.Position.X, Player.ThirdPersonView.Avatar.Position.Y, Player.ThirdPersonView.Avatar.Position.Z);
            }
            prevPGDN = PGDN;
            #endregion key PGDN
        }
        #endregion Keyboard Input

        #region Helper Functions
        public Vector3 Project(Vector2 p, ViewInfo view)
        {
            //Point p = new Point(GameInput.MouseX, GameInput.MouseY);
            //RaycastCallback raycastCallback = new RaycastCallback(new RaycastCallback(new RigidBody(NthDimension.Physics.Collision.Shapes.BoxShape));
            Vector2 biasp = new Vector2(p.X,
                                        p.Y);   // TODO:: Y appears lower than cursor

            Vector3 v1 = RayHelper.UnProject(view.projectionMatrix,
                                                view.modelviewMatrix,
                                                new Size(ApplicationBase.Instance.Width, ApplicationBase.Instance.Height),
                                                new Vector3(biasp.X, biasp.Y, -1.5f));
            //new Vector3(biasp.X, biasp.Y, 0f));

            Vector3 v2 = RayHelper.UnProject(view.projectionMatrix,
                                              view.modelviewMatrix,
                                                new Size(ApplicationBase.Instance.Width, ApplicationBase.Instance.Height),
                                                new Vector3(biasp.X, biasp.Y, 1f));

            NthDimension.Physics.Dynamics.RigidBody body; NthDimension.Physics.LinearMath.JVector normal; float frac;

            bool result = Scene.CollisionRaycast(GenericMethods.FromOpenTKVector(v1), GenericMethods.FromOpenTKVector(v2),
                null, out body, out normal, out frac);

            Vector3 hitCoords = v1 + v2 * frac;

            return hitCoords;
        }
        public Vector2 UnProject(Vector3 pos, Matrix4 viewMatrix, Matrix4 projectionMatrix, int screenWidth, int screenHeight)
        {
            pos = Vector3.Transform(pos, viewMatrix);
            pos = Vector3.Transform(pos, projectionMatrix);
            pos.X /= pos.Z;
            pos.Y /= pos.Z;
            pos.X = (pos.X + 1) * screenWidth / 2;
            pos.Y = (pos.Y + 1) * screenHeight / 2;

            return new Vector2(pos.X, pos.Y);
        }
        #endregion

        #region OnLoad / OnResize / OnClosing / OnFocusedChanged
        private bool loadUIFonts(ref NVGcontext vg)
        {
            #region Setup Fonts

            #region Font: sans
            //Fonts.Load(vg, "sans", "Roboto-Regular.ttf");
            //Fonts.Load(vg, "sans", "Attractive-Regular.ttf");
            Fonts.Load(vg, "sans", "Cmu-Bright-Roman.ttf");
            //Fonts.Load(vg, "sans", "spaceage.ttf");
            if (Fonts.Get("sans") != -1)
                ConsoleUtil.log("<> Loaded Font: Roboto-Regular.ttf");
            else
                return false;
            #endregion

            #region Font: sans-bold
            Fonts.Load(vg, "sans-bold", "Roboto-Bold.ttf");
            if (Fonts.Get("sans-bold") != -1)
                ConsoleUtil.log("<> Loaded Font: Roboto-Bold.ttf");
            else
                return false;
            #endregion

            #region Font: icons
            Fonts.Load(vg, "icons", "entypo.ttf");
            if (Fonts.Get("icons") != -1)
                ConsoleUtil.log("<> Loaded Font: entypo.ttf");
            else
                return false;
            #endregion

            #region Font: emoji
            Fonts.Load(vg, "emoji", "NotoEmoji-Regular.ttf");
            if (Fonts.Get("emoji") != -1)
                ConsoleUtil.log("<> Loaded Font: NotoEmoji-Regular.ttf");
            else
                return false;
            #endregion

            NanoVG.nvgAddFallbackFontId(vg, Fonts.Get("sans"), Fonts.Get("emoji"));
            NanoVG.nvgAddFallbackFontId(vg, Fonts.Get("sans-bold"), Fonts.Get("emoji"));
            #endregion

            return true;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //// Test Audio Here
            //var soundFile = new WavSound(File.Open(Path.Combine(GameSettings.AudioFolder , "blip.wav"), FileMode.Open));
            //soundFile.Play();
            //if (null != AppInput)
            //    Scene.removeChild(AppInput);
            //AppInput = new GameInput(Scene);
            //((GameInput)AppInput).OnKeyDown += delegate
            //{
            //    playBlip();
            //};

        }
        int oldW = 0;
        int oldH = 0;
        protected override void OnResize(EventArgs e)
        {
            if(null!=this.AppInput)
                if (this.AppInput.MouseButtonLeft ||
                    this.AppInput.MouseButtonRight)
                    return;

            if (this.Width < minWindowSize.Width) this.Width = minWindowSize.Width;
            if (this.Height < minWindowSize.Height) this.Height = minWindowSize.Height;

            if (oldW == this.Width && oldH == this.Height) return;
            //OpenTK.Graphics.OpenGL.GL.Viewport(this.ClientRectangle);

            if (null != m_screen)
                m_screen.Size = new Size(Width, Height);

            NthDimension.Settings.Instance.video.windowWidth = oldW = this.Width > minWindowSize.Width ? this.Width : minWindowSize.Width;
            NthDimension.Settings.Instance.video.windowHeight = oldH = this.Height > minWindowSize.Height ? this.Height : minWindowSize.Height;

            // Settings.Instance.video.virtualScreenWidth = this.Width > minWindowSize.Width ? this.Width : minWindowSize.Width;
            // Settings.Instance.video.virtualScreenHeight = this.Height > minWindowSize.Height ? this.Height : minWindowSize.Height;

            NthDimension.Settings.Instance.SaveSettings(Path.Combine(DirectoryUtil.Documents, "settings.xml"));

            base.OnResize(e);           
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;

            //m_confirmExitDialog = new ExitMessageDialog();

            //m_confirmExitDialog.OnButtonPressed +=
            //    delegate (enuExitDialogResponce responce)
            //    {
            //        switch (responce)
            //        {
            //            case enuExitDialogResponce.No:
            //            case enuExitDialogResponce.Cancel:
            //                e.Cancel = true;
            //                break;
            //            case enuExitDialogResponce.Yes:
            //                base.Close();
            //                WindowsGame.Instance.exitGame();
            //                break;



            //        }
            //    };
            //m_confirmExitDialog.Show(this.Screen2D.WindowHUD);
            //m_confirmExitDialog.BringToFront(this.Screen2D.WindowHUD);
            //m_confirmExitDialog.Focus();
        }
        protected override void OnFocusedChanged(EventArgs e)
        {
            if (null == AppInput) return;

            if (null == m_screen) return;
            m_screen.FocusedChanged(this, e);

            if (!this.Focused)
            {
                AppInput.MouseButtonLeft = false;
                AppInput.MouseButtonRight = false;
            }
        }

        #endregion

        #region Scene API

        
        private void ActivateScene(SceneItem sceneItem)
        {
            _readyToPlay = false;
            m_SelectedItem = sceneItem;

            string unloadSceneId = (null != ActiveSceneFile) ?
                                         ActiveSceneFile.SceneIdentifier :
                                         string.Empty;

            try
            {
                if (ActiveSceneFile != null)
                {
                    #region Unload Textures
                    //for (int i = 0; i < StudioWindow.Instance.TextureLoader.textures.Count; i++)
                    //    if(StudioWindow.Instance.TextureLoader.textures[i].name.Contains(string.Format(@"scenes\{0}", ActiveSceneFile.SceneIdentifier.ToLower())))
                    //        StudioWindow.Instance.TextureLoader.DeleteTexture(StudioWindow.Instance.TextureLoader.textures[i].identifier);
                    #endregion Unload Textures

                    #region Unload Models (does not destroy the VAOs)
                    if (null != StudioWindow.Instance.Scene)
                    {
                        #region Clear Scene

                        #region Remove Player
                        if (null != StudioWindow.Instance.Player)
                        {
                            StudioWindow.Instance.Scene.VisibleDrawables.Clear();
                            Drawable[] playerModels = StudioWindow.Instance.Player.AvatarModels;
                            if (null != playerModels)
                                foreach (var pd in playerModels)
                                {
                                    StudioWindow.Instance.Scene.Drawables.Remove(pd);
                                    pd.kill();
                                }
                        }
                        #endregion Remove Player

                        #region Remove Previous Scene
                        if (!string.IsNullOrEmpty(unloadSceneId))
                        {
                            // Static
                            ListMesh oldSceneMeshes = new ListMesh();

                            try
                            {
                                foreach (MeshVbo mesh in MeshLoader.meshes)
                                {
                                    try
                                    {
                                        if (null != mesh)
                                            if (mesh.Name.Contains(string.Format(@"scenes\{0}", unloadSceneId)))
                                                oldSceneMeshes.Add(mesh);
                                    }
                                    catch (Exception dE)
                                    {
                                        continue;
                                    }
                                }
                            }
                            catch (Exception drwE) { ConsoleUtil.errorlog("Clear Scene Models Error ", drwE.Message); }

                            try
                            {
                                foreach (var mesh in oldSceneMeshes)
                                {
                                    try
                                    {
                                        ConsoleUtil.log(string.Format("(-) Removing mesh {0}", mesh.Name));

                                        //MeshLoader.meshes.Remove(mesh); // Enabling this will require to reload the cache file or local mesh files

                                        List<Drawable> toBeRemoved = Scene.Drawables.Where(mm => mm.meshes.Contains(mesh)).ToList();

                                        foreach (var del in toBeRemoved)
                                        {
                                            StudioWindow.Instance.Scene.Drawables.Remove(del);
                                            StudioWindow.Instance.Scene.removeChild(del);
                                        }
                                    }
                                    catch (Exception dE)
                                    {
                                        continue;
                                    }

                                }
                            }
                            catch (Exception drwE)
                            {
                                ConsoleUtil.errorlog("Removing Model Failed ", drwE.Message);
                            }

                        }
                        #endregion Remove Previous Scene

                        #region Remove Remote Players (Avatars)
                        try
                        {
                            for (int r = StudioWindow.Instance.Scene.RemotePlayers.Count; r >= 0; r--)
                                try
                                {
                                    StudioWindow.Instance.Scene.Drawables.Remove(StudioWindow.Instance.Scene.RemotePlayers[r]);                                    
                                    StudioWindow.Instance.Scene.RemotePlayers[r].kill();

                                }
                                catch (Exception dE)
                                {
                                    continue;
                                }

                            
                        }
                        catch { }

                        // Animated
                        try
                        {
                            StudioWindow.Instance.Scene.RemotePlayersDestroy.AddRange(StudioWindow.Instance.Scene.RemotePlayers);
                        }
                        catch (Exception drwE) { ConsoleUtil.errorlog("Call to Scene.RemotePlayersDestroy.AddRange() Failed ", drwE.Message); }
                        #endregion

                        #region Remove NPCs
                        try
                        {
                            for (int n = StudioWindow.Instance.Scene.NonPlayers.Count; n >= 0; n--)
                                try
                                {
                                    StudioWindow.Instance.Scene.Drawables.Remove(StudioWindow.Instance.Scene.NonPlayers[n]);
                                    StudioWindow.Instance.Scene.NonPlayers[n].kill();
                                }
                                catch (Exception nE)
                                {
                                    continue;
                                }

                        }
                        catch { }
                        try
                        {
                            StudioWindow.Instance.Scene.NonPlayersDestroy.AddRange(StudioWindow.Instance.Scene.NonPlayers);
                        }
                        catch (Exception drwE) { ConsoleUtil.errorlog("Call to Scene.DestroyNPCs.AddRange() Failed ", drwE.Message); }
                        #endregion

                        List<Light> removeLights = new List<Light>();
                        List<Skybox> removeSkybox = new List<Skybox>();

                        foreach (Drawable d in StudioWindow.Instance.Scene.Drawables)
                        {
                            if (d is Light)
                                removeLights.Add(d as LightDirectional);

                            if (d is Skybox)
                                removeSkybox.Add(d as Skybox);
                        }

                        for (int s = 0; s < removeLights.Count; s++)
                            StudioWindow.Instance.Scene.Drawables.Remove(removeLights[s]);
                        for (int s = 0; s < removeSkybox.Count; s++)
                            StudioWindow.Instance.Scene.Drawables.Remove(removeSkybox[s]);
                        

                        #endregion
                    }
                    #endregion Unload Models
                }

                StudioWindow.Instance.MeshLoader.AddCustomCacheFile(Path.Combine(DirectoryUtil.Documents_Cache, m_SelectedItem.Package.ModelsFile));
                StudioWindow.Instance.TextureLoader.AddCustomCacheFile(Path.Combine(DirectoryUtil.Documents_Cache, m_SelectedItem.Package.TexturesFile));
                StudioWindow.Instance.MaterialLoader.AddCustomCacheFile(Path.Combine(DirectoryUtil.AssemblyDirectory, NthDimension.Settings.Instance.game.materialCacheFile));
                StudioWindow.Instance.MaterialLoader.AddCustomCacheFile(Path.Combine(DirectoryUtil.Documents_Cache, m_SelectedItem.Package.MaterialsFile));

                ActiveSceneFile = m_SelectedItem;
                m_SelectedItem.LoadingAssetsProgress = 0f;
                m_SelectedItem.SceneIsLoading = true;

                //LoadAssets = true;
                Scene.Initialized = false;
                Scene.Scene.Name = ActiveSceneFile.SceneIdentifier;

                StudioWindow.Instance.LoadingPercentageReset();
                StudioWindow.Instance.ReloadAssets();

                
            }
            catch (Exception xE)
            {
                // ConsoleUtil.errorlog(string.Format("@@@ Start scene {0} failed!", m_SelectedItem.City), xE.Message);
            }

        }
        /// <summary>
        /// Must be called on the Main Thread
        /// </summary>
        private void ConfigureScene()
        {
            #region Engine Functions
            
            // When loading assets MTA the shaders and materials are constructed here and NOT in doLoadAssets_MTA
            this.ShaderLoader.LoadShaders(loadingShaderProgress);
            //this.writeShadersCache();             // ONLY on STA Thread
            this.MaterialLoader.LoadMaterials(loadingMaterialsProgress);
            //this.writeMaterialsCache();           // ONLY on STA Thread
            this.TemplateLoader.loadTemplates(loadingTemplatesProgress);

            #region Sky Cubemap FBO
            //if(null == CBOSet_Sky)
            CBO_Sky = new CubemapBufferSets(Scene, FramebufferCreator, 128);    //create cubemap buffers
                                                                                   //if(Scene.EnvTextures != CBOSet_Sky.outTextures)
            Scene.EnvTextures = CBO_Sky.outTextures;
            #endregion Sky Cubemap FBO

#if WATER_FBO
                    // create viewInfo for reflection
                                    ViewInfoWater           = new ViewInfo();
                                    ViewInfoWater.aspect    = (float) Width/(float) Height;
                                    //ViewInfoWater.zNear     = 0.05f;  // Line Added
                                    ViewInfoWater.UpdateProjectionMatrix();
#endif
            //set noise texture
            Scene.SetTextureId(Material.WorldTexture.noise, TextureLoader.getTextureId("base\\noise_pixel.png"));

            // managed setup of shaders

#if !_DEVUI_
            Scene.SetupSceneFilterShaders();
#endif
            Scene.Create(); //Scene.CreateRenderSurface();
            #endregion
        }
        private void ConfigurePlayer()
        {
            //if (null != AppInput)
            //    Scene.removeChild(AppInput);
            //AppInput = new GameInput(Scene);

            if (null != Player)
            {
                Scene.removeChild(Player);
            }
            AvatarInfoDesc desc = AvatarPresets.FemaleFit_Generic_0();
            //AvatarInfoDesc desc = AvatarPresets.FemaleFit_Generic_Ebony_00;
            //AvatarInfoDesc desc = AvatarPresets.MaleFit_Generic_0;

            Player = new ApplicationUser(Scene, desc, new Vector3(0, 0, -1), AppInput);
            Player.SetPosition(Scene.PlayerSpawnAt);
        }
        private void EnterScene()
        {
            this.bindUserInput();

            ConfigureScene();

            Scene.OnInitialize();
            Scene.SceneXsnFile = Path.Combine(DirectoryUtil.Documents_Cache, ActiveSceneFile.Package.SceneFile);

            Scene.loadObjects(Scene.SceneXsnFile);

            ConfigurePlayer();

            //Scene.OctreeWorld.Children.Clear();

            foreach (Drawable d in this.Scene.Drawables)
            {
                for (int m = 0; m < d.meshes.Length; m++)
                {
                    if (
                        GenericMethods.nearlyEqual(
                            Vector3.Subtract(d.meshes[m].OctreeBounds.Max, d.meshes[m].OctreeBounds.Min).LengthFast, 0f))
                        d.UpdateBounds();

                    //this.Scene.OctreeWorld.Add(new BoxObject(d.meshes[m].OctreeBounds, d.meshes[m].name));
                    //this.Scene.OctreeWorld.Add(d.meshes[m]);

                }
            }

            //Scene.OctreeWorld.BuildTree(new Rafa.Graphics.Culling.BoundingAABB(new Vector3(-1300, -40, -1300),
            //    new Vector3(1300, 50, 1300)));

            ConsoleUtil.log(string.Format("Scene loaded in {0}", DateTime.Now - tSceneLoad));

            VAR_AppState = ApplicationState.Playing;
            return;
        }
        private void bindUserInput()
        {
            if (null != AppInput)
                Scene.removeChild(AppInput);
            AppInput = new GameInput(Scene);
       
            this.KeyDown += delegate(object sender, OpenTK.Input.KeyboardKeyEventArgs k)
            {
                if (k.Key == OpenTK.Input.Key.Enter)
                {
                    this.AudioPlayer.playBlip();

#if DEBUG
                Console.WriteLine("blip");
#endif
                }
            };
        }
#endregion

#region Render Frame
        protected override void OnRenderFrame(OpenTK.FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            if (null != crowd)
                crowd.Draw();

            //if (null != Scene && Player != null)
            //{
            //    TransformationWidgets2D.DrawRotation(MATRIXMODE.World, new Vector3(0, 0, 0), Player.ViewInfo);
            //}

        }
#endregion Render Frame

#region Update Frame
        
        protected override void OnUpdateFrame(OpenTK.FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // TODO:: Create a stack of IUpdate actions and call here
            // ie IUpdateFrame[] updateObjects = Scene.GetUpdateable();
            //    foreach(var upd in updateObjects)
            //        upd.UpdateFrame(e.Time);
            // 

#region UI Updates
            this.updatePerformanceTrends(e.Time);
            if (null != m_screen)
            {
                m_screen.Update();

                int lx, ly = 0;
                Widget w = m_screen.GetControlAt(Mouse.X, Mouse.Y, out lx, out ly);
                if (w != null)
                    this.Title = w.Name;

                if (VAR_AppState == ApplicationState.AssetLoad)
                    m_screen.SetProgress(VAR_SceneLoadPercentage * 100, string.Format("Loading Scene {0}... {1}%", ActiveSceneFile.SceneName, (VAR_SceneLoadPercentage * 100).ToString("##0.0")));
            }
#endregion UI Updates

            //SciterWnd.UpdateWindow();

#region Player Input Update
            if (null != AppInput && !CursorOverGUI())
                AppInput.update();

            this.CtrlF1();
            
#endregion

            this.mousePickAvatars();

            if (null != crowd)
                crowd.Update();

#region Enter Scene
            if (_readyToPlay && VAR_AppState != ApplicationState.Playing && !Scene.Initialized)
            {
                EnterScene();
            }
#endregion Enter Scene

        }
#endregion

        public void createCrowd(int humanCount = 10, int assasinCount = 1)
        {

            StudioWindow.Instance.Scene.RemotePlayers.Clear();

            DateTime cNow = DateTime.Now;
            ConsoleUtil.log(string.Format("Creating Crowd..."));
            //crowd = new CrowdSimulator(-2, 2, -45, 10);
            crowd = new CrowdSimulator(-50, 50, -50, 50);
            crowd.Init(humanCount, assasinCount);
            ConsoleUtil.log(string.Format("Creating Crowd... Done! (Total time {0})", (DateTime.Now - cNow)), false);
        }

        
    }
}
