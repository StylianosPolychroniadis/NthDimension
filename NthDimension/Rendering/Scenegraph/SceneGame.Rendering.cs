/// <summary>
/// NthDimension Deferred Rendering Implementation
/// Rev. 2.0.0
/// </summary>

// Features
#define VOXELS
#define MEASUREPERFORMANCE
// Debug
#if DEBUG
#define MEASUREPERFORMANCE
//#define FBOSHADOW_DEBUG
//#define FBOSHADOW_DEBUG1
#endif


namespace NthDimension.Rendering.Scenegraph
{
    using NthDimension.Algebra;
    using NthDimension.Rasterizer;
    using NthDimension.Rendering.Configuration;
    using NthDimension.Rendering.Culling;
    using NthDimension.Rendering.Drawables;
    using NthDimension.Rendering.Drawables.Framebuffers;
    using NthDimension.Rendering.Drawables.Lights;
    using NthDimension.Rendering.GameViews;
    using NthDimension.Rendering.Shaders;
    using System;

    public partial class SceneGame  // Could be extracted to RenderDriver.cs as separate class 
    {
        #region Draw Calls
        private bool RenderSceneFrame(enuGameRenderPass currentPass, ViewInfo curView)
        {
            #region Render State Setup
            ApplicationBase.Instance.Renderer.DepthTestEnabled = true;
            ApplicationBase.Instance.Renderer.CullFaceEnabled = true;

#if OPENTK3
            Game.Instance.Renderer.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
#else
            ApplicationBase.Instance.Renderer.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
#endif
            #endregion

            // 1. Diffuse Pass
            this.drawCallDiffuse(currentPass, curView);
            // 2. Transparency Pass
            this.drawCallTransparent(currentPass, curView);
            // 3. Shadow Pass                                           
            this.drawCallShadow(currentPass, curView);
            // 4. Selection Pass
            if (this.drawCallSelection(currentPass, curView)) return true; // Brake early -OK
            // 5. Normals Pass                                          
            this.drawCallAlbedo(currentPass, curView);
            // 6. Deferred Pass
            this.drawCallDeferredInfo(currentPass, curView);

            #region Render State Restore
            ApplicationBase.Instance.Renderer.DepthMask(true);
            ApplicationBase.Instance.Renderer.DepthTestEnabled = false;
            ApplicationBase.Instance.Renderer.CullFaceEnabled = false;

#if OPENTK3
            Game.Instance.Renderer.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
#else
            ApplicationBase.Instance.Renderer.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
#endif
            #endregion

            return false;

            #region FBO SHADOW (PSSM) Debug
#if FBOSHADOW_DEBUG
            //ApplicationBase.Instance.Renderer.Disable(EnableCap.Blend);
            ApplicationBase.Instance.Renderer.BlendEnabled = false;


            SunFrameBuffer.CopyToFramebuffer(Framebuffer.enuDebugDrawTexture.DepthTexture, ApplicationBase.Instance.FBO_Scene.SceneFramebuffer.FboHandle, 150 + 50, ApplicationBase.Instance.Height - 50,
                                                                                              150 + 250, ApplicationBase.Instance.Height - 250);

            //SunFrameBuffer[0].DebugDraw(Framebuffer.enuDebugDrawTexture.DepthTexture, 150 + 50, Game.Instance.Height - 50,
            //                                                                                    150 + 250, Game.Instance.Height - 250);


            //SunFrameBuffer[1].DebugDraw(Framebuffer.enuDebugDrawTexture.DepthTexture, 150 + 50, Game.Instance.Height - 260,
            //                                                                                    150 + 250, Game.Instance.Height - 460);


            //SunFrameBuffer[2].DebugDraw(Framebuffer.enuDebugDrawTexture.DepthTexture, 150 + 50, Game.Instance.Height - 470,
            //                                                                                    150 + 250, Game.Instance.Height - 670);


            //SunFrameBuffer[3].DebugDraw(Framebuffer.enuDebugDrawTexture.DepthTexture, 150 + 50, Game.Instance.Height - 680,
            //                                                                                    150 + 250, Game.Instance.Height - 880);


            //Game.Instance.FBO_SHADOW[0].DebugDraw(Framebuffer.enuDebugDrawTexture.ColorTexture, 150 + 50, Game.Instance.Height - 50,
            //                                                                                    150 + 250, Game.Instance.Height - 250);
            //Game.Instance.FBO_SHADOW[0].DebugDraw(Framebuffer.enuDebugDrawTexture.DepthTexture, 150 + 260, Game.Instance.Height - 50,
            //                                                                                    150 + 460, Game.Instance.Height - 250);

            //Game.Instance.FBO_SHADOW[1].DebugDraw(Framebuffer.enuDebugDrawTexture.ColorTexture, 150 + 50, Game.Instance.Height - 260,
            //                                                                                    150 + 250, Game.Instance.Height - 460);
            //Game.Instance.FBO_SHADOW[1].DebugDraw(Framebuffer.enuDebugDrawTexture.DepthTexture, 150 + 260, Game.Instance.Height - 260,
            //                                                                                    150 + 460, Game.Instance.Height - 460);

            //Game.Instance.FBO_SHADOW[2].DebugDraw(Framebuffer.enuDebugDrawTexture.ColorTexture, 150 + 50, Game.Instance.Height - 470,
            //                                                                                    150 + 250, Game.Instance.Height - 670);
            //Game.Instance.FBO_SHADOW[2].DebugDraw(Framebuffer.enuDebugDrawTexture.DepthTexture, 150 + 260, Game.Instance.Height - 470,
            //                                                                                    150 + 460, Game.Instance.Height - 670);

            //Game.Instance.FBO_SHADOW[3].DebugDraw(Framebuffer.enuDebugDrawTexture.ColorTexture, 150 + 50, Game.Instance.Height - 680,
            //                                                                                    150 + 250, Game.Instance.Height - 880);
            //Game.Instance.FBO_SHADOW[3].DebugDraw(Framebuffer.enuDebugDrawTexture.DepthTexture, 150 + 260, Game.Instance.Height - 680,
            //                                                                                    150 + 460, Game.Instance.Height - 880);
#endif
            #endregion FBO SHADOW (PSSM) Debug
#pragma warning disable CS0162
            return false;
        }
        private void drawCallAlbedo(enuGameRenderPass currentPass, ViewInfo curView)
        {
            if (currentPass == enuGameRenderPass.albedo)
            {
#if MULTIPASS
                //AllOffsets.Clear();
                //Drawables.Sort(CompareByMaterial);
#endif

                ApplicationBase.Instance.Renderer.BlendEnabled = false;

                foreach (Drawable curDrawable in Drawables)
                {
                    if (curDrawable.IsVisible)
                    {
#if MEASUREPERFORMANCE
                        System.Diagnostics.Stopwatch drawTime = new System.Diagnostics.Stopwatch();
                        drawTime.Start();
#endif
                        curDrawable.drawNormal(curView);

#if MEASUREPERFORMANCE
                        long time = (drawTime.ElapsedTicks);
                        curDrawable.Performance.SetNormalPassTime(time); // μSec
#endif
                    }
                }

#if MULTIPASS
                //GCHandle handle = GCHandle.Alloc(AllOffsets.ToArray(), GCHandleType.Pinned);
                //IntPtr pointer = handle.AddrOfPinnedObject();
                //ApplicationBase.Instance.Renderer.MultiDrawElements(PrimitiveType.Triangles, AllIndicesLengths.Values.ToArray<int>(), DrawElementsType.UnsignedInt, pointer, AllOffsets.Count);
                //if (handle.IsAllocated)
                //    handle.Free();
#endif
            }
        }
        private void drawCallDiffuse(enuGameRenderPass currentPass, ViewInfo curView)
        {
            if (currentPass == enuGameRenderPass.diffuse)
            {
#if MULTIPASS
                //AllOffsets.Clear();
                //Drawables.Sort(CompareByMaterial);
#else // !MULTIPASS

                ApplicationBase.Instance.Renderer.BlendEnabled = false;
                foreach (Drawable curDrawable in Drawables)
                {
                    if (curDrawable.IsVisible)
                        if ((curDrawable.Renderlayer == RenderLayer.Solid) ||
                            (curDrawable.Renderlayer == RenderLayer.Both))
                        {
#if MEASUREPERFORMANCE
                            System.Diagnostics.Stopwatch drawTime = new System.Diagnostics.Stopwatch();
                            drawTime.Start();
#endif // MEASUREPERFORMANCE
                            curDrawable.draw(curView, false);
#if MEASUREPERFORMANCE
                            long time = (drawTime.ElapsedTicks);
                            curDrawable.Performance.SetDiffusePassTime(time); // μSec
#endif  // MEASUREPERFORMANCE
                        }
#endif // MULTIPASS
                }

#if MULTIPASS
                //GCHandle handle = GCHandle.Alloc(AllOffsets.ToArray(), GCHandleType.Pinned);
                //IntPtr pointer = handle.AddrOfPinnedObject();
                //ApplicationBase.Instance.Renderer.MultiDrawElements(PrimitiveType.Triangles, AllIndicesLengths.Values.ToArray<int>(), DrawElementsType.UnsignedInt, pointer, AllIndicesLengths.Count);
                //if (handle.IsAllocated)
                //    handle.Free();
#endif


            }
        }
        private void drawCallTransparent(enuGameRenderPass currentPass, ViewInfo curView)
        {
            if (currentPass == enuGameRenderPass.transparent)
            {
#if MULTIPASS
                //AllOffsets.Clear();
#endif
                ApplicationBase.Instance.Renderer.BlendEnabled = true;

                /*
               for (int i = Drawables.Count - 1; i >= 0; i--)
                   if ((Drawables[i].Renderlayer == RenderLayer.Transparent) ||
                       (Drawables[i].Renderlayer == RenderLayer.Both))
                           Drawables[i].draw(curView, true);

               * */
                //Drawables.Sort(CompareByMaterial);
                foreach (Drawable curDrawable in Drawables)
                {
                    

                    if (curDrawable.IsVisible)
                        if ((curDrawable.Renderlayer == RenderLayer.Transparent) ||
                            (curDrawable.Renderlayer == RenderLayer.Both))
                        {
#if MEASUREPERFORMANCE
                            System.Diagnostics.Stopwatch drawTime = new System.Diagnostics.Stopwatch();
                            drawTime.Start();
#endif
                            curDrawable.draw(curView, true);

#if MEASUREPERFORMANCE
                            long time = (drawTime.ElapsedTicks);
                            curDrawable.Performance.SetTransparentPassTime(time); // μSec
#endif
                            //Utilities.ConsoleUtil.log(string.Format("TransparentPass -> GameObject: {0} Drawable: {1}", curDrawable.Parent.Name, curDrawable.Name));
                        }
                }

#if MULTIPASS
                //GCHandle handle = GCHandle.Alloc(AllOffsets.ToArray(), GCHandleType.Pinned);
                //IntPtr pointer = handle.AddrOfPinnedObject();
                //ApplicationBase.Instance.Renderer.MultiDrawElements(PrimitiveType.Triangles, AllIndicesLengths.Values.ToArray<int>(), DrawElementsType.UnsignedInt, pointer, AllOffsets.Count);
                //if (handle.IsAllocated)
                //    handle.Free();
#endif

                ApplicationBase.Instance.Renderer.BlendEnabled = false;

            }
        }
        private void drawCallShadow(enuGameRenderPass currentPass, ViewInfo curView)
        {
            if (currentPass == enuGameRenderPass.shadow)
            {
#if MULTIPASS
                //AllOffsets.Clear();
                //Drawables.Sort(CompareByMaterial);
#endif
                foreach (Drawable curDrawable in Drawables)
                {
                    if (curDrawable.IsVisible && curDrawable.CastShadows)
                    {
#if MEASUREPERFORMANCE
                        System.Diagnostics.Stopwatch drawTime = new System.Diagnostics.Stopwatch();
                        drawTime.Start();
#endif

                        curDrawable.drawShadow(curView);

#if MEASUREPERFORMANCE
                        long time = (drawTime.ElapsedTicks);
                        curDrawable.Performance.SetShadowPassTime(time); // μSec
#endif
                    }
                }
#if MULTIPASS
                //GCHandle handle = GCHandle.Alloc(AllOffsets.ToArray(), GCHandleType.Pinned);
                //IntPtr pointer = handle.AddrOfPinnedObject();
                //ApplicationBase.Instance.Renderer.MultiDrawElements(PrimitiveType.Triangles, AllIndicesLengths.Values.ToArray<int>(), DrawElementsType.UnsignedInt, pointer, AllOffsets.Count);
                //if (handle.IsAllocated)
                //    handle.Free();
#endif
            }
        }
        private bool drawCallSelection(enuGameRenderPass currentPass, ViewInfo curView)
        {
            if (currentPass == enuGameRenderPass.selection)
            {
#if MULTIPASS
                //AllOffsets.Clear();
                //Drawables.Sort(CompareByMaterial);
#endif

                bool hasSelection = false;

                foreach (Drawable curDrawable in Drawables)
                {
                    if (curDrawable.IsVisible)
                        if (curDrawable.SelectedSmooth > 0.01)
                        {
#if MEASUREPERFORMANCE
                            System.Diagnostics.Stopwatch drawTime = new System.Diagnostics.Stopwatch();
                            drawTime.Start();
#endif

                            curDrawable.drawSelection(curView);
                            hasSelection = true;

#if MEASUREPERFORMANCE
                            long time = (drawTime.ElapsedTicks);
                            curDrawable.Performance.SetSelectionPassTime(time); // μSec
#endif
                        }
                }

#if MULTIPASS
                //GCHandle handle = GCHandle.Alloc(AllOffsets.ToArray(), GCHandleType.Pinned);
                //IntPtr pointer = handle.AddrOfPinnedObject();
                //ApplicationBase.Instance.Renderer.MultiDrawElements(PrimitiveType.Triangles, AllIndicesLengths.Values.ToArray<int>(), DrawElementsType.UnsignedInt, pointer, AllOffsets.Count);
                //if (handle.IsAllocated)
                //    handle.Free();
#endif

                return hasSelection;
            }
            return false;
        }        
        private void drawCallDeferredInfo(enuGameRenderPass currentPass, ViewInfo curView)
        {
            if (currentPass == enuGameRenderPass.defInfo)
            {
#if MULTIPASS
                //AllOffsets.Clear();
                //Drawables.Sort(CompareByMaterial);
#endif

                ApplicationBase.Instance.Renderer.BlendEnabled = false;

                foreach (Drawable curDrawable in Drawables)
                {
                    if (curDrawable.IsVisible)
                    {
#if MEASUREPERFORMANCE
                        System.Diagnostics.Stopwatch drawTime = new System.Diagnostics.Stopwatch();
                        drawTime.Start();
#endif
                        curDrawable.drawDefInfo(curView);
#if MEASUREPERFORMANCE

                        long time = (drawTime.ElapsedTicks);
                        curDrawable.Performance.SetDeferredPassTime(time); // μSec
#endif
                    }
                }
#if MULTIPASS
                //GCHandle handle = GCHandle.Alloc(AllOffsets.ToArray(), GCHandleType.Pinned);
                //IntPtr pointer = handle.AddrOfPinnedObject();
                //ApplicationBase.Instance.Renderer.MultiDrawElements(PrimitiveType.Triangles, AllIndicesLengths.Values.ToArray<int>(), DrawElementsType.UnsignedInt, pointer, AllOffsets.Count);
                //if (handle.IsAllocated)
                //    handle.Free();
#endif
            }
        }
        #endregion

        internal void DrawBuffers(FramebufferSet curRenderComposite, ViewInfo curView)
        {
            /// Revision 3.0 NthDimension Real-Time Deferred Renderer
#if _DEVUI_
            return;
#endif
            RenderOptions renderOptions = curRenderComposite.renderOptions;

            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Entering Scene Draw {0}:{1}", this.GetType(), Name));

            #region Albedo Pass
            #region Render State
            ApplicationBase.Instance.Renderer.BlendEnabled = true;
            curRenderComposite.SceneFramebuffer.enable(true);
            curRenderComposite.SceneFramebuffer.Multisampling = false;

            ApplicationBase.Instance.Renderer.BlendEnabled = true;
            #endregion Render State

            RenderSceneFrame(enuGameRenderPass.albedo, curView);
            #endregion

            #region SSAO Pass
            #region Render State
            ApplicationBase.Instance.Renderer.BlendEnabled = false;
            #endregion Render State


            if (renderOptions.ssAmbientOcclusion)
            {
                curRenderComposite.AoPreFramebuffer.enable(false);
                Filter2d_RenderSurface.draw(enuShaderFilters.ssaoPreShader,
                                        new int[]
                                        {
                                            curRenderComposite.SceneFramebuffer.ColorTexture,
                                            curRenderComposite.SceneFramebuffer.DepthTexture
                                        },
                                        Uniform.modelview_matrix,   // ToDo:: define and pass a uniform struct here containing modelviewmatrix + ao configuration (view settings)
                                        curView.modelviewMatrix);

                curRenderComposite.AoFramebuffer.enable(false);
                Filter2d_RenderSurface.draw(enuShaderFilters.ssaoShader,
                                        new int[]
                                        {
                                            curRenderComposite.AoPreFramebuffer.ColorTexture,
                                            GetTextureId(Rendering.Material.WorldTexture.noise)
                                        });

                curRenderComposite.AoBlurFramebuffer.enable(false);
                Filter2d_RenderSurface.draw(enuShaderFilters.ssaoBlrShaderA, new int[] {
                                                                            curRenderComposite.AoFramebuffer.ColorTexture,
                                                                            curRenderComposite.AoBlurFramebuffer2.ColorTexture
                                                                       });

                curRenderComposite.AoBlurFramebuffer2.enable(false);
                Filter2d_RenderSurface.draw(enuShaderFilters.ssaoBlrShader, new int[] {
                                                                            curRenderComposite.AoBlurFramebuffer.ColorTexture
                                                                      });
            }
            #endregion

            #region Reflection Pass
            //render defferd reflections
            curRenderComposite.ReflectionFramebuffer.enable(true);
            Filter2d_RenderSurface.draw(enuShaderFilters.reflectionShader, new int[] {
                Scene.EnvTextures[0],
                Scene.EnvTextures[1],
                Scene.EnvTextures[2],
                Scene.EnvTextures[3],
                Scene.EnvTextures[4],
                Scene.EnvTextures[5],
                curRenderComposite.SceneFramebuffer.ColorTexture,
            },
                Uniform.invMVPMatrix,
                curView.invModelviewProjectionMatrix);

            SetTextureId(Rendering.Material.WorldTexture.reflectionMap, curRenderComposite.ReflectionFramebuffer.ColorTexture);
            #endregion

            #region Deferred SunLight Pass

            #region Render State
            //render defferedLight
            curRenderComposite.LightFramebuffer.enable(true);

            ApplicationBase.Instance.Renderer.BlendEnabled = true;
#if OPENTK3
            Game.Instance.Renderer.BlendFunc(BlendingFactor.One, BlendingFactor.One);
#else
            ApplicationBase.Instance.Renderer.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
#endif
            #endregion Render State

            foreach (LightDirectional SunLight in DirectionalLights)
                if (SunLight.Enabled)
                {
#if FBOSHADOW_DEBUG
                    if (Settings.Instance.video.pssmShadows)
                    {

                        SunLight.drawable.draw(new int[]
                        {
                                curFramebuffers.SceneFramebuffer.ColorTexture,

                                SunFrameBufferPssm[0].DepthTexture,
                                SunInnerFrameBufferPssm[0].DepthTexture,

                                SunFrameBufferPssm[1].DepthTexture,
                                SunInnerFrameBufferPssm[1].DepthTexture,

                                SunFrameBufferPssm[2].DepthTexture,
                                SunInnerFrameBufferPssm[2].DepthTexture,

                                SunFrameBufferPssm[3].DepthTexture,
                                SunInnerFrameBufferPssm[3].DepthTexture,
                   
                                // TODO:: Pass textures for PSSM

                                GetTextureId(Rendering.Material.WorldTexture.noise),
                                curFramebuffers.SceneFramebuffer.DepthTexture
                        }, ref curView);
                    }
                    else
                    {
#endif
                        SunLight.drawable.draw(new int[]
                        {
                            curRenderComposite.SceneFramebuffer.ColorTexture,              // 0

                            // Todo:: Enable in Shader
                            //SunFrameBufferFar3.DepthTexture,
                            //SunFrameBufferFar2.DepthTexture,
                            SunFrameBufferFar1.DepthTexture,                           // 1 (3)
                            SunFrameBufferNear.DepthTexture,                           // 2 (4)

                            GetTextureId(Rendering.Material.WorldTexture.noise),        // 3 (5)

                            curRenderComposite.SceneFramebuffer.DepthTexture               // 4 (6)
                        }, ref curView);
#if FBOSHADOW_DEBUG
                    }
#endif
                }
            #endregion

            #region Deferred Spotlights Pass            
            ApplicationBase.Instance.Renderer.CullFaceEnabled = true;
            foreach (var light in Spotlights)
            {
#if FBOSHADOW_DEBUG
                if (Settings.Instance.video.pssmShadows)
                {
                    for (int i = 0; i < ApplicationBase.Instance.FBO_ShadowPssm.Length; i++)
                        light.drawable.draw(new int[]
                        {
                            curFramebuffers.SceneFramebuffer.ColorTexture,
                            ApplicationBase.Instance.FBO_ShadowPssm[i].ColorTexture,
                            0,
                            GetTextureId(Rendering.Material.WorldTexture.noise),
                            curFramebuffers.SceneFramebuffer.DepthTexture
                        }, ref curView);
                }
                else
                {
#endif
                light.drawable.draw(new int[]
                {
                    curRenderComposite.SceneFramebuffer.ColorTexture,
                    ApplicationBase.Instance.FBO_Shadow.ColorTexture,
                    0,
                    GetTextureId(Rendering.Material.WorldTexture.noise),
                    curRenderComposite.SceneFramebuffer.DepthTexture
                }, ref curView);
#if FBOSHADOW_DEBUG
            }
#endif

        }
            #endregion

            #region Deferred Pointlights Pass
            ApplicationBase.Instance.Renderer.CullFaceEnabled = true;
            foreach (var light in PointLights)
            {
#if FBOSHADOW_DEBUG
                if (Settings.Instance.video.pssmShadows)
                {
                    for (int i = 0; i < ApplicationBase.Instance.FBO_ShadowPssm.Length; i++)
                        light.drawable.draw(new int[]
                        {
                            curFramebuffers.SceneFramebuffer.ColorTexture,
                            ApplicationBase.Instance.FBO_ShadowPssm[i].ColorTexture,
                            0,
                            GetTextureId(Rendering.Material.WorldTexture.noise),
                            curFramebuffers.SceneFramebuffer.DepthTexture
                        }, ref curView);
                }
                else
                {
#endif
                light.drawable.draw(new int[]
                {
                    curRenderComposite.SceneFramebuffer.ColorTexture,
                    ApplicationBase.Instance.FBO_Shadow.ColorTexture,
                    0,
                    GetTextureId(Rendering.Material.WorldTexture.noise),
                    curRenderComposite.SceneFramebuffer.DepthTexture
                }, ref curView);
#if FBOSHADOW_DEBUG
            }
#endif

            }
            #endregion

            #region Lightmap Smoothing Pass
            ApplicationBase.Instance.Renderer.BlendEnabled = false;
            ApplicationBase.Instance.Renderer.CullFaceEnabled = false;

            if (Settings.Instance.video.lightmapSmoothing)
            {
                SetTextureId(Rendering.Material.WorldTexture.lightMap, 
                             curRenderComposite.LightBlurFramebuffer.ColorTexture);

                curRenderComposite.LightBlurFramebuffer.enable(false);
                Filter2d_RenderSurface.draw(enuShaderFilters.lightBlurShader, new int[]
                                                                    {
                                                                        curRenderComposite.SceneFramebuffer.ColorTexture,
                                                                        curRenderComposite.LightFramebuffer.ColorTexture
                                                                    });
            }
            else
                SetTextureId(Rendering.Material.WorldTexture.lightMap, curRenderComposite.LightFramebuffer.ColorTexture);


            curRenderComposite.SceneFramebuffer.enable(false);
            RenderSceneFrame(enuGameRenderPass.diffuse, curView);
            #endregion

            #region Transparent Pass
            ApplicationBase.Instance.Renderer.BlendEnabled = true;
            // copy scene to transparent fb -- we can do lookups
            curRenderComposite.SceneBackdropFb.enable(true);
            Filter2d_RenderSurface.draw(enuShaderFilters.copycatShader, 
                                new int[] 
                                {
                                    curRenderComposite.SceneFramebuffer.ColorTexture
                                });

            // switch back to scene fb
            curRenderComposite.SceneFramebuffer.enable(false);

            BackdropTextures = new int[] {
                curRenderComposite.SceneBackdropFb.ColorTexture,
                curRenderComposite.SceneBackdropFb.DepthTexture };

            if (renderOptions.ssAmbientOcclusion)
                Filter2d_RenderSurface.draw(enuShaderFilters.ssaoBlendShader, 
                                        new int[] {
                                            curRenderComposite.AoBlurFramebuffer2.ColorTexture,
                                            curRenderComposite.SceneBackdropFb.ColorTexture });

            RenderSceneFrame(enuGameRenderPass.transparent, curView);
            #endregion

            #region Selection Pass
            curRenderComposite.SelectionFb.enable(true);

            bool hasSelection = RenderSceneFrame(enuGameRenderPass.selection, curView);

            //ApplicationBase.Instance.Renderer.Disable(EnableCap.Blend);
            //ApplicationBase.Instance.Renderer.Disable(EnableCap.DepthTest);
            //ApplicationBase.Instance.Renderer.Disable(EnableCap.CullFace);
            ApplicationBase.Instance.Renderer.BlendEnabled = false;
            ApplicationBase.Instance.Renderer.DepthTestEnabled = false;
            ApplicationBase.Instance.Renderer.CullFaceEnabled = false;

            if (hasSelection)
            {
                curRenderComposite.SelectionblurFb.enable(false);
                Filter2d_RenderSurface.draw(enuShaderFilters.bloomShader, new int[] { curRenderComposite.SelectionFb.ColorTexture }, Uniform.in_vector, b_bloomSize);
            }

            curRenderComposite.SelectionblurFb2.enable(true);

            if (hasSelection)
                Filter2d_RenderSurface.draw(enuShaderFilters.bloomShader, new int[] { curRenderComposite.SelectionblurFb.ColorTexture }, Uniform.in_vector, b_bloomSize);
            #endregion

            #region Bloom Pass (Note: BloomExposure and BloomStrength hard-coded in bloom_curve.fs)

            // TODO:: NOTE:: bloomExp and bloonStrength are hard coded in bloom_curve.fs! Need to pass as uniforms

            if (renderOptions.bloom && b_bloomEnabled)
            {
                curRenderComposite.BloomFramebuffer2.Multisampling = false;
                curRenderComposite.BloomFramebuffer2.enable(false);

                Filter2d_RenderSurface.draw(enuShaderFilters.bloomCurveShader,
                                        new int[] {
                                                    curRenderComposite.SceneFramebuffer.ColorTexture
                                                  }
                                        );

                for (int i = 0; i < 2; i++)
                {
                    curRenderComposite.BloomFramebuffer.enable(false);
                    Filter2d_RenderSurface.draw(enuShaderFilters.bloomShader,
                                            new int[] {
                                                        curRenderComposite.BloomFramebuffer2.ColorTexture
                                                      },
                                            Uniform.in_vector,
                                            b_bloomSize);

                    curRenderComposite.BloomFramebuffer2.enable(false);
                    Filter2d_RenderSurface.draw(enuShaderFilters.bloomShader,
                                            new int[] {
                                                        curRenderComposite.BloomFramebuffer.ColorTexture
                                                      },
                                            Uniform.in_vector,
                                            b_bloomSize);
                }
                curRenderComposite.BloomFramebuffer2.Multisampling = true;
            }
            #endregion

            #region Depth of Field Pass
            if (renderOptions.depthOfField /* && DoF_Motion*/)
            {
                //Vector2 invec = new Vector2(curView.GetFocus(0.9f), 0.1f); ////new Vector2(curView.GetFocus(0.9f), 0.01f)
                curRenderComposite.DofPreFramebuffer.enable(false);
                Filter2d_RenderSurface.draw(enuShaderFilters.dofpreShader, new int[] {
                    curRenderComposite.ScreenSpaceNormalFb.ColorTexture,
                    curRenderComposite.SceneBackdropFb.ColorTexture
                }, Uniform.in_vector, new Vector2(curView.GetFocus(0.9f), 0.1f));
                //invec); //new Vector2(curView.GetFocus(0.9f), 0.01f)

                curRenderComposite.DofFramebuffer.enable(false);
                Filter2d_RenderSurface.draw(enuShaderFilters.dofShader, new int[] { curRenderComposite.DofPreFramebuffer.ColorTexture, GetTextureId(Rendering.Material.WorldTexture.noise) });

                curRenderComposite.DofFramebuffer2.enable(false);
                Filter2d_RenderSurface.draw(enuShaderFilters.dofShader, new int[] { curRenderComposite.DofFramebuffer.ColorTexture, GetTextureId(Rendering.Material.WorldTexture.noise) });

                //Utilities.ConsoleUtil.log(string.Format("DoF {0}", invec.ToString()), false);
            }
            #endregion

            #region Write Output (Composite) Pass
            curRenderComposite.OutputFb.enable(false);
            curRenderComposite.SceneFramebuffer.Multisampling = true;

            int texture = curRenderComposite.SceneFramebuffer.ColorTexture;

            if (curRenderComposite.AoPreFramebuffer != null)
                texture = curRenderComposite.LightBlurFramebuffer.ColorTexture;

            if (b_bloomEnabled)
            {
                Filter2d_RenderSurface.draw(enuShaderFilters.composite,
                                        new int[] {
                                                    curRenderComposite.SceneFramebuffer.ColorTexture,
                                                    curRenderComposite.BloomFramebuffer2.ColorTexture,
                                                    curRenderComposite.SelectionFb.ColorTexture,
                                                    curRenderComposite.SelectionblurFb2.ColorTexture,
                                                    curRenderComposite.DofFramebuffer2.ColorTexture,
                                                    texture
                                                   },
                                        Uniform.in_vector,
                                        compositeMod);
            }
            else
            {
                Filter2d_RenderSurface.draw(enuShaderFilters.composite,
                        new int[] {
                                                    curRenderComposite.SceneFramebuffer.ColorTexture,
                                                    //curFramebuffers.BloomFramebuffer2.ColorTexture,
                                                    curRenderComposite.SelectionFb.ColorTexture,
                                                    curRenderComposite.SelectionblurFb2.ColorTexture,
                                                    curRenderComposite.DofFramebuffer2.ColorTexture,
                                                    texture
                                   },
                        Uniform.in_vector,
                        compositeMod);
            }
            #endregion

            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Leaving Scene Update {0}:{1}", this.GetType(), Name));
        }
        internal void DrawShadowBuffer_Simple(Framebuffer shadowFb)
        {
            shadowFb.enable(false);

            for (int i = 0; i < Spotlights.Count; i++)
            {
                Spotlights[i].lightId = i;

                bool needsUpdate = Spotlights[i].viewInfo.checkForUpdates(Drawables);
                CurrentLight = i;

                if (needsUpdate)
                //if(true)
                //if(Spotlights[i].IsVisible)
                {
                    //Utilities.ConsoleUtil.log(string.Format(
                    //    "{0} - Updating Shadowbuffer SpotLight-Id: {1}, No of Spotlights: {2}",
                    //    DateTime.Now.ToString("HH:mm:ss.fff"), i, Spotlights.Count), false);

                    ApplicationBase.Instance.Renderer.BlendEnabled = false;
                    ApplicationBase.Instance.Renderer.DepthTestEnabled = true;
                    ApplicationBase.Instance.Renderer.CullFaceEnabled = false;

                    ApplicationBase.Instance.Renderer.DepthFunc(DepthFunction.Always);
                    Filter2d_RenderSurface.draw(enuShaderFilters.wipingShader,
                        new int[]
                        {
                            Spotlights[i].ProjectionTexture
                        },
                        Uniform.in_vector,
                        new Vector2(i, LightCount));

                    ApplicationBase.Instance.Renderer.DepthFunc(DepthFunction.Less);

                    ApplicationBase.Instance.Renderer.ColorMask(false, false, false, true);
                    RenderSceneFrame(enuGameRenderPass.shadow, Spotlights[i].viewInfo);
                    ApplicationBase.Instance.Renderer.ColorMask(true, true, true, true);
                }
            }

            ApplicationBase.Instance.Renderer.BlendEnabled = false;
            ApplicationBase.Instance.Renderer.DepthTestEnabled = true;
            ApplicationBase.Instance.Renderer.CullFaceEnabled = false;

            int tmpLightCount = LightCount;
            LightCount = 1;
            CurrentLight = 0;

            //if (SunLight0.viewInfo.wasUpdated)
            // Missing depth clamp???
            foreach (LightDirectional SunLight in DirectionalLights)
            {
#if PSSM
                if (Settings.Instance.video.pssmShadows)
                {
                    //for (int i = 0; i < SunFrameBufferPssm.Length; i++)
                    //{
                    //    SunFrameBufferPssm[i].enable(true);
                    //    ApplicationBase.Instance.Renderer.ColorMask(false, false, false, false);
                    //    if (SunLight.Enabled)
                    //        drawSceneFrame(enuRenderPass.shadow, SunLight.viewInfo);     // Key 
                    //}
                    //ApplicationBase.Instance.Renderer.ColorMask(true, true, true, true);

                    //for (int i = 0; i < SunInnerFrameBufferPssm.Length; i++)
                    //{
                    //    SunInnerFrameBufferPssm[i].enable(true);
                    //    ApplicationBase.Instance.Renderer.ColorMask(false, false, false, false);
                    //    if (SunLight.Enabled)
                    //        drawSceneFrame(enuRenderPass.shadow, SunLight.innerViewInfo);    // Key
                    //}
                    //ApplicationBase.Instance.Renderer.ColorMask(true, true, true, true);
                    
                }
                else
#endif
                {
                    // Wide range shadows                  
                    if (SunLight.viewInfo.wasUpdated && SunLight.Enabled)
                    {
                        SunFrameBufferFar1.enable(true);
                        ApplicationBase.Instance.Renderer.ColorMask(false, false, false, false);
                        RenderSceneFrame(enuGameRenderPass.shadow, SunLight.viewInfo);
                        ApplicationBase.Instance.Renderer.ColorMask(true, true, true, true);

                        // TODO:: Enable in Shader
                        //SunFrameBufferFar2.enable(true);
                        //ApplicationBase.Instance.Renderer.ColorMask(false, false, false, false);
                        //drawSceneFrame(enuRenderPass.shadow, SunLight.viewInfo);
                        //ApplicationBase.Instance.Renderer.ColorMask(true, true, true, true);
                        // TODO:: Enable in Shader
                        //SunFrameBufferFar3.enable(true);
                        //ApplicationBase.Instance.Renderer.ColorMask(false, false, false, false);
                        //drawSceneFrame(enuRenderPass.shadow, SunLight.viewInfo);
                        //ApplicationBase.Instance.Renderer.ColorMask(true, true, true, true);

                    }

                    // Short range shadows
                    if (SunLight.Enabled)
                    {
                        SunFrameBufferNear.enable(true);
                        ApplicationBase.Instance.Renderer.ColorMask(false, false, false, false);
                        RenderSceneFrame(enuGameRenderPass.shadow, SunLight.shortViewInfo);    // Key
                        ApplicationBase.Instance.Renderer.ColorMask(true, true, true, true);
                    }
                }
            }
            LightCount = tmpLightCount;
        }

#if PSSM
        internal void DrawShadowBuffer_PSSM(Framebuffer[] shadowFb)
        {
            //Game.Instance.FBO_Shadow.enable(false);
            //shadowFb.enable(false);

            //drawShadowSplit(0);
            //return;

            BoundingSphere[] obj_BSphere = new BoundingSphere[4];
            int worldDimensions = 3000;// Configuration.Settings.Instance.video.shadowResolution/2; // NOTE:: Shadow Resolution here instead of 256???

            //obj_BSphere[0].Center = new Vector3(-worldDimensions, EyePos.Y, -worldDimensions);    // Y was 50
            //obj_BSphere[1].Center = new Vector3(-worldDimensions, EyePos.Y, worldDimensions);    // Y was 50
            //obj_BSphere[2].Center = new Vector3(worldDimensions, EyePos.Y, worldDimensions);    // Y was 50
            //obj_BSphere[3].Center = new Vector3(worldDimensions, EyePos.Y, -worldDimensions);    // Y was 50

            obj_BSphere[0].Center = new Vector3(-worldDimensions, 50, -worldDimensions);    // Y was 50
            obj_BSphere[1].Center = new Vector3(-worldDimensions, 50, worldDimensions);    // Y was 50
            obj_BSphere[2].Center = new Vector3(worldDimensions, 50, worldDimensions);    // Y was 50
            obj_BSphere[3].Center = new Vector3(worldDimensions, 50, -worldDimensions);    // Y was 50

            //obj_BSphere[0].Center = new Vector3(-EyePos.X, EyePos.Y, -EyePos.Z);    // Y was 50
            //obj_BSphere[1].Center = new Vector3(-EyePos.X, EyePos.Y, EyePos.Z);    // Y was 50
            //obj_BSphere[2].Center = new Vector3(EyePos.X, EyePos.Y, EyePos.Z);    // Y was 50
            //obj_BSphere[3].Center = new Vector3(EyePos.X, EyePos.Y, -EyePos.Z);    // Y was 50

            obj_BSphere[0].Radius = 1.0f;
            obj_BSphere[1].Radius = 1.0f;
            obj_BSphere[2].Radius = 1.0f;
            obj_BSphere[3].Radius = 1.0f;

#region Sun Look at (modelview matrix)



#region Commented from SunViewInfo
            //Matrix4 lightViewMat = Matrix4.Identity;
            //if (Game.Instance.Player.PlayerViewMode == Player.enuPlayerViewMode.FirstPerson)
            //{
            //    Vector3 pos = Game.Instance.Player.Position;
            //    float texelSize = 1f / Settings.Instance.video.shadowResolution;


            //    position = new Vector3((float)Math.Floor(pos.X / texelSize) * texelSize,
            //        (float)Math.Floor(pos.Y / texelSize) * texelSize,
            //        (float)Math.Floor(pos.Z / texelSize) * texelSize);

            //    PointingDirection = new Vector3(0.4817817f, -0.4817817f, 0.731965f);

            //    Vector3 camDir = Game.Instance.Player.PointingDirection;
            //    lightViewMat = Matrix4.LookAt(position, position + (-camDir + PointingDirection).Normalized(), new Vector3(0, 1, 0));
            //}

            //if (Game.Instance.Player.PlayerViewMode == Player.enuPlayerViewMode.ThirdPerson)
            //{
            //    Vector3 pos = Game.Instance.Scene.EyePos;

            //    float texelSize = 1f / Settings.Instance.video.shadowResolution;

            //    position = new Vector3((float)Math.Floor(pos.X / texelSize) * texelSize,
            //        (float)Math.Floor(pos.Y / texelSize) * texelSize,
            //        (float)Math.Floor(pos.Z / texelSize) * texelSize);

            //    pointingDirection = new Vector3(0.4817817f, -0.4817817f, 0.731965f);


            //    Vector3 camDir = Game.Instance.Player.PointingDirection;
            //    Vector3 newDir = (-camDir + PointingDirection).Normalized();
            //    Vector3 upVec = Game.Instance.Player.ViewInfo.pointingDirectionUp.Normalized(); // new Vector3(0, 1, 0); //

            //    lightViewMat = Matrix4.LookAt(Scene.EyePos, Scene.EyePos + SunLight0.PointingDirection, new Vector3(0f, 1f, 0f));
            //}
#endregion

#endregion

            //int depth_size    = 2048;
            int numSplits = 4;//shadowFb.Length;
            float strength = 0.75f;

            //float camNear     = 0.1f;// Game.Instance.Player.ViewInfo.zNear;
            //float camFar      = 120f;// Game.Instance.Player.ViewInfo.zFar;
            //float ratio       = camFar / camNear;// Game.Instance.Player.ViewInfo.zFar / Game.Instance.Player.ViewInfo.zNear;

            float camNear = ApplicationBase.Instance.Player.ViewInfo.zNear;
            float camFar = ApplicationBase.Instance.Player.ViewInfo.zFar;
            float ratio = camFar / camNear;
            float[] step = { -1, 100, 600, 1000 };

            Matrix4[] lightProjMatrix = new Matrix4[numSplits];
            Matrix4[] lightProjMatrixInner = new Matrix4[numSplits];
            Matrix4[] cropMat = new Matrix4[numSplits];
            Matrix4[] cropInnerMat = new Matrix4[numSplits];
            Matrix4[] sliceProjMatrix = new Matrix4[numSplits];
            Matrix4[] sliceInnerProjMatrix = new Matrix4[numSplits];

            float[] camFrustumFarPlane = new float[4];
            float[] camFrustumNearPlane = new float[4];

#region Frustum Planes
            camFrustumFarPlane[0] = camNear;
            for (int split = 1; split < numSplits; split++)
            {
                camFar = step[split];
                lightProjMatrix[split] = Matrix4.Identity;
                lightProjMatrixInner[split] = Matrix4.Identity;

                float si = split / (float)numSplits;
                camFrustumNearPlane[split] = strength * (camNear * (float)Math.Pow(ratio, si)) +
                                                    ((1.0f - strength) * (camNear + (si * (camFar - camNear))));
                camFrustumFarPlane[split - 1] = camFrustumNearPlane[split] * 1.005f;
            }
            camFrustumFarPlane[numSplits - 1] = camFar;
#endregion

            for (int split = 0; split < numSplits; split++)
            {
                float nearDist = camFrustumNearPlane[split];
                float farDist = camFrustumFarPlane[split];

                // these heights and widths are half the heights and widths of
                // the near and far plane rectangles
                float nearHeight = (float)Math.Tan(/*(MathHelper.DegreesToRadians(*/ApplicationBase.Instance.Player.ViewInfo.fovy/*))*/ / 2.0f) * nearDist;
                float nearWidth = nearHeight * ApplicationBase.Instance.Player.ViewInfo.aspect;
                float farHeight = (float)Math.Tan(/*(MathHelper.DegreesToRadians(*/ApplicationBase.Instance.Player.ViewInfo.fovy/*))*/ / 2.0f) * farDist;
                float farWidth = farHeight * ApplicationBase.Instance.Player.ViewInfo.aspect;

                //Vector3 fc = ApplicationBase.Instance.Scene.EyePos + ApplicationBase.Instance.Player.GetFrontVec() * farDist;       // Note GetFrontVec or PointingDirection ?
                //Vector3 nc = ApplicationBase.Instance.Scene.EyePos + ApplicationBase.Instance.Player.GetFrontVec() * nearDist;      // Note GetFrontVec or PointingDirection ?

                Vector3 fc = ApplicationBase.Instance.Scene.EyePos + ApplicationBase.Instance.Player.PointingDirection.Normalized() * farDist;       // Note GetFrontVec or PointingDirection ?
                Vector3 nc = ApplicationBase.Instance.Scene.EyePos + ApplicationBase.Instance.Player.PointingDirection.Normalized() * nearDist;      // Note GetFrontVec or PointingDir


                Vector4[] cornerPoints = new Vector4[8];

                // near plane
                cornerPoints[0] = new Vector4(nc - (ApplicationBase.Instance.Player.VectorUp * nearHeight) - (ApplicationBase.Instance.Player.GetRightVec() * nearWidth), 1.0f);
                cornerPoints[1] = new Vector4(nc + (ApplicationBase.Instance.Player.VectorUp * nearHeight) - (ApplicationBase.Instance.Player.GetRightVec() * nearWidth), 1.0f);
                cornerPoints[2] = new Vector4(nc + (ApplicationBase.Instance.Player.VectorUp * nearHeight) + (ApplicationBase.Instance.Player.GetRightVec() * nearWidth), 1.0f);
                cornerPoints[3] = new Vector4(nc - (ApplicationBase.Instance.Player.VectorUp * nearHeight) + (ApplicationBase.Instance.Player.GetRightVec() * nearWidth), 1.0f);
                // far plane
                cornerPoints[4] = new Vector4(fc - (ApplicationBase.Instance.Player.VectorUp * farHeight) - (ApplicationBase.Instance.Player.GetRightVec() * farWidth), 1.0f);    // top left
                cornerPoints[5] = new Vector4(fc + (ApplicationBase.Instance.Player.VectorUp * farHeight) - (ApplicationBase.Instance.Player.GetRightVec() * farWidth), 1.0f);    // top right
                cornerPoints[6] = new Vector4(fc + (ApplicationBase.Instance.Player.VectorUp * farHeight) + (ApplicationBase.Instance.Player.GetRightVec() * farWidth), 1.0f);    // bottom left
                cornerPoints[7] = new Vector4(fc - (ApplicationBase.Instance.Player.VectorUp * farHeight) + (ApplicationBase.Instance.Player.GetRightVec() * farWidth), 1.0f);    // bottom right


                // near plane
                //cornerPoints[0] = new Vector4(nc - (Game.Instance.Player.GetUpVec() * nearHeight) - (Game.Instance.Player.GetRightVec() * nearWidth), 1.0f);
                //cornerPoints[1] = new Vector4(nc + (Game.Instance.Player.GetUpVec() * nearHeight) - (Game.Instance.Player.GetRightVec() * nearWidth), 1.0f);
                //cornerPoints[2] = new Vector4(nc + (Game.Instance.Player.GetUpVec() * nearHeight) + (Game.Instance.Player.GetRightVec() * nearWidth), 1.0f);
                //cornerPoints[3] = new Vector4(nc - (Game.Instance.Player.GetUpVec() * nearHeight) + (Game.Instance.Player.GetRightVec() * nearWidth), 1.0f);
                //// far plane
                //cornerPoints[4] = new Vector4(fc - (Game.Instance.Player.GetUpVec() * farHeight) - (Game.Instance.Player.GetRightVec() * farWidth), 1.0f);    // top left
                //cornerPoints[5] = new Vector4(fc + (Game.Instance.Player.GetUpVec() * farHeight) - (Game.Instance.Player.GetRightVec() * farWidth), 1.0f);    // top right
                //cornerPoints[6] = new Vector4(fc + (Game.Instance.Player.GetUpVec() * farHeight) + (Game.Instance.Player.GetRightVec() * farWidth), 1.0f);    // bottom left
                //cornerPoints[7] = new Vector4(fc - (Game.Instance.Player.GetUpVec() * farHeight) + (Game.Instance.Player.GetRightVec() * farWidth), 1.0f);    // bottom right

                if (true)
                    ApplicationBase.Instance.debug_aid_frustum(new Vector3(cornerPoints[0]),
                        new Vector3(cornerPoints[1]),
                        new Vector3(cornerPoints[2]),
                        new Vector3(cornerPoints[3]),
                        new Vector3(cornerPoints[4]),
                        new Vector3(cornerPoints[5]),
                        new Vector3(cornerPoints[6]),
                        new Vector3(cornerPoints[7])
                        );




                foreach (LightSun sunLight in SunLights)
                {
                    Vector3 sphereCenter = BoundingAABB.CreateFromPoints(new Vector3[] {
                        new Vector3(cornerPoints[0]),
                        new Vector3(cornerPoints[1]),
                        new Vector3(cornerPoints[2]),
                        new Vector3(cornerPoints[3]),
                        new Vector3(cornerPoints[4]),
                        new Vector3(cornerPoints[5]),
                        new Vector3(cornerPoints[6]),
                        new Vector3(cornerPoints[7])
                    }).Center;


                    Vector3 sunDirection = sunLight.PointingDirection.Normalized();
                    Vector3 up = ApplicationBase.Instance.Player.GetUpVec(); // new Vector3(0f, 1f, 0f);
                    Matrix4 lightViewMat = Matrix4.LookAt(sphereCenter, sphereCenter + sunDirection, new Vector3(0f, 1f, 0f));
                    lightViewMat *= ApplicationBase.Instance.Player.ViewInfo.invModelviewMatrix;


                    // builds a projection matrix for rendering from the shadow's POV.
                    // First, it computes the appropriate z-range and sets an orthogonal projection.
                    // Then, it translates and scales it, so that it exactly captures the bounding box
                    // of the current frustum slice
                    Vector4 transf = lightViewMat * cornerPoints[0];
                    Vector4 transfInner = lightViewMat * cornerPoints[0];

                    float minZ = transf.Z;
                    float maxZ = transf.Z;

                    for (int k = 0; k < cornerPoints.Length; k++)
                    {
                        transf = lightViewMat * cornerPoints[k];
                        if (transf.Z > maxZ)
                            maxZ = transf.Z;
                        if (transf.Z < minZ)
                            minZ = transf.Z;
                    }

                    // make sure all relevant shadow casters are included
                    // note that these here are dummy objects at the edges of our scene
                    for (int k = 0; k < 4; k++)
                    {
                        transf = lightViewMat * new Vector4(obj_BSphere[k].Center, 1.0f);

                        if (transf.Z + obj_BSphere[k].Radius > maxZ)
                            maxZ = transf.Z + obj_BSphere[k].Radius;

                        if (transf.Z - obj_BSphere[k].Radius > minZ)
                            minZ = transf.Z - obj_BSphere[k].Radius;
                    }

                    // set the projection matrix with the new z-bounds
                    // note the inversion because the light looks at the neg. z axis
                    // gluPerspective(LIGHT_FOV, 1.0, maxZ, minZ); // for point lights
                    //lightProjMatrix[split] = Matrix4.CreateOrthographicOffCenter(-1.0f, 1.0f, -1.0f, 1.0f, -minZ, -maxZ);
                    //lightProjMatrixInner[split] = Matrix4.CreateOrthographicOffCenter(-1.0f, 1.0f, -1.0f, 1.0f, -minZ * 0.4f, -maxZ * 0.4f);



                    Matrix4 sunModelView = lightViewMat;//new Matrix4(sunLight.ViewInfo.modelviewMatrix.ToArray());
                    Matrix4 sunModelViewProjection = new Matrix4(sunLight.ViewInfo.modelviewProjectionMatrix.ToArray());

                    Matrix4 sunInnerModelView = new Matrix4(sunLight.innerViewInfo.modelviewMatrix.ToArray());
                    Matrix4 sunInnerModelViewProjection = new Matrix4(sunLight.innerViewInfo.modelviewProjectionMatrix.ToArray());

                    if (sunLight.Enabled)
                    {

                        lightProjMatrix[split] = Matrix4.CreateOrthographic(maxZ,
                                                                            maxZ,
                                                                            -minZ * 0.5f, maxZ * 0.5f);
                        lightProjMatrixInner[split] = Matrix4.CreateOrthographic(minZ,
                                                                                 maxZ,
                                                                                 -minZ * 0.5f, maxZ * 0.5f);

                        //lightProjMatrix[split] = Matrix4.CreateOrthographic(sunLight.maxShadowDist,
                        //                                                    sunLight.maxShadowDist,
                        //                                                   -sunLight.innerShadowDist * 0.5f, sunLight.innerShadowDist * 0.5f);
                        //lightProjMatrixInner[split] = Matrix4.CreateOrthographic(sunLight.innerShadowDist,
                        //                                                         sunLight.maxShadowDist,
                        //                                                         -sunLight.maxShadowDist * 0.5f, sunLight.maxShadowDist * 0.5f);




                        float maxX = -1000.0f;
                        float maxY = -1000.0f;
                        float minX = 1000.0f;
                        float minY = 1000.0f;

                        //float maxX = 120.0f;
                        //float maxY = 30.0f;
                        //float minX = -120.0f;
                        //float minY = -30.0f;

                        for (int k = 0; k < 8; k++)
                        {
                            transf = lightProjMatrix[split] * lightViewMat * cornerPoints[k];
                            transf.X /= transf.W;       // NOTE:: .a ???
                            transf.Y /= transf.W;

                            if (transf.X > maxX)
                                maxX = transf.X;
                            if (transf.X < minX)
                                minX = transf.X;
                            if (transf.Y > maxY)
                                maxY = transf.Y;
                            if (transf.Y < minY)
                                minY = transf.Y;
                        }

                        //build a crop matrix so we makes sure that we get the best precision in z and loose as little as possible in x and y
                        float scaleX = 2.0f / (maxX - minX);
                        float scaleY = 2.0f / (maxY - minY);
                        float offsetX = -0.5f * (maxX + minX) * scaleX;
                        float offsetY = -0.5f * (maxY + minY) * scaleY;

                        cropMat[split] = new Matrix4(new Vector4(scaleX, 0.0f, 0.0f, offsetX),
                                                     new Vector4(0.0f, scaleY, 0.0f, offsetY),
                                                     new Vector4(0.0f, 0.0f, 1.0f, 0.0f),
                                                     new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

                        sliceProjMatrix[split] = lightProjMatrix[split];


                        sunLight.ViewInfo.modelviewMatrix = lightViewMat;
                        sunLight.ViewInfo.invModelviewMatrix = Matrix4.Invert(lightViewMat);
                        sunLight.ViewInfo.projectionMatrix = lightProjMatrix[split];
                        sunLight.ViewInfo.GenerateViewProjectionMatrix();
                        //sunLight.ViewInfo.wasUpdated                = true;
                        //sunLight.ViewInfo.Update();
                        sunLight.ViewInfo.calculateVectors();
                        sunLight.shadowMatrix = Matrix4.Mult(sunModelViewProjection, Light.shadowBias);


                        sunLight.innerViewInfo.modelviewMatrix = lightViewMat;
                        sunLight.innerViewInfo.invModelviewMatrix = Matrix4.Invert(lightViewMat);
                        sunLight.innerViewInfo.projectionMatrix = lightProjMatrixInner[split];
                        sunLight.innerViewInfo.GenerateViewProjectionMatrix();
                        //sunLight.innerViewInfo.wasUpdated           = true;
                        //sunLight.innerViewInfo.Update();
                        sunLight.innerViewInfo.calculateVectors();
                        sunLight.innerShadowMatrix = Matrix4.Mult(sunInnerModelViewProjection, Light.shadowBias);

                        sunLight.shadowView = lightViewMat;
                        sunLight.shadowProj = lightProjMatrix[split];
                        sunLight.splitNo = split;

                        DrawShadowBuffer_PSSM_Split(split);
                    }

                    sunLight.ViewInfo.modelviewMatrix = sunModelView;
                    sunLight.ViewInfo.invModelviewMatrix = sunModelView.Inverted();

                    sunLight.ViewInfo.modelviewProjectionMatrix = sunModelViewProjection;
                    sunLight.ViewInfo.invModelviewProjectionMatrix = sunModelViewProjection.Inverted();
                    //sunLight.ViewInfo.GenerateViewProjectionMatrix();
                    sunLight.ViewInfo.calculateVectors();


                    sunLight.innerViewInfo.modelviewMatrix = sunInnerModelView;
                    sunLight.innerViewInfo.invModelviewMatrix = sunInnerModelView.Inverted();
                    sunLight.innerViewInfo.modelviewProjectionMatrix = sunInnerModelViewProjection;
                    sunLight.innerViewInfo.invModelviewProjectionMatrix = sunInnerModelViewProjection.Inverted();
                    //sunLight.innerViewInfo.GenerateViewProjectionMatrix();
                    sunLight.innerViewInfo.calculateVectors();
                }

                shadowFb[split].enable(false);
            }

            //DrawShadowBuffer_PSSM_Split(0);

        }
        internal void DrawShadowBuffer_PSSM_Split(int split)
        {
            //shadowFb.enable(false);

#region SpotLights
            for (int i = 0; i < Spotlights.Count; i++)
            {
                Spotlights[i].lightId = i;

                bool needsUpdate = Spotlights[i].viewInfo.checkForUpdates(Drawables);
                CurrentLight = i;

                if (needsUpdate)
                {
                    //ConsoleUtil.log(string.Format(
                    //    "{0} - Updating Shadowbuffer SpotLight-Id: {1}, No of Spotlights: {2}",
                    //    DateTime.Now.ToString("HH:mm:ss.fff"), i, Spotlights.Count));

                    //ApplicationBase.Instance.Renderer.Disable(EnableCap.Blend);
                    //ApplicationBase.Instance.Renderer.Enable(EnableCap.DepthTest);
                    //ApplicationBase.Instance.Renderer.Disable(EnableCap.CullFace);

                    ApplicationBase.Instance.Renderer.BlendEnabled = false;
                    ApplicationBase.Instance.Renderer.DepthTestEnabled = true;
                    ApplicationBase.Instance.Renderer.CullFaceEnabled = false;

                    ApplicationBase.Instance.Renderer.DepthFunc(DepthFunction.Always);
                    SceneRenderSurface.draw(enuShaderTypes.wipingShader, new int[] { Spotlights[i].ProjectionTexture },
                        Uniform.in_vector, new Vector2(i, LightCount));
                    ApplicationBase.Instance.Renderer.DepthFunc(DepthFunction.Less);

                    ApplicationBase.Instance.Renderer.ColorMask(false, false, false, true);
                    drawSceneFrame(enuRenderPass.shadow, Spotlights[i].viewInfo);
                    ApplicationBase.Instance.Renderer.ColorMask(true, true, true, true);

                }
            }
#endregion

            //ApplicationBase.Instance.Renderer.Disable(EnableCap.Blend);
            //ApplicationBase.Instance.Renderer.Enable(EnableCap.DepthTest);
            //ApplicationBase.Instance.Renderer.Disable(EnableCap.CullFace);
            ApplicationBase.Instance.Renderer.BlendEnabled = false;
            ApplicationBase.Instance.Renderer.DepthTestEnabled = true;
            ApplicationBase.Instance.Renderer.CullFaceEnabled = false;

            //"disable" writing to certain uv space
            int tmpLightCount = LightCount;
            LightCount = 1;
            CurrentLight = 0;

            if (split == 0)
                SunFrameBufferPssm[0].ClearColor = new Color4(0f, 0f, 1f, 1f);
            if (split == 1)
                SunFrameBufferPssm[1].ClearColor = new Color4(1f, 1f, 0f, 1f);
            if (split == 2)
                SunFrameBufferPssm[2].ClearColor = new Color4(0f, 1f, 1f, 1f);
            if (split == 3)
                SunFrameBufferPssm[3].ClearColor = new Color4(1f, 0f, 0f, 1f);

#region Sun
            foreach (LightSun SunLight in SunLights)
            // if (SunLight0.viewInfo.wasUpdated)
            {
                //generating wide range sun shadows
                SunFrameBufferPssm[split].enable(true); // is true    
                //SunFrameBuffer[split].enable(true); // is true

                ViewInfo sunView = new ViewInfo(SunLight.Parent);
                ViewInfo sunInnerView = new ViewInfo(SunLight.Parent);

                sunView = SunLight.ViewInfo;
                sunInnerView = SunLight.innerViewInfo;


                ApplicationBase.Instance.Renderer.ColorMask(false, false, false, false);
                if (SunLight.Enabled)
                    drawSceneFrame(enuRenderPass.shadow, sunView);
                //if (SunLight_Fill.Enabled)
                //    drawScene(enuRenderPass.shadow, SunLight_Fill.viewInfo);
                //if (SunLight_Back.Enabled)
                //    drawScene(enuRenderPass.shadow, SunLight_Back.viewInfo);
                ApplicationBase.Instance.Renderer.ColorMask(true, true, true, true);


                //generating short range sun shadows
                SunInnerFrameBufferPssm[split].enable(true);
                //SunInnerFrameBuffer[split].enable(true);
                ApplicationBase.Instance.Renderer.ColorMask(false, false, false, false);
                if (SunLight.Enabled)
                    drawSceneFrame(enuRenderPass.shadow, sunInnerView);
                //if (SunLight_Fill.Enabled)
                //    drawScene(enuRenderPass.shadow, SunLight_Fill.innerViewInfo);
                //if (SunLight_Back.Enabled)
                //    drawScene(enuRenderPass.shadow, SunLight_Back.innerViewInfo);
                ApplicationBase.Instance.Renderer.ColorMask(true, true, true, true);
            }
#endregion


            //clean up
            LightCount = tmpLightCount;
        }
#endif

        internal void DrawGuis()
        {
            foreach (var gui in Guis)
            {
                gui.draw();
            }
        }
    }
}
