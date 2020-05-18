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

#define COMPUTE_ENABLED
#define NETWORK_ENABLED


using System.Diagnostics;


namespace NthDimension.Rendering
{
    using System;
    using System.IO;
    
    using OpenTK.Graphics;

    using NthDimension.Algebra;
    using NthDimension.Rasterizer;
    using NthDimension.Rendering.Configuration;
    using NthDimension.Rendering.Drawables;
    using NthDimension.Rendering.Drawables.Framebuffers;
    using NthDimension.Rendering.Geometry;
    using NthDimension.Rendering.Loaders;
    using NthDimension.Rendering.Scenegraph;
    using NthDimension.Rendering.Utilities;
    using NthDimension.Utilities;
    using System.Threading.Tasks;
    using NthDimension.FFMpeg;

    public partial class ApplicationBase
    {
        private static string           serverPayloadFile          = "payload.dat";

        public volatile bool            ScanNewFiles               = true;

        private IGraphicsContext        ctx_MTA;

        public bool                     IsIntegratedGpu
        {
            get { return DeviceVendor == DeviceVendor.INTEL; }
        }

        #region Load

        //private bool _engineInitialized = false;

        protected override void OnLoad(EventArgs e)
        {
            //Matrix4 m = Matrix4.CreateRotationX(135);
            //Console.WriteLine(m.ToString());
            //Console.ReadLine();



            #region Renderer
            VSync                           = OpenTK.VSyncMode.Off;
            GraphicsContext.ShareContexts   = true;
            Context.ErrorChecking           = Environment.UserInteractive;
            VAR_StopWatch                   = new Stopwatch();
            VAR_ScreenSize_Virtual          = Settings.Instance.video.CreateSizeVector(VideoSettings.Target.main);
            VAR_AppState                    = new ApplicationState();
            VAR_ScreenSize                  = new Vector2(Width, Height);
            //ExecutablePath                = AssetsPath                  
            //                              =  System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            this.VAR_VersionOpenGL          = Renderer.GetString(StringName.Version);
            this.VAR_VersionOpenGLShader    = Renderer.GetString(StringName.ShadingLanguageVersion);
            this.VAR_deviceOpenGL           = Renderer.GetString(StringName.Renderer);
            this.VAR_vendorOpenGL           = Renderer.GetString(StringName.Vendor);

            this.Renderer.GetInteger(NthDimension.Rasterizer.GetPName.MaxTextureSize,                   out ApplicationBase.DeviceLimits.Renderer_MAX_TEXTURE_SIZE);
            //this.Renderer.GetInteger(NthDimension.Rasterizer.GetPName.RedBits,                          out ApplicationBase.DeviceLimits.Renderer_RED_BITS);
            //this.Renderer.GetInteger(NthDimension.Rasterizer.GetPName.GreenBits,                        out ApplicationBase.DeviceLimits.Renderer_GREEN_BITS);
            //this.Renderer.GetInteger(NthDimension.Rasterizer.GetPName.BlueBits,                         out ApplicationBase.DeviceLimits.Renderer_BLUE_BITS);
            //this.Renderer.GetInteger(NthDimension.Rasterizer.GetPName.AlphaBias,                        out ApplicationBase.DeviceLimits.Renderer_ALPHA_BITS);
            this.Renderer.GetInteger(NthDimension.Rasterizer.GetPName.DepthBits,                        out ApplicationBase.DeviceLimits.Renderer_DEPTH_BITS);
            this.Renderer.GetInteger(NthDimension.Rasterizer.GetPName.StencilBits,                      out ApplicationBase.DeviceLimits.Renderer_STENCIL_BITS);
            this.Renderer.GetInteger(NthDimension.Rasterizer.GetPName.MaxVertexAttribs,                 out ApplicationBase.DeviceLimits.Shader_MAX_VERTEX_ATTRIBS);
            this.Renderer.GetInteger(NthDimension.Rasterizer.GetPName.MaxVertexUniformComponents,       out ApplicationBase.DeviceLimits.Shader_MAX_VERTEX_UNIFORMS);
            this.Renderer.GetInteger(NthDimension.Rasterizer.GetPName.MaxFragmentUniformComponents,     out ApplicationBase.DeviceLimits.Shader_MAX_FRAGMENT_UNIFORMS);
            this.Renderer.GetInteger(NthDimension.Rasterizer.GetPName.MaxVaryingComponents,             out ApplicationBase.DeviceLimits.Shader_MAX_VARYING_COMPONENTS);
            this.Renderer.GetInteger(NthDimension.Rasterizer.GetPName.MaxVertexTextureImageUnits,       out ApplicationBase.DeviceLimits.Shader_MAX_VERTEX_TEXTURE_UNITS);
            this.Renderer.GetInteger(NthDimension.Rasterizer.GetPName.MaxTextureImageUnits,             out ApplicationBase.DeviceLimits.Shader_MAX_FRAGMENT_TEXTURE_UNITS);


            if (this.VAR_vendorOpenGL.ToUpper().Contains("ATI"))
                this.DeviceVendor = DeviceVendor.ATI;
            if (this.VAR_vendorOpenGL.ToUpper().Contains("NVIDIA"))
                this.DeviceVendor = DeviceVendor.NVIDIA;
            if (this.VAR_vendorOpenGL.ToUpper().Contains("INTEL"))
                this.DeviceVendor = DeviceVendor.INTEL;

            this.Renderer.ClearColor(this.VAR_ScreenColor.ToColor4());

#if COMPUTE_ENABLED
            foreach (var p in Compute.ComputePlatform.Platforms)
                foreach (var d in p.Devices)
                    //if (d.Vendor.ToUpperInvariant().Contains("NVIDIA")) // "INTEL")) // "AMD"))
                    if (d.Vendor.ToUpperInvariant().Contains("ADVANCED MICRO DEVICES")) // "INTEL")) // "AMD"))
                    {
                        _computer = d;
                        break;
                    }
#endif

#if _WINDOWS_
            ConsoleUtil.ANSIEffects();
#endif

            ConsoleUtil.log(string.Format("NthDimension Real-Time 3D Rendering {0}", "5.0.1 rev. 7" /*System.Reflection.Assembly.GetExecutingAssembly().ImageRuntimeVersion*/));
            ConsoleUtil.log(string.Format("(C) Copyright {0}, All Rights Reserved{1}SYSCON Technology, Hellas", DateTime.Now.Year, Environment.NewLine));
            ConsoleUtil.log(string.Empty);
            ConsoleUtil.log("[\\\\Device ]");
            ConsoleUtil.log(string.Empty);
            ConsoleUtil.log(String.Format("   Device Vendor                    : {0}", VAR_vendorOpenGL));
            ConsoleUtil.log(String.Format("   Device Model                     : {0}", VAR_deviceOpenGL));
            
            if (_computer != null)
            {                
                ConsoleUtil.log(String.Format("   Device                           : {0}", _computer.Available ?
                                  string.Format("{1} {2}bit, {0} [GPU Compute Available(*)]", _computer.Vendor.Trim(new char[] { ' ', '\t' }), _computer.Name.Trim(new char[] { ' ', '\t' }), _computer.AddressBits) :
                                  @"N/A"));
                ConsoleUtil.log(String.Format("   Total Device Memory              : {0} MByte", (VAR_TotalVideoRam = _computer.MaxMemoryAllocationSize) / 1024 / 1024));
            }
            else
                ConsoleUtil.log(String.Format("   Total Device Memory              : {0} MByte", VAR_TotalVideoRam = GetAvailableDeviceMemoryBytes()/* / 1024 / 1024*/));

            ConsoleUtil.log(string.Empty);
            ConsoleUtil.log(String.Format("   Graphics Interface Version       : OpenGL {0}", VAR_VersionOpenGL));
            ConsoleUtil.log(String.Format("   Shader Capability                : GLSL {0}", VAR_VersionOpenGLShader));
           
            ConsoleUtil.log(string.Empty);
          
            //ConsoleUtil.log("--- Device Limits ");
            ConsoleUtil.log(String.Format("   Max Texture Size                 : {0} pixel(s)", ApplicationBase.DeviceLimits.Renderer_MAX_TEXTURE_SIZE));
            //ConsoleUtil.log(String.Format("   Max Color Bits                   : {0}", ApplicationBase.DeviceLimits.Renderer_RED_BITS));
            ConsoleUtil.log(String.Format("   Max Depth Bits                   : {0}", ApplicationBase.DeviceLimits.Renderer_DEPTH_BITS));
            ConsoleUtil.log(String.Format("   Max Stencil Bits                 : {0}", ApplicationBase.DeviceLimits.Renderer_STENCIL_BITS));
            ConsoleUtil.log(String.Format("   Max Shader Vertex Attribs        : {0}", ApplicationBase.DeviceLimits.Shader_MAX_VERTEX_ATTRIBS));
            ConsoleUtil.log(String.Format("   Max Shader Vertex Uniforms       : {0}", ApplicationBase.DeviceLimits.Shader_MAX_VERTEX_UNIFORMS));
            ConsoleUtil.log(String.Format("   Max Shader Vertex Textures       : {0}", ApplicationBase.DeviceLimits.Shader_MAX_VERTEX_TEXTURE_UNITS));
            ConsoleUtil.log(String.Format("   Max Shader Fragment Uniforms     : {0}", ApplicationBase.DeviceLimits.Shader_MAX_FRAGMENT_UNIFORMS));
            ConsoleUtil.log(String.Format("   Max Shader Fragment Textures     : {0}", ApplicationBase.DeviceLimits.Shader_MAX_FRAGMENT_TEXTURE_UNITS));
            ConsoleUtil.log(String.Format("   Max Shader Varying Components    : {0}", ApplicationBase.DeviceLimits.Shader_MAX_VARYING_COMPONENTS));
            if (_computer != null && _computer.Available)
            {
                ConsoleUtil.log(string.Empty);
                ConsoleUtil.log(String.Format(" * Parallel-GPU Compute active      : OpenCL {0}", _computer.DriverVersion));
                ConsoleUtil.log(String.Format("   GPU Computer-Vision driver       : {0}", _computer.OpenCLCVersionString));
                ConsoleUtil.log(String.Format("   Max GPU Compute units            : {0}", _computer.MaxComputeUnits));
            }
           
            ConsoleUtil.log(String.Empty);
            ConsoleUtil.log("[\\\\Config ]");
            ConsoleUtil.log(string.Empty);
            if (Settings.Instance.video.highResolution)
                ConsoleUtil.log(String.Format("{0} High Resolution Mode Enabled!{0}", Environment.NewLine));
            ConsoleUtil.log(string.Format("   Window Size        : {0}x{1}", Settings.Instance.video.windowWidth, Settings.Instance.video.windowHeight));
            ConsoleUtil.log(string.Format("   Render Size        : {0}x{1}", Settings.Instance.video.virtualScreenWidth, Settings.Instance.video.virtualScreenHeight));
            ConsoleUtil.log(string.Format("   Fullsceen          : {0}", Settings.Instance.video.fullScreen ? "Yes" : "No"));
            ConsoleUtil.log(string.Format("   Target Perf.       : {0} FPS", Settings.Instance.video.targetFrameRate));
            ConsoleUtil.log(string.Format("   Post Process       : {0}", Settings.Instance.video.postProcessing ? "Yes" : "No"));
            ConsoleUtil.log(string.Format("   Smooth Lightmap    : {0}", Settings.Instance.video.lightmapSmoothing ? "Yes" : "No"));
            if (Settings.Instance.video.useShadows)
            {
                ConsoleUtil.log(string.Format("   Shadows            : {0}", Settings.Instance.video.useShadows ? "Yes" : "No"));
#if FBOSHADOW_DEBUG
                ConsoleUtil.log(string.Format("   Debug PSSM         : {0}", Settings.Instance.video.pssmShadows ? "Yes" : "No"));
#endif                
                ConsoleUtil.log(string.Format("   Shadow Quality     : {0}", Settings.Instance.video.shadowQuality));
                ConsoleUtil.log(string.Format("   Shadowmap Res.     : {0}x{0} Far {1}x{1} Near", Settings.Instance.video.shadowResolution, Settings.Instance.video.shadowResolution * 2));
            }
            ConsoleUtil.log(string.Format("   Effects Quality    : {0}", Settings.Instance.video.effectsQuality));
            ConsoleUtil.log(string.Format("   Effects Update     : {0} sec", Settings.Instance.video.effectsUpdateInterval));
            ConsoleUtil.log(string.Format("   SSAO               : {0}", Settings.Instance.video.ssAmbientOccluison ? "Yes" : "No"));
            ConsoleUtil.log(string.Format("   Bloom              : {0}", Settings.Instance.video.bloom ? "Yes" : "No"));
            ConsoleUtil.log(string.Format("   DOF                : {0}", Settings.Instance.video.depthOfField ? "Yes" : "No"));
            ConsoleUtil.log(String.Format("   VR/AR Stereo       : {0}", Context.GraphicsMode.Stereo ? "Yes" : "No"));
            ConsoleUtil.log(string.Empty);
            ConsoleUtil.log("[\\\\Application Start]");
            ConsoleUtil.log(string.Empty);
            ConsoleUtil.log(string.Format("   Machine            : {0}", Environment.MachineName));
            ConsoleUtil.log(string.Format("   User               : {0}", Environment.UserName));
            ConsoleUtil.log(string.Format("   Date               : {0}", DateTime.Now.ToString()));
            ConsoleUtil.log(string.Empty);
            ConsoleUtil.log(String.Format("   OpenGL Context (Id {0}):", Context.GraphicsMode.Index));
            ConsoleUtil.log(String.Format("                        Color          {0} bit", Context.GraphicsMode.ColorFormat.BitsPerPixel));
            ConsoleUtil.log(String.Format("                        Depth          {0} bit", Context.GraphicsMode.Depth));
            ConsoleUtil.log(String.Format("                        Stencil        {0} bit", Context.GraphicsMode.Stencil));
            ConsoleUtil.log(String.Format("                        Samples        {0}", Context.GraphicsMode.Samples));
            ConsoleUtil.log(String.Format("                        Buffers        {0}", Context.GraphicsMode.Buffers));
            ConsoleUtil.log(String.Format("                        Accum. Format  {0}", Context.GraphicsMode.AccumulatorFormat));
            ConsoleUtil.log(String.Format("                        Error Checks   {0}{1}", Environment.UserInteractive ? "On" : "Off", Environment.NewLine));
            ConsoleUtil.log(string.Empty);
            //ConsoleUtil.log(string.Empty);

           
            ConsoleUtil.log(              "   ** Staring OpenAL Audio **");
            Audio.Create();
            ConsoleUtil.log(String.Format("   \t{0}", Audio.DeviceName));
            ConsoleUtil.log(string.Empty);

            #region Instanciate Loaders & Scene
            ConsoleUtil.log(              "   ** Staring ffmpeg Video **");
            VideoGL3x.SetupOpenGL(_renderer, _audio);
            ConsoleUtil.log(string.Format("   \tVideo: {0}", VideoGL3x.Renderer == null ? "Failed": "GL ffmpeg OK!"));
            ConsoleUtil.log(string.Format("   \tAudio: {0}", VideoGL3x.Audio == null ? "Failed" : "GL ffmpeg OK!"));
            ConsoleUtil.log(string.Empty);

            ConsoleUtil.log(              "   ** Staring Script Compiler **");
            this.CreateCompiler();
            ConsoleUtil.log(string.Empty);

            TextureLoader           = new TextureLoader();
            FramebufferCreator      = new FramebufferCreator(this.Width, this.Height);
            MeshLoader              = new MeshLoader();
            AnimationLoader         = new AnimationLoader();
            ShaderLoader            = new ShaderLoader();
            MaterialLoader          = new MaterialLoader();
            TemplateLoader          = new TemplateLoader();
            
            Scene                   = new SceneGame();

#endregion
            //MeshLoader.SetConsoleCtrlHandler(new MeshLoader.HandlerRoutine(MeshLoader.ConsoleCtrlCheck), true);
            // set files for displaying loading screen
            ShaderLoader.FromXmlFile(   Path.Combine(GameSettings.ShaderFolder, "composite.xsp"));
            ShaderLoader.FromTextFile(  Path.Combine(GameSettings.ShaderFolder, "composite.vs"),
                                        Path.Combine(GameSettings.ShaderFolder, "splash_shader.fs"));

            ShaderLoader.LoadShaders();

            MeshLoader.FromObj(Path.Combine(GameSettings.ModelBaseFolder, "sprite_plane.obj"));
            MeshLoader.FromObj(Path.Combine(GameSettings.ModelBaseFolder, "shadow_plane.obj"));

#if !_DEVUI_
            MeshLoader.LoadMeshes();
#endif

            TextureLoader.fromPng(Path.Combine(GameSettings.TextureBaseFolder, "engine_back.png"), true);
            TextureLoader.fromPng(Path.Combine(GameSettings.TextureBaseFolder, "engine_back_h.png"), true);
            TextureLoader.fromPng(Path.Combine(GameSettings.TextureBaseFolder, "noise_pixel.png"), false);                //loading noise manualy so we can disable multisampling

#if !_DEVUI_
            TextureLoader.LoadTextures();
#endif

            MaterialLoader.FromXmlFile(Path.Combine(GameSettings.MaterialFolder, "composite.xmf"));

#if !_DEVUI_
            MaterialLoader.LoadMaterials();
#endif

            Filter2D_Splash = new Quad2d();                                                                 // setup 2d filter (loading screen)

            VAR_timeElapsed = GetElapsedTime();                                                                   // set time to zero

           

            _renderer.Flush();
            #endregion Renderer

            #region Network
#if NETWORK_ENABLED
            Server = new Server.NServer(VAR_AppTitle, mServerIpEndPoint, VAR_NETWORK_MAX_USERS);
#endif
#endregion
        }
#endregion
        
        private void doLoadAssets_MTA()
        {
            if(null == ctx_MTA)
                ctx_MTA = new GraphicsContext(this.Context.GraphicsMode, this.WindowInfo);
            ctx_MTA.LoadAll();
            ctx_MTA.MakeCurrent(this.WindowInfo);
            

            if (Settings.Instance.game.useCache)
            {

#if !_DEVUI_

                this.serverSyncCache();

#region READ CACHE FILES

                VAR_StopWatch.Restart();
                ConsoleUtil.log("============ Reading Cache File ===========");
                this.loadingCacheFilesProgress(0.16f);
                this.readMeshesCache();         this.loadingCacheFilesProgress(0.32f);      VAR_StopWatch.Restart();
                this.readAnimationsCache();     this.loadingCacheFilesProgress(0.48f);      VAR_StopWatch.Restart();
                this.readShadersCache();        this.loadingCacheFilesProgress(0.64f);      VAR_StopWatch.Restart();
                this.readTexturesCache();       this.loadingCacheFilesProgress(0.80f);      VAR_StopWatch.Restart();
                this.readMaterialsCache();      this.loadingCacheFilesProgress(0.96f);      VAR_StopWatch.Restart();
                this.readTemplatesCache();      this.loadingCacheFilesProgress(1f);         VAR_StopWatch.Stop();
                ConsoleUtil.log("===========================================");
#endregion

#endif

                foreach (MeshVbo m in Scene.meshes)
                    m.CurrentLod = MeshVbo.MeshLod.Level0;

                percentageA = 1f;
            }


            if (ScanNewFiles)
            {
                this._fileSeeker = new FileSeeker(/*Path.Combine(DirectoryUtil.AssemblyDirectory, @"data")*/);
                this._fileSeeker.LoadAllFiles();
            }

#region PUT Assets into RAM

            // ACCESS VIOLATIONS due to Threading
            try
            {
                // Note: Models Should be Loaded from the Main UI Thread, otherwise VAOs are not constructed
                ctx_MTA.MakeCurrent(this.WindowInfo);
                this.MeshLoader.LoadMeshes(loadingMeshProgress);
                //this.writeMeshesCache();                                              // ONLY on STA Thread
                //this.writeAnimationsCache();                                          // ONLY on STA Thread
                ctx_MTA.MakeCurrent(this.WindowInfo);
                this.TextureLoader.LoadTextures(loadingTextureProgress);
                //this.writeTexturesCache();                                            // ONLY on STA Thread
                //ctx.MakeCurrent(this.WindowInfo);                                     // Loaded lated in CreateScene  
                loadingShaderProgress(1f);
                //this.ShaderLoader.LoadShaders(loadingShaderProgress);                 // Loaded lated in CreateScene
                //this.writeShadersCache();                                             // Loaded lated in CreateScene
                //ctx.MakeCurrent(this.WindowInfo);                                     // Loaded lated in CreateScene
                //loadingMaterialsProgress(1f);
                //this.MaterialLoader.LoadMaterials(loadingMaterialsProgress);          // Loaded lated in CreateScene
                //this.writeMaterialsCache();
                //ctx_MTA.MakeCurrent(this.WindowInfo);
                //this.TemplateLoader.loadTemplates(loadingTemplatesProgress);
                //this.writeTemplatesCache();                                           // ONLY on STA Thread
                //Renderer.Finish(); // TODO:: GlFinish() here to avoid textre corruption

                //this._fileSeeker = new FileSeeker(DirectoryUtil.Documents);
                //if(ScanNewFiles)
                //    this._fileSeeker.LoadAllFiles();

#endregion PUT Assets into RAM

                _assetsLoaded = true;

                if (null != OnAssetsLoaded)
                    this.OnAssetsLoaded();
            }
#pragma warning disable CS0168
            catch (Exception aE)
            {
                
            }

            ctx_MTA = null;

        }
        private void doLoadAssets_STA()
        {
            if (Settings.Instance.game.useCache)
            {

#if !_DEVUI_

                this.serverSyncCache();

#region READ CACHE FILES

                VAR_StopWatch.Restart();
                ConsoleUtil.log("============ Reading Cache File ===========");
                this.loadingCacheFilesProgress(0.16f);
                this.readMeshesCache();         this.loadingCacheFilesProgress(0.32f);  VAR_StopWatch.Restart();
                this.readAnimationsCache();     this.loadingCacheFilesProgress(0.48f);  VAR_StopWatch.Restart();
                this.readShadersCache();        this.loadingCacheFilesProgress(0.64f);  VAR_StopWatch.Restart();
                this.readTexturesCache();       this.loadingCacheFilesProgress(0.80f);  VAR_StopWatch.Restart();
                this.readMaterialsCache();      this.loadingCacheFilesProgress(0.96f);  VAR_StopWatch.Restart();
                this.readTemplatesCache();      this.loadingCacheFilesProgress(1f);     VAR_StopWatch.Stop();
                ConsoleUtil.log("===========================================");
#endregion

#endif

                foreach (MeshVbo m in Scene.meshes)
                    m.CurrentLod = MeshVbo.MeshLod.Level0;

                percentageA = 1f;
            }


            
            if (ScanNewFiles)
            {
                this._fileSeeker = new FileSeeker(/*Path.Combine(DirectoryUtil.AssemblyDirectory, @"data")*/);
                this._fileSeeker.LoadAllFiles();
            }

#region PUT Assets into RAM

            // ACCESS VIOLATIONS due to Threading

            this.TextureLoader.LoadTextures(loadingTextureProgress, true);
            this.writeTexturesCache();

            this.MeshLoader.LoadMeshes(loadingMeshProgress);
            this.writeMeshesCache();
            this.writeAnimationsCache();

            this.ShaderLoader.LoadShaders(loadingShaderProgress);
            this.writeShadersCache();

            this.MaterialLoader.LoadMaterials(loadingMaterialsProgress);
            this.writeMaterialsCache();

            this.TemplateLoader.loadTemplates(loadingTemplatesProgress);
            this.writeTemplatesCache();

            if(Settings.Instance.game.generateCache)
            {
                Settings.Instance.game.generateCache = false;
                Settings.Instance.SaveSettings(Path.Combine(DirectoryUtil.Documents, "settings.xml"));
            }

            
#endregion PUT Assets into RAM

            //this._fileSeeker = new FileSeeker(DirectoryUtil.Documents);
            //if(ScanNewFiles)
            //    this._fileSeeker.LoadAllFiles();

            _assetsLoaded = true;

            if (null != OnAssetsLoaded)
                this.OnAssetsLoaded();


        }

        //private void scanLocalFiles()
        //{
        //    this._fileSeeker = new FileSeeker(/*Path.Combine(DirectoryUtil.AssemblyDirectory, @"data")*/);
        //    if (ScanNewFiles)
        //        this._fileSeeker.LoadAllFiles();
        //}

        private bool serverSynced = false;
        private void serverSyncCache()
        {
            if (serverSynced) return;
            serverSynced = true;
#region Server Sync
            if (!offlineDebug)
            {
                //while (SynchronizedCache)
                //{
                //    // TODO:: Store latest zip file hash to settings
                //}

                if (File.Exists(serverPayloadFile))
                {
                    string execPath = FileSizeUtil.AssemblyDirectory;

                    ConsoleUtil.log(String.Format("Updating cache from Server..." + execPath));

                    while (FileInUse(serverPayloadFile))
                        System.Threading.Thread.Sleep(1);


                    ConsoleUtil.log(String.Format("Extracting file cache file, please wait..."));

                    CompressionUtil.ImprovedExtractToDirectory(Path.Combine(execPath, serverPayloadFile), execPath,
                        CompressionUtil.Overwrite.Always);

                    //Settings.Instance.game.lastCacheHash = MD5Tool.GetChecksum("mysocinet.dat");

                    File.Delete(serverPayloadFile);

                    this.WriteCacheInfo();
                }
            }
#endregion
        }
    }
}
