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

//#define THREADED_RENDERING

using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Geometry;
using NthDimension.Rendering.Shaders;
using NthDimension.Rasterizer;
using Vector2 = NthDimension.Algebra.Vector2;

namespace NthDimension.Rendering
{
    public partial class ApplicationBase
    {
        #region Draw

//private bool renderLock = false;
#if THREADED_RENDERING
        System.Threading.Thread             T_Render;
        OpenTK.Graphics.IGraphicsContext    T_Render_Context;
#endif

        protected override void OnRenderFrame(OpenTK.FrameEventArgs e)
        {
#if THREADED_RENDERING
            if (null == T_Render || !T_Render.IsAlive)
            {
                T_Render = new System.Threading.Thread(delegate() { doRender_MTA(e); } );
                T_Render.IsBackground = true;
                T_Render.Start();
            }
#else
            doRender_STA(e);
#endif
        }

//        protected void doRender_MTA(OpenTK.FrameEventArgs e)
//        {
//#region Playing | Game.Menu

//            if (GameState == GameState.Playing || GameState == GameState.Menu)
//            {
//                UISceneReady(true);
//                try
//                {

//                    double framerate = 1 / e.Time;
//                    smoothframerate = framerate * (1 - framerate_smoothness) + smoothframerate * framerate_smoothness;
//                    Scene.totalDrawCalls = 0;

//                    if (!Context.IsCurrent)
//                        Context.MakeCurrent(this.WindowInfo);

//                   // Renderer.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // Not present originally
//#if !_DEVUI_

//#if WATER_FBO // make water fb black
//                FBOSet_Water.OutputFb.enable(true);
//#endif

//#region SkyBox

//                    // refresh one of the 6 cubemap textures
//                    if (CurrentCBOSide > 5)
//                        CurrentCBOSide = 0;

//                    Scene.DrawMainFunction(CBOSet_Sky.FrameBufferSets[CurrentCBOSide],
//                        CBOSet_Sky.cubeView[CurrentCBOSide]);

//                    CurrentCBOSide++;

//#endregion

//#region Shadow Buffers

//                    //render shadowbuffers
//                    if (Settings.Instance.video.useShadows)
//                        //Scene.DrawShadowBuffer_PSSM(FBO_Shadow);
//                        Scene.DrawShadowBuffer_Simple(FBO_Shadow); // Created Feb-15-18 to match vanilla

//#endregion

//#if WATER_FBO // create viewInfo for water reflections
//                ViewInfoWater.projectionMatrix = Player.ViewInfo.projectionMatrix;
//                ViewInfoWater.modelviewMatrix = Matrix4.Mult(Scene.WaterMatrix, Player.ViewInfo.modelviewMatrix);
//                ViewInfoWater.invModelviewMatrix = Matrix4.Mult(Scene.WaterMatrix,
//                    Player.ViewInfo.invModelviewMatrix);
//                ViewInfoWater.pointingDirection = Vector3.Multiply(Player.ViewInfo.pointingDirection, yFlip);
//                ViewInfoWater.pointingDirectionUp = Vector3.Multiply(Player.ViewInfo.pointingDirectionUp, yFlip);
//                ViewInfoWater.pointingDirectionRight = Vector3.Multiply(Player.ViewInfo.pointingDirectionRight,
//                    yFlip);
//                ViewInfoWater.position = Vector3.Multiply(Player.ViewInfo.position, yFlip) + waterLevel * 2;
//                ViewInfoWater.GenerateViewProjectionMatrix();

//                // render water reflections
//                Renderer.CullFace(CullFaceMode.Front);
//                Scene.DrawFramebufferSet(FBOSet_Water, ViewInfoWater);
//#endif

//                    Renderer.CullFace(CullFaceMode.Back);

//#region Draw Scene

//                    //if (Player.PlayerViewMode == Player.enuPlayerViewMode.ThirdPerson)
//                    //    Player.ViewInfo.Position = Scene.EyePos;


//                    Scene.DrawMainFunction(FBOSet_Main, Player.ViewInfo);


//#endregion

//#endif
//                    Renderer.CullFace(CullFaceMode.Back);

//#region Draw Nano UI

//                    DrawUI(e.Time);



//#region Draw FPS UI

//                    // draw Guis
//#if !_DEVUI_
//                    if (Player.PlayerViewMode == Player.enuPlayerViewMode.FirstPerson)
//                        Scene.DrawGuis();
//#endif

//#endregion

//#endregion

//                    //SwapBuffers();
//                    Context.SwapBuffers();

//                    checkGlError("Render Loop Finished");

//                    Scene.resetUpdateState();


//                    this.checkDeviceLimitsOnce();
//                }
//                catch
//                {
//                }
//            }
//#endregion

//#region Loading Screen

//            else
//            {
//                UISceneReady(false);
//                try
//                {
//                    if (!this.Context.IsCurrent)
//                        this.Context.MakeCurrent(this.WindowInfo);

//                    //MakeCurrent();
//                    Renderer.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
//                    Renderer.CullFaceEnabled = false; // Renderer.Disable(EnableCap.CullFace);
//                    Renderer.Viewport(0, 0, Width, Height);

//                    Renderer.MatrixMode(MatrixMode.Projection);
//                    Renderer.LoadIdentity();
//                    Renderer.Frustum(-1f, 1f, -1f, 1f, 0.1f, 1f);

//                    if (ShowSplash)
//                    {
                        
//                        Vector2 splashValue = Vector2.One * loadingPercentage;
//                        Utilities.ConsoleUtil.log(splashValue.ToString());

//                        SplashFilter2D.draw(ShaderLoader.GetShaderByName("splash_shader.fs"), new int[]
//                           {
//                                TextureLoader.getTextureId("base\\engine_back.png"),
//                                TextureLoader.getTextureId("base\\engine_back_h.png")
//                           }, Uniform.in_vector,
//                                   splashValue);

//                    }
//                    //Vector2.One);


//                    DrawUI(e.Time);

//                    //SwapBuffers();
//                    this.Context.SwapBuffers();
//                }
//                catch { }

//            }

//#endregion
//        }

            
        protected void doRender_STA(OpenTK.FrameEventArgs e)
        {
            if (VAR_SkipFrame)
            {
                foreach (var video in VideoSources)
                {
                    if (video.Key.State != FFMpeg.PlayState.Playing)
                        video.Key.Pause();
                }
                return;
            } 

            #region Playing | Game.Menu

            if (VAR_AppState == ApplicationState.Playing || VAR_AppState == ApplicationState.Pause)
            {
                try
                {
                    #region Frame Statistics Reset
                    UISceneReady(true);
                    double framerate = 1 / e.Time;
                    VAR_Framerate_Smoothness = framerate * (1 - VAR_framerate_smoothness) + VAR_Framerate_Smoothness * VAR_framerate_smoothness;
                    Scene.DrawCallTotal = 0;
                    #endregion Frame Statistics Reset

                    #region Context Synchroniazation
                    if (!Context.IsCurrent)
                    {
                        Context.MakeCurrent(this.WindowInfo);
                        Context.LoadAll();
                    }
                    else
                    {
                        if (ApplicationBase.Instance.VAR_ScreenSize_Virtual.X < ApplicationBase.Instance.VAR_ScreenSize_Current.X ||
                            ApplicationBase.Instance.VAR_ScreenSize_Virtual.Y < ApplicationBase.Instance.VAR_ScreenSize_Current.Y)
                        {
                            ApplicationBase.Instance.VAR_ScreenSize_Virtual = ApplicationBase.Instance.VAR_ScreenSize_Current;
                            Context.Update(this.WindowInfo);
                        }
                    }
                    #endregion Context Synchronization

                    #region ffmpeg

                    foreach (var video in VideoSources)
                    {
                        if (video.Key.Created &&
                            video.Key.State != FFMpeg.PlayState.Playing &&
                            video.Value.loaded &&
                            video.Value.Textures[0].texture != 0)
                        {
                            //video.Key.OnVideoStopped += delegate ()
                            //{
                            //    Utilities.ConsoleUtil.log("Video Stopped");
                            //};

                            video.Key.Play();
                            //Utilities.ConsoleUtil.log(string.Format("Playing Video {0}", video.Key.Path));
                        }
                    }
                    #endregion

                    #region Deferred Pipeline

#if WATER_FBO // make water fb black
                    FBOSet_Water.OutputFb.enable(true);
#endif

                    #region SkyBox

                    // refresh one of the 6 cubemap textures
                    if (CBO_SkySide > 5)
                        CBO_SkySide = 0;

                    Scene.DrawBuffers(CBO_Sky.FrameBufferSets[CBO_SkySide],
                        CBO_Sky.cubeView[CBO_SkySide]);

                    CBO_SkySide++;

                    #endregion

                    #region Shadow Buffers

                    //render shadowbuffers
                    if (Settings.Instance.video.useShadows)
                    {
                        //if (Settings.Instance.video.pssmShadows)
                        //    Scene.DrawShadowBuffer_PSSM(FBO_ShadowPssm);    // FBO_ShadowPssm
                        //else
                            Scene.DrawShadowBuffer_Simple(FBO_Shadow);      // Created Feb-15-18 to match vanilla
                    }

#endregion

#if WATER_FBO // create viewInfo for water reflections
                ViewInfoWater.projectionMatrix          = Player.ViewInfo.projectionMatrix;
                ViewInfoWater.modelviewMatrix           = Matrix4.Mult(Scene.WaterMatrix, Player.ViewInfo.modelviewMatrix);
                ViewInfoWater.invModelviewMatrix        = Matrix4.Mult(Scene.WaterMatrix, Player.ViewInfo.invModelviewMatrix);
                ViewInfoWater.pointingDirection         = Vector3.Multiply(Player.ViewInfo.pointingDirection, yFlip);
                ViewInfoWater.pointingDirectionUp       = Vector3.Multiply(Player.ViewInfo.pointingDirectionUp, yFlip);
                ViewInfoWater.pointingDirectionRight    = Vector3.Multiply(Player.ViewInfo.pointingDirectionRight, yFlip);
                ViewInfoWater.position                  = Vector3.Multiply(Player.ViewInfo.position, yFlip) + waterLevel * 2;
                ViewInfoWater.GenerateViewProjectionMatrix();

                // render water reflections
                Renderer.CullFace(CullFaceMode.Front);
                Scene.DrawFramebufferSet(FBOSet_Water, ViewInfoWater);
#endif

                    Renderer.CullFace(CullFaceMode.Back);

                    Scene.DrawBuffers(FBO_Scene, Player.ViewInfo);

                    Renderer.CullFace(CullFaceMode.Back);
                    #endregion

                    #region Draw Nano UI
                    
                    GuiDraw(e.Time);



#region Draw FPS UI

                    // draw Guis
#if !_DEVUI_
                    if (Player.PlayerViewMode == ApplicationUser.enuPlayerViewMode.FirstPerson)
                        Scene.DrawGuis();
#endif

#endregion

#endregion

                    Context.SwapBuffers();
                   
                    CheckGlError("Render Loop Finished");

                    Scene.resetUpdateState();


                    this.checkDeviceLimitsOnce();
                }
                catch
                {
                }
            }
            #endregion Playing | Game.Menu

            #region Editor
            else if(VAR_AppState == ApplicationState.Editor)
            {
                try
                {
                    #region ffmpeg
                    foreach (var video in VideoSources)
                    {
                        if (video.Key.State != FFMpeg.PlayState.Playing)
                            video.Key.Pause();
                    }
                    #endregion

                    #region Frame Statistics Reset
                    UISceneReady(true);
                    double framerate = 1 / e.Time;
                    VAR_Framerate_Smoothness = framerate * (1 - VAR_framerate_smoothness) + VAR_Framerate_Smoothness * VAR_framerate_smoothness;
                    Scene.DrawCallTotal = 0;
                    #endregion Frame Statistics Reset

                    #region Context Synchroniazation
                    if (!Context.IsCurrent)
                    {
                        Context.MakeCurrent(this.WindowInfo);
                        Context.LoadAll();
                    }
                    else
                    {
                        if (ApplicationBase.Instance.VAR_ScreenSize_Virtual.X < ApplicationBase.Instance.VAR_ScreenSize_Current.X ||
                            ApplicationBase.Instance.VAR_ScreenSize_Virtual.Y < ApplicationBase.Instance.VAR_ScreenSize_Current.Y)
                        {
                            ApplicationBase.Instance.VAR_ScreenSize_Virtual = ApplicationBase.Instance.VAR_ScreenSize_Current;
                            Context.Update(this.WindowInfo);
                        }
                    }
                    #endregion Context Synchronization


Renderer.CullFace(CullFaceMode.Back);
Scene.Editor.DrawBuffers(FBO_Scene, Player.ViewInfo);
Renderer.CullFace(CullFaceMode.Back);

                    Context.SwapBuffers();

                    CheckGlError("Render Loop Finished");

                    Scene.resetUpdateState();
                }
                catch
                {
                }
            }
            #endregion Editor

            #region Loading Screen (default)

            else
            {

                try
                {
                    //if (!this.Context.IsCurrent)
                    //    try
                    //    {
                    //        this.Context.MakeCurrent(this.WindowInfo);
                    //    }
                    //    catch (System.Exception cE)
                    //    {
                    //        return;
                    //    }

                    UISceneReady(false);

                    MakeCurrent();

                    Renderer.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    Renderer.CullFaceEnabled = false;
                    Renderer.Viewport(0, 0, Width, Height);

                    Renderer.MatrixMode(MatrixMode.Projection);
                    Renderer.LoadIdentity();
                    Renderer.Frustum(-1f, 1f, -1f, 1f, 0.1f, 1f);

                    if (VAR_ShowSplash)
                        Filter2D_Splash.draw(ShaderLoader.GetShaderByName("splash_shader.fs"), new int[]
                            {
                                TextureLoader.getTextureId("base\\engine_back.png"),
                                TextureLoader.getTextureId("base\\engine_back_h.png")
                            }, Uniform.in_vector,
                                    Vector2.One * VAR_SceneLoadPercentage);

                    GuiDraw(e.Time);

                    SwapBuffers();

                    //this.Context.SwapBuffers();
                }
                catch { }

            }

            #endregion Loading Screen
        }

        public virtual void GuiDraw(double time) { }

        public virtual void UISceneReady(bool ready) { }



#endregion
    }
}
