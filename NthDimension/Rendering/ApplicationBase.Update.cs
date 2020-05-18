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

using System.Collections.Generic;

namespace NthDimension.Rendering
{
    using System.Threading.Tasks;
    using NthDimension.Rendering.Configuration;
    using NthDimension.Rendering.Utilities;

    public partial class ApplicationBase
    {
        //public string               AssetsPath              = string.Empty;
        //public string               ExecutablePath          = string.Empty;

        public volatile bool        _assetsLoaded               = false;
        public volatile bool        _readyToPlay                = false;
        public volatile bool        _engineStarted              = false;

        private bool                assetsTaskStarted           = false;
        private volatile bool                resizeFramebuffers          = true;

        public void ReloadAssets()
        {
            assetsTaskStarted         = false;    // Should be false -ok
            _assetsLoaded             = false;    // Should be false -ok
            LoadAssets                = true;     // Should be true  -ok          // Trigger Loading of assets
            VAR_AppState              = ApplicationState.AssetLoad;
        }

        private float percentageA = 0;
        private float percentageB = 0;
        private float percentageC = 0;
        private float percentageD = 0;
        private float percentageE = 0;
        private float percentageF = 0;
        private float percentageH = 0;

        protected override void OnUpdateFrame(OpenTK.FrameEventArgs e)
        {
            VAR_FrameTime += (float)e.Time;
            VAR_FrameTime_Last = (float)e.Time;

            if(resizeFramebuffers)
            {
                resizeFramebuffers = false;

                //if (!Context.IsCurrent)
                //{
                //    Context.MakeCurrent(this.WindowInfo);
                //    Context.LoadAll();
                //}

                //Renderer.Viewport(0, 0, this.Width, this.Height);


                this.CreateFramebuffers();

                if (null != Scene)
                    Scene.OnResize();

                if(null != Player)
                    Player.OnResize();
            }

            this.uploadTexturesToGpu();
            this.uploadMeshesToGpu();
            

            #region Login

            if (VAR_AppState == ApplicationState.Login)
            {

            }
            #endregion Login
            #region Inititalize Loading
            else if (VAR_AppState == ApplicationState.AssetLoad)
            {
                //this.OnLoad(e);
                if (this.LoadAssets && !assetsTaskStarted)
                {
                    assetsTaskStarted = true;
                    //string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                    //              System.IO.Directory.SetCurrentDirectory(DirectoryUtil.Documents_MySoci);

                    if (!Settings.Instance.game.generateCache)                                                                      // Comment this line to use GPU Perf Studio
                    {
                        try
                        {

                            ScanNewFiles = true;
                            Task.Run(() =>    // Access Violation on OpenTK.GenBuffers on some dae models when building cache           // Comment this line to use GPU Perf Studio
                            {
                                try
                                {
                                    this.doLoadAssets_MTA();
                                }
                                catch(System.Exception mtaE)
                                {
                                    Utilities.ConsoleUtil.errorlog(string.Format("doLoadAssetsMTA Failed\n{0}\n\n", mtaE.Message), mtaE.StackTrace);
                                    //this.doLoadAssets_STA();
                                }
                                finally
                                {
                                    //BaseAssetsLoaded();
                                }
                            }
                            );
                        }
                        catch (System.Exception ldaE)
                        {
                            Utilities.ConsoleUtil.errorlog(string.Format("doLoadAssetsMTA Failed", ldaE.Message), ldaE.StackTrace);
                        }

                    }
                    // Comment this line to use GPU Perf Studio
                    else                                                                                                            // Comment this line to use GPU Perf Studio
                    {
                        try
                        {
                            this.doLoadAssets_STA();
                        }
                        catch (System.Exception ldaE)
                        {
                            Utilities.ConsoleUtil.errorlog(string.Format("doLoadAssetsSTA Failed", ldaE.Message), ldaE.StackTrace);
                        }
                        finally
                        {
                            //BaseAssetsLoaded();
                        }
                    }

                }
            }

            #endregion
            #region Game Playing | Menu
            else if (VAR_AppState == ApplicationState.Playing || VAR_AppState == ApplicationState.Pause)
            {
                if(!IsIntegratedGpu)
                    this.updateVideoPlaybackMaterials();

                Scene.Update(); // Note: Do not use Scene.Update as Drawable override here
            }
            #endregion
            #region Engine Started
            else if (VAR_AppState == ApplicationState.ContextActive)
            {
                this.CreateFramebuffers();
                this.GuiCreate();
                this.RegisterInputHandlers();
                this.VAR_AppState = ApplicationState.Login;   //this.GameState       = GameState.InitLoading;
            }
            #endregion

            UpdateUI(VAR_FrameTime);
        }
        protected override void OnResize(System.EventArgs e)
        {
            resizeFramebuffers      = true;
            VAR_SkipFrame           = true;
            VAR_ScreenSize          = VAR_ScreenSize_Current 
                                    = new Algebra.Vector2(this.Width, 
                                                          this.Height);

            Settings.Instance.video.windowWidth         = this.Width;
            Settings.Instance.video.windowHeight        = this.Height;

            //Renderer.Viewport(0, 0, this.Width, this.Height);

            //if (null != this.Context && null != this.WindowInfo)
            //{
            //    if (this.Context.IsCurrent)
            //        this.Context.Update(this.WindowInfo);
            //    else if (!this.Context.IsCurrent)
            //    {
            //        this.Context.MakeCurrent(this.WindowInfo);
            //        this.Context.LoadAll();
            //    }
            //}



            //Renderer.Viewport(0, 0, this.Width, this.Height);

            if (null != FBO_Scene)
                FBO_Scene.MarkForDelete = true;

            ////if (null != Scene)
            ////    Scene.OnResize();

            VAR_SkipFrame           = false;
        }

        private void updateVideoPlaybackMaterials()
        {
            foreach (KeyValuePair<NthDimension.FFMpeg.VideoSource, Material> v in VideoSources)
            {
                try
                {
                    //if (!v.Key.Created)
                    //    v.Key.InitializeSource();

                    if (v.Value.Textures[0].texture <= 0)
                    {
                        if (!v.Key.Created)
                            v.Key.InitializeSource();

                        string tname = v.Key.Path;
                        string strRemove = string.Format("{0}\\{1}", NthDimension.Utilities.DirectoryUtil.AssemblyDirectory, GameSettings.VideoFolder);
                        tname = tname.Replace(strRemove, string.Empty);

                        Texture t = new Texture();
                        if (null == v.Key.VideoPlayback) continue;
                        t.texture = v.Key.VideoPlayback.TextureHandle;
                        t.identifier = TextureLoader.textures.Count;
                        t.type = Texture.Type.fromPng;
                        t.pointer = t.name = tname;
                        //t.bitmap = new System.Drawing.Bitmap(v.Key.VideoPlayback.Size.Width, v.Key.VideoPlayback.Size.Height);
                        t.loaded = true;

                        TextureLoader.registerTexture(t);

                        v.Value.setTexture(Material.TexType.baseTexture, t);

                    }

                    if (v.Key.State == FFMpeg.PlayState.Playing)
                    {
                        v.Value.Textures[0].texture = v.Key.VideoPlayback.TextureHandle;
                        v.Key.Update();

                        float progress = (float)(v.Key.PlayingOffset.TotalMilliseconds / v.Key.FileLength.TotalMilliseconds);

                        //if(progress >= 1f)
                        //{
                        //    v.Key.Stop();
                        //    v.Key.Play();
                        //}

                        //ConsoleUtil.log(string.Format("Video Frame: {2} Frames: {0} Duration {1}", v.Key.VideoPlayback.PlayedFrameCount,
                        //                                                               v.Key.FrameDuration, progress), false);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        public virtual void UpdateUI(double time) { }
        public virtual void ResetUI(List<int> imagesToDelete) { }

        #region loading screen progress
        public void LoadingPercentageReset()
        {
            VAR_SceneLoadPercentage = 0f;
            percentageA = 0f;
            percentageB = 0f;
            percentageC = 0f;
            percentageD = 0f;
            percentageE = 0f;
            percentageF = 0f;
            percentageH = 0f;
        }

        void loadingMeshProgress(float progress)
        {
            percentageA = progress;
            VAR_SceneLoadPercentage = (percentageA + percentageB + percentageC + percentageD + percentageE + percentageF + percentageH) / 7f;
            //this.OnRenderFrame(new OpenTK.FrameEventArgs(FrameTime));     // Do not render, will corrupt OpenGL state and receive identity 0 when GenBuffers
        }
        public void loadingShaderProgress(float progress)
        {
            percentageB = progress;
            VAR_SceneLoadPercentage = (percentageA + percentageB + percentageC + percentageD + percentageE + percentageF + percentageH) / 7f;
            //this.OnRenderFrame(new OpenTK.FrameEventArgs(FrameTime));
        }
        void loadingTextureProgress(float progress)
        {
            percentageC = progress;
            VAR_SceneLoadPercentage = (percentageA + percentageB + percentageC + percentageD + percentageE + percentageF + percentageH) / 7f;
            //this.OnRenderFrame(new OpenTK.FrameEventArgs(FrameTime));
        }
        public void loadingMaterialsProgress(float progress)
        {
            percentageD = progress;
            VAR_SceneLoadPercentage = (percentageA + percentageB + percentageC + percentageD + percentageE + percentageF + percentageH) / 7f;
            this.OnRenderFrame(new OpenTK.FrameEventArgs(VAR_FrameTime));
        }
        public void loadingTemplatesProgress(float progress)
        {
            percentageE = progress;
            VAR_SceneLoadPercentage = (percentageA + percentageB + percentageC + percentageD + percentageE + percentageF + percentageH) / 7f;
            this.OnRenderFrame(new OpenTK.FrameEventArgs(VAR_FrameTime));
        }
        public void loadingCacheFilesProgress(float progress)
        {
            percentageF = progress;
            VAR_SceneLoadPercentage = (percentageA + percentageB + percentageC + percentageD + percentageE + percentageF + percentageH) / 7f;
            //this.OnRenderFrame(new OpenTK.FrameEventArgs(FrameTime));
        }
        public void setLoadingProgress(float progress)
        {
            percentageH = progress;
            VAR_SceneLoadPercentage = (percentageA + percentageB + percentageC + percentageD + percentageE + percentageF + percentageH) / 7f;
            this.OnRenderFrame(new OpenTK.FrameEventArgs(VAR_FrameTime));
        }
        #endregion

        private object _lockGpuTextureUpload = new object();

        /// <summary>
        /// Uploads loaded PNGs onto the GPU. For multithreaded assets loading
        /// </summary>
        private void uploadTexturesToGpu()
        {
            if (TextureLoader.Pending.Count > 0)
            {
                lock (_lockGpuTextureUpload)
                {
                    List<Texture> toGpu = new List<Texture>();

                    //toGpu.AddRange(TextureLoader.Pending);    // CopyTo(T array) throws exceptions sometimes

                    try
                    {
                        for (int tg = 0; tg < TextureLoader.Pending.Count; tg++)
                            toGpu.Add(TextureLoader.Pending[tg]);
                    }
                    catch
                    { }

                    try
                    {
                        foreach (Texture t in toGpu)
                        {
                            TextureLoader.UploadPngToGpu(t);
                            // TODO: DDS
                            TextureLoader.Pending.Remove(t);
                        }
                    }
                    catch { }
                }

            }
        }

        private object _lockGpuMeshUpload = new object();

        /// <summary>
        /// Uploads loaded meshes onto the GPU. For multithreaded assets loading
        /// </summary>
        private void uploadMeshesToGpu()
        {

            if (MeshLoader.Pending.Count > 0)
            {
                lock (_lockGpuMeshUpload)
                {
                    List<Geometry.MeshVbo> toGpu = new List<Geometry.MeshVbo>();

                    try
                    {
                        //toGpu.AddRange(MeshLoader.Pending);

                        for (int tg = 0; tg < MeshLoader.Pending.Count; tg++)
                            toGpu.Add(MeshLoader.Pending[tg]);
                    }
                    catch { }

                    try
                    {
                        foreach (Geometry.MeshVbo m in toGpu)
                        {
                            Geometry.MeshVbo mRef = m;

                            try
                            {
                                mRef.CurrentLod = Geometry.MeshVbo.MeshLod.Level3;
                                mRef.MeshData.ContainsVbo = false;
                                MeshLoader.GenerateVBO(ref mRef,
                                    Geometry.MeshVbo.MeshLod.Level3); // Note: What about the rest LOD levels?
                            }
                            catch { }

                            try
                            {
                                mRef.CurrentLod = Geometry.MeshVbo.MeshLod.Level2;
                                mRef.MeshData.ContainsVbo = false;
                                MeshLoader.GenerateVBO(ref mRef,
                                    Geometry.MeshVbo.MeshLod.Level2); // Note: What about the rest LOD levels?
                            }
                            catch { }

                            try
                            {
                                mRef.CurrentLod = Geometry.MeshVbo.MeshLod.Level1;
                                mRef.MeshData.ContainsVbo = false;
                                MeshLoader.GenerateVBO(ref mRef,
                                    Geometry.MeshVbo.MeshLod.Level1); // Note: What about the rest LOD levels?
                            }
                            catch { }

                            try
                            {
                                mRef.CurrentLod = Geometry.MeshVbo.MeshLod.Level0;
                                mRef.MeshData.ContainsVbo = false;
                                MeshLoader.GenerateVBO(ref mRef,
                                    Geometry.MeshVbo.MeshLod.Level0); // Note: What about the rest LOD levels?
                            }
                            catch { }

                            MeshLoader.Pending.Remove(m);
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
