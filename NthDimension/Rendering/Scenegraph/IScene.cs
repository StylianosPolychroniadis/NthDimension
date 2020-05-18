using NthDimension.Algebra;
using NthDimension.Rasterizer;
using NthDimension.Rendering.Configuration;
using NthDimension.Rendering.Drawables;
using NthDimension.Rendering.Drawables.Framebuffers;
using NthDimension.Rendering.GameViews;
using NthDimension.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Scenegraph
{
    #region enum RenderPass
    public enum enuEditorRenderPass
    {
        point,
        line,
        polygon,
        pointSelection,
        lineSelection,
        polygonSelection,
        all
    }
    #endregion


    public class SceneEditor : Drawable
    {
        private SceneGame   m_sceneGame;
        private Vector2     m_sceneGamma = Vector2.Zero;

        public Quad2d       RenderSurface;
        public string       Name            = "SceneEditor";

        
        public SceneGame    Scene
        {
            get { return m_sceneGame; }
            //set { m_sceneGame = value; }
        }

        #region Ctor
        public SceneEditor(SceneGame scene)
        {
            m_sceneGame = scene;
            RenderSurface = new Quad2d(this);  // Does this cause access violation changing scenes?
            m_sceneGamma.X = Settings.Instance.video.gamma;
        }
        #endregion


        internal void DrawBuffers(FramebufferSet curRenderComposite, ViewInfo curView)
        {
#if _DEVUI_
            return;
#endif
            RenderOptions renderOptions = curRenderComposite.renderOptions;

            ApplicationBase.Instance.CheckGlError(string.Format("(!) OpenGL Error: Entering Editor Scene Draw {0}:{1}", this.GetType(), Name));


            // Pass Grid
            RenderEditorFrame(enuEditorRenderPass.all, curView);


            #region Write Output (Composite) Pass
            curRenderComposite.OutputFb.enable(false);
            curRenderComposite.SceneFramebuffer.Multisampling = true;

            #endregion Write Output (Composite) Pass

            #region Send to Screen


            RenderSurface.draw(enuShaderFilters.composite,
                       new int[] {
                                                    curRenderComposite.SceneFramebuffer.ColorTexture,
                                                    //curFramebuffers.BloomFramebuffer2.ColorTexture,
                                                    curRenderComposite.SelectionFb.ColorTexture,
                                                    curRenderComposite.SelectionblurFb2.ColorTexture,
                                                    curRenderComposite.DofFramebuffer2.ColorTexture,
                                                    curRenderComposite.SceneFramebuffer.ColorTexture
                                  },
                       Uniform.in_vector,
                       m_sceneGamma);
            #endregion Send to Screen

        }

        private bool RenderEditorFrame(enuEditorRenderPass currentPass, ViewInfo curView)
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

            // Draw Calls Here ...
            this.drawCallAll(currentPass, curView);

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
        }


        private void drawCallAll(enuEditorRenderPass currentPass, ViewInfo curView)
        {
            if (currentPass == enuEditorRenderPass.all)
            {
#if MULTIPASS
                //AllOffsets.Clear();
                //Drawables.Sort(CompareByMaterial);
#endif

                ApplicationBase.Instance.Renderer.BlendEnabled = false;

                foreach (Drawable curDrawable in Scene.Drawables)
                {
                    if (curDrawable.IsVisible)
                    {
#if MEASUREPERFORMANCE
                        System.Diagnostics.Stopwatch drawTime = new System.Diagnostics.Stopwatch();
                        drawTime.Start();
#endif
                        //curDrawable.drawNormal(curView);
                        curDrawable.draw(curView, false);

#if MEASUREPERFORMANCE
                        long time = (drawTime.ElapsedTicks);
                        curDrawable.Performance.SetDiffusePassTime(time); // μSec
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








    }
}
