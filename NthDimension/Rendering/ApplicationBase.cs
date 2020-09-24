/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

#define _WINDOWS_
//#define WATER_FBO

namespace NthDimension.Rendering
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Drawing;
    using System.Diagnostics;
    using System.Collections.Generic;


    using OpenTK.Graphics;

    using NthDimension.Algebra;
    using NthDimension.Rasterizer;

    using NthDimension.Rendering.Drawables;
    using NthDimension.Rendering.Configuration;
    using NthDimension.Rendering.Scenegraph;
    using NthDimension.Rendering.Drawables.Framebuffers;
    using NthDimension.Rendering.Loaders;
    using NthDimension.Rendering.Utilities;
    using NthDimension.Utilities;
    using NthDimension.Compute;


    //    using Launcher;
    //using Launcher.Plugin;

    public partial class ApplicationBase
#if _WINDOWS_
        : OpenTK.GameWindow //, IGraphicsPlugin
#endif
    {
        #region Events
        public delegate void AssetsLoadingComplete();
        public event AssetsLoadingComplete OnAssetsLoaded;
        #endregion

        #region Singleton
        protected static ApplicationBase _instance;
        public static ApplicationBase Instance
        {
            get
            {
                if (null == _instance)
                    //throw new NullReferenceException("Game instance not initialized");
                    return null;

                return _instance;
            }
        }
        #endregion

        public enum enuThreadAppartment                                                                                         // TODO:: To be used for avoid writing cache files twice (see CreateScene)
        {
            STA,
            MTA
        }

        #region Properties

        public ApplicationUserInput AppInput;
        public static DeviceCapabilities DeviceLimits = new DeviceCapabilities();
        public DeviceVendor DeviceVendor = DeviceVendor.UNDEFINED;
        /// <summary>
        /// Graphics API (OpenGL)
        /// </summary>
        public RendererBaseGL3 Renderer
        {
            get { return _renderer; }
        }
        public AudioBase Audio
        {
            get { return _audio; }
        }


        /// <summary>
        /// Compute API (OpenCL)
        /// </summary>
        public ComputeDevice ComputeCL
        {
            get { return _computer; }
        }

        
        public ApplicationCompiler Compiler
        {
            get { return _compiler; }
            private set { _compiler = value; }
        }


        #region Variables

        public bool VAR_AppFullScreen;
        public ApplicationState VAR_AppState { get; set; }
        internal readonly string VAR_AppTitle;
        internal string VAR_AppPath { get; set; }
        public float VAR_FrameTime;
        public float VAR_FrameTime_Last { get; set; }
        public float VAR_FrameTime_Previous { get; set; }

        public double VAR_Framerate_Smoothness = .995d;

        public float VAR_SceneLoadPercentage = float.MinValue;

        public Vector4 VAR_ScreenColor = new Vector4(0f, 0f, 0f, 1f); //new Vector4(0.156f, 0.203f, 0.219f, 1f);
        public Vector2 VAR_ScreenSize { get; set; }
        public Vector2 VAR_ScreenSize_Virtual;
        public Vector2 VAR_ScreenSize_Current;

        public bool VAR_ShowSplash = true;
        public Stopwatch VAR_StopWatch { get; set; }

        public volatile bool VAR_SkipFrame = false;

        #endregion

        #region Asset Factories
        public FramebufferCreator FramebufferCreator { get; set; }
        public TextureLoader TextureLoader { get; set; }
        public MaterialLoader MaterialLoader { get; set; }
        public ShaderLoader ShaderLoader { get; set; }
        public MeshLoader MeshLoader { get; set; }
        public TemplateLoader TemplateLoader { get; set; }
        public AnimationLoader AnimationLoader { get; set; }
        #endregion

        #region User
        public ApplicationUser Player;
        #endregion

        #region Scene
        public SceneGame Scene { get; set; }

        #endregion

        #region Framebuffers
        // Update

        public Framebuffer                  FBO_Shadow { get; set; }
        //public Framebuffer[] FBO_ShadowPssm = new Framebuffer[4];
        public FramebufferSet               FBO_Scene { get; set; }
        public CubemapBufferSets            CBO_Sky { get; set; }
        public int                          CBO_SkySide { get; set; } //= 0;
        public Quad2d                       Filter2D_Splash { get; set; }
        #endregion

        public Point MousePosition { get; set; }

        public bool LoadAssets = true;

        public static bool SynchronizedCache = true;
        //public static DateTime CacheSynchonized;

        public static bool cacheModel_InSync = false;
        //public static string cacheShader_MD5;
        //public static DateTime cacheShader_time;
        public static bool cacheShader_InSync = false;
        //public static string cacheTemplate_MD5;
        //public static DateTime cacheTemplate_time;
        public static bool cacheTemplate_InSync = false;
        //public static string cacheMaterial_MD5;
        //public static DateTime cacheMaterial_time;
        public static bool cacheMaterial_InSync = false;
        //public static string cacheTexture_MD5;
        //public static DateTime cacheTexture_time;
        public static bool cacheTexture_InSync = false;

        public bool CursorOverAvatar { get; set; }

        public List<string> VideoFiles = new List<string>();
        public Dictionary<NthDimension.FFMpeg.VideoSource, Material> VideoSources= new Dictionary<NthDimension.FFMpeg.VideoSource, Material>();
        public Dictionary<string, NthDimension.Audio.AudioSource> AudioSources = new Dictionary<string, Audio.AudioSource>();

#if WATER_FBO
        public Framebuffer                      FBO_Water { get; set; }
        public FramebufferSet FBOSet_Water { get; set; }
        public ViewInfo WaterView { get; set; }
#endif

    #region IGraphicsPlugin Implementation
    //public GameWindow Content { get { return this; } }

    //public ShowAs ShowAs { get { return ShowAs.Normal; } }

    //public string Description {  get { return "NthDimension OpenGL 3.0 Graphics"; } }

    //public string Group { get { return "NthDimension Platform"; } }

    //public string SubGroup { get { return "NthDimension Platform"; } }

    //public System.Xml.Linq.XElement Configuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    //string IPlugin.Icon { get { return "not defined"; } }
    #endregion IGraphicsPlugin Implementation 

    #endregion

    #region Fields
    protected static RendererBaseGL3 _renderer;
        protected static AudioBase _audio;
        protected static ComputeDevice _computer;
        protected StreamWriter _log;                                                   // = new StreamWriter(File.Open(DirectoryUtil.Documents_MySoci + "/logs/engine_" + DateTime.Now.ToFileTime() + ".log", FileMode.Create, FileAccess.Write))
        protected FileSeeker _fileSeeker;

        private double VAR_framerate_smoothness = 0.995;
        private long VAR_timeElapsed;

        protected string VAR_VersionOpenGL;
        protected string VAR_VersionOpenGLShader;
        protected string VAR_deviceOpenGL;
        protected string VAR_vendorOpenGL;

        public long VAR_TotalVideoRam;

        #endregion

        private bool limitsCheckOnFirstRenderLoop = false;
        private bool offlineDebug = false;
        private static Vector3 yFlip = new Vector3(1, -1, 1);
        private static Vector3 waterLevel = new Vector3(0, SceneGame.WaterLevel, 0);

        #region Ctor
        public ApplicationBase(string gamePath,
                               string gameTitle,
                               RendererBaseGL3 rendererAPI,
                               AudioBase audioAPI,
                               int windowWidth,
                               int windowHeight,
                               bool fullScreen,
                               GraphicsMode graphicsMode,
                               OpenTK.DisplayDevice device,
                               int majorGlsl,                              // TODO:: Move to RendererBase and let the API declare values
                               int minorGlsl,                              // TODO:: Move to RendererBase and let the API declare values
                               GraphicsContextFlags contextFlags,
                               bool offline)
            : base(windowWidth,
                windowHeight,
                graphicsMode, //new GraphicsMode(new ColorFormat(32), 8, 4), 
                gameTitle,
                fullScreen ? OpenTK.GameWindowFlags.Fullscreen : OpenTK.GameWindowFlags.Default,
                device,
                majorGlsl,
                minorGlsl,
                contextFlags)
        {


            VAR_AppTitle        = gameTitle;
            VAR_AppPath         = gamePath;
            Width               = windowWidth;
            Height              = windowHeight;
            VAR_AppFullScreen   = fullScreen;

            _renderer   = rendererAPI;
            _audio      = audioAPI;
            _instance   = this;

            this.offlineDebug = offline;     

            try
            {
                string docLogs = Path.Combine(DirectoryUtil.Documents, "logs");
                string docTextures = Path.Combine(DirectoryUtil.Documents, "textures");
                string docMaterials = Path.Combine(DirectoryUtil.Documents, "materials");

                if (!Directory.Exists(docLogs))
                    Directory.CreateDirectory(docLogs);

                if (!Directory.Exists(docTextures))
                {
                    Directory.CreateDirectory(docTextures);
                    Directory.CreateDirectory(Path.Combine(docTextures, "tmp"));
                }

                if (!Directory.Exists(docMaterials))
                {
                    Directory.CreateDirectory(docMaterials);
                    Directory.CreateDirectory(Path.Combine(docMaterials, "tmp"));
                }
            }

#pragma warning disable CS0168
            catch (Exception cE)
            {
                
            }

            this.FocusedChanged             += OnFocusedChanged;
            this.WindowStateChanged         += OnWindowStateChanged;

            Microsoft.Win32.SystemEvents.PowerModeChanged += OnPowerModeChanged;

            _log = new StreamWriter(File.Open(string.Format("{0}{1}{2}{3}", DirectoryUtil.Documents, "/logs/engine_", DateTime.Now.ToFileTime(), ".log"), FileMode.Create, FileAccess.Write))
            { AutoFlush = true };

        }

        private void OnPowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            VAR_SkipFrame = e.Mode == Microsoft.Win32.PowerModes.Suspend;            
        }

        private void OnFocusedChanged(object sender, EventArgs e)
        {
            VAR_SkipFrame = this.Focused;
        }
        private void  OnWindowStateChanged(object sender, EventArgs e)
        {
            VAR_SkipFrame = (this.WindowState == OpenTK.WindowState.Minimized);
        }



        ~ApplicationBase()
        {
            foreach(var audio in AudioSources)
                audio.Value.Release();
            

            foreach (var video in VideoSources)
            {
                if (video.Key.State != FFMpeg.PlayState.Playing)
                    video.Key.Stop();

                video.Key.Dispose();
            }

            
        }
        #endregion

        #region Game States
        public void enterGame()
        {
            Player.Hud.IsVisible = true;
            Player.FirstPersonView = Player.FirstPersonTools[1];
            VAR_AppState = ApplicationState.Playing;
        }

        public virtual void exitGame()
        {
            //DialogResult dr = MessageBox.Show("Save scene changes?", "Exit", MessageBoxButtons.YesNoCancel);

            //if (dr == DialogResult.Yes)
            //{
            //    Scene.saveObjects();

            //    using (StreamWriter outfile = new StreamWriter("log.txt"))
            //        outfile.Write(mLog.ToString());


            //    disconnectClient();
            //    System.Windows.Forms.Cursor.Show();

            //    this.Exit();
            //}
            //else if(dr == DialogResult.No)
            //{
            //    disconnectClient();
            //    this.Exit();
            //}
            //else if (dr == DialogResult.Cancel)
            //    return;

            Microsoft.Win32.SystemEvents.PowerModeChanged -= OnPowerModeChanged;

            DisconnectClient();
            //System.Windows.Forms.Cursor.Show();

            this.Exit();
        }

        public virtual void enterMenu()
        {
            VAR_AppState = ApplicationState.Pause;
            Player.Hud.IsVisible = false;
            Player.FirstPersonView = Player.FirstPersonTools[0];
        }


        #endregion

        #region Resize

        

        

        #endregion Resize

        #region Exit

        public override void Exit()
        {
            try
            {
                this.VAR_AppState = ApplicationState.ShutDown;
                this.BroadcastPlayerLeave();

                foreach (var audio in AudioSources)
                    audio.Value.Release();


                foreach (var video in VideoSources)
                {
                    if (video.Key.State != FFMpeg.PlayState.Playing)
                        video.Key.Stop();

                    video.Key.Dispose();
                }

                foreach (Texture tex in TextureLoader.textures)
                {
                    uint texid = (uint)tex.identifier;
                    Renderer.DeleteTextures(1, ref texid);
                    //ConsoleUtil.log(string.Format("Deleting texture id {0}", texid));
                    CheckGlError(String.Format("Delete Texture {0}", texid));
                }

            }
            catch
            {
            }
            finally
            {
                using (StreamWriter outfile = new StreamWriter("log.txt"))
                {
                    outfile.Write(_log.ToString());
                }
                this.Close();
                base.Exit();
                Environment.Exit(0);
            }
        }

        #endregion

        #region Create Framebuffers
        public void CreateFramebuffers()
        {

            RenderOptions mOptionsMain = Settings.Instance.video.CreateRenderOptions(VideoSettings.Target.main);

            // NOTE:: Requires all files to be loaded before calling


            if (null != FramebufferCreator) FramebufferCreator.Delete();
            FramebufferCreator = new FramebufferCreator(this.Width, this.Height);
            CheckGlError("*** CREATE FrameBufferCreator()");
            if (null != FBO_Shadow) FBO_Shadow.Delete();
            FBO_Shadow = FramebufferCreator.createFrameBuffer("ShadowsFBO",
                                                            Scene.ShadowResolution,
                                                            Scene.ShadowResolution,
                                                            PixelInternalFormat.Rgba16f,
                                                            false);                //crate shadow framebuffer
            CheckGlError("*** CREATE Shadow Framebuffer");
            if (null != FramebufferCreator.defaultFb) FramebufferCreator.defaultFb.Delete();
            FBO_Scene = new FramebufferSet(FramebufferCreator, FramebufferCreator.defaultFb, mOptionsMain);
            CheckGlError("*** CREATE FrameBufferSet()");

#if WATER_FBO
            RenderOptions mOptionsWater = Settings.Instance.video.CreateRenderOptions(VideoSettings.Target.water);
            FBO_Water                       = FramebufferCreator.createFrameBuffer("FBO_Water", (int)(VAR_ScreenSize_Virtual.X * 0.5), (int)(VAR_ScreenSize_Virtual.Y * 0.5));                //create framebufferset for waterreflections
                CheckGlError("*** CREATE Water FrameBuffer");
            FBOSet_Water                    = new FramebufferSet(FramebufferCreator, FBO_Water, mOptionsWater);
                CheckGlError("*** CREATE Water FrameBuffer Set");
#endif


        }
        #endregion

        #region virtual


        public virtual void     WriteCacheInfo() { }
        public virtual bool     CursorOverGUI()
        {
            return false;
        }
        public virtual bool     CursorOverDialog()
        {
            return false;
        }


        //public virtual void     OnSelectAvatar(PlayerModel.PlayerModelSelection e)
        //{

        //}
        //public virtual void     OnUnselectAvatar(PlayerModel.PlayerModelSelection e)
        //{

        //}


        public virtual void OnSelectAvatar(ModelSelection e)
        {
            ConsoleUtil.log(string.Format("Avatar {0} selected on ApplicationBase", e.Model.Name));
        }
        public virtual void OnUnselectAvatar(ModelSelection e)
        {

        }



        public virtual void     CacheSynchronized(bool inSync)
        {

        }
        
        public virtual void     RegisterInputHandlers() { }

        public virtual void     GuiCreate() { }
        public virtual void     CreatePlayer() { }


#endregion virtual

        /// <summary>
        /// Called by engine render function at the first render call
        /// </summary>
        private void checkDeviceLimitsOnce()
        {
            return; // TODO:: Validate your calculations
#pragma warning disable CS0162
            if (VAR_AppState == ApplicationState.Playing && !limitsCheckOnFirstRenderLoop)
            {
                int attrs = NthDimension.Rendering.Factories.ShaderLoader.UsedShaders/*.Where(item => item is Shader)*/.Sum(item => item.ActiveAttributes);
                int unifs = NthDimension.Rendering.Factories.ShaderLoader.UsedShaders/*.Where(item => item is Shader)*/.Sum(item => item.ActiveUniforms);

                if (attrs > DeviceLimits.Shader_MAX_VERTEX_ATTRIBS)
                    ConsoleUtil.errorlog("WARNING: Render Loop ", string.Format("\tYou have exceeded the device limits for vertex attributes. Total used {0}, Maximum {1}", 
                                                            attrs, 
                                                            DeviceLimits.Shader_MAX_VERTEX_ATTRIBS));

                if(unifs > DeviceLimits.Shader_MAX_VERTEX_UNIFORMS + DeviceLimits.Shader_MAX_FRAGMENT_UNIFORMS)
                    ConsoleUtil.errorlog("WARNING: Render Loop ", string.Format("\tYou have exceeded the device limits for shader uniforms. Total used {0}, Maximum {1}", 
                                                            unifs,
                                                            DeviceLimits.Shader_MAX_VERTEX_UNIFORMS + DeviceLimits.Shader_MAX_FRAGMENT_UNIFORMS));

                limitsCheckOnFirstRenderLoop = true;
            }
        }
        /// <summary>
        /// Writes the specified string to the Log system
        /// </summary>
        /// <param name="line"></param>
        public void WriteToLogFile(string line)
        {
            _log.WriteLine(String.Format("{0}\t{1}", DateTime.Now.ToString(@"dd/MM/yyyy hh:mm:ss.fff"), line));
        }
        /// <summary>
        /// Queries OpenGL for last error, if error occured it is automatically sent to the Log system
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public bool CheckGlError(String op)
        {
#if !OPTIMIZED
            ErrorCode error = Renderer.GetError(); ;
            bool isError = error != ErrorCode.NoError;

            if (isError)
            {
                ConsoleUtil.errorlog(op + ": ", error.ToString());
                //Console.ReadLine();
            }

            return isError;
#endif
            return false;
        }
        /// <summary>
        /// Queries whether the file access is denied, eg used by another process
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool FileInUse(string file)
        {
            try
            {
                using (Stream stream = new FileStream(file, FileMode.Open))
                {
                    stream.Close();
                    return false;
                }
            }
            catch
            {
                return true;
            }
        }
        /// <summary>
        /// Returns the total number of bytes of the GPU device RAM
        /// </summary>
        /// <returns></returns>
        public int GetAvailableDeviceMemoryBytes()
        {
            int ret = 0;

            switch (this.DeviceVendor)
            {
                case DeviceVendor.ATI:
                    int vbo_free_memory = 0;
                    int texture_free_memory = 0;
                    int renderbuffer_free_memory = 0;

                    //Renderer.GetInteger(GetPName.ATI_VBO_FREE_MEMORY, out vbo_free_memory);
                    //Renderer.GetInteger(GetPName.ATI_TEXTURE_FREE_MEMORY, out texture_free_memory);
                    //Renderer.GetInteger(GetPName.ATI_RENDERBUFFER_FREE_MEMORY, out renderbuffer_free_memory);

                    ret = vbo_free_memory + texture_free_memory + renderbuffer_free_memory;

                    break;
            }

            return ret / 1024; // Value in MBytes
        }
        /// <summary>
        /// Returns the free number of bytes of the GPU device RAM
        /// </summary>
        /// <returns></returns>
        public int  GetUsedDeviceMemoryBytes()
        {
            int ret = 0;

            switch (this.DeviceVendor)
            {
                case DeviceVendor.ATI:
                    //int vbo_free_memory = 0;
                    int texture_free_memory = 0;
                    //int renderbuffer_free_memory = 0;

                    //Renderer.GetInteger(GetPName.ATI_VBO_FREE_MEMORY, out vbo_free_memory);
                    //Renderer.GetInteger(GetPName.ATI_TEXTURE_FREE_MEMORY, out texture_free_memory);
                    //Renderer.GetInteger(GetPName.ATI_RENDERBUFFER_FREE_MEMORY, out renderbuffer_free_memory);

                    //ret = /*TotalVideoRam - */(vbo_free_memory + texture_free_memory + renderbuffer_free_memory);
                    ret = texture_free_memory;
                    break;
            }

            return ret / 1024; // Value in MBytes
        }
        /// <summary>
        /// Returns the TickCount since the start of the engine
        /// </summary>
        /// <returns></returns>
        public long GetElapsedTime()
        {
            return (DateTime.Now.Ticks - VAR_timeElapsed);
        }
    }
}
